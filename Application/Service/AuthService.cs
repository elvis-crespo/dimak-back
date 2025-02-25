using dimax_front.Application.Interfaces;
using dimax_front.Application.Validators;
using dimax_front.Domain.DTOs;
using dimax_front.Domain.Entities;
using dimax_front.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace dimax_front.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly WorkshopDbContext _workshopDb;
        private readonly IConfiguration _configuration;

        public AuthService(WorkshopDbContext workshopDb, IConfiguration configuration)
        {
            _workshopDb = workshopDb;
            _configuration = configuration;
        }

        public async Task<ServiceResponse.GeneralResponse> RegisterAsync(RegisterDTO registerDto)
        {
            try
            {
                // Agrupar todas las validaciones en una lista
                var validationResponses = new List<ServiceResponse.GeneralResponse?>
                {
                    UserValidator.ValidateUsername(registerDto.Username),
                    UserValidator.ValidatePassword(registerDto.Password),
                    UserValidator.ValidateEmail(registerDto.Email),
                    UserValidator.ValidateRole(registerDto.Role)
                };

                // Comprobar si alguna validación falló
                var errorResponse = validationResponses.FirstOrDefault(r => r != null);
                if (errorResponse != null)
                {
                    return errorResponse; // Devuelve el primer error encontrado
                }
                // Check if user already exists
                var existingUser = await _workshopDb.Users.FirstOrDefaultAsync(u => u.Username == registerDto.Username);

                if (existingUser != null)
                {
                    return new ServiceResponse.GeneralResponse(
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status400BadRequest,
                        Message: "User already exists"
                    );
                }

                // Hash password
                var passwordHasher = new PasswordHasher<User>();
                var hashedPassword = passwordHasher.HashPassword(new User(), registerDto.Password);

                // Add new user
                var newUser = new User
                {
                    Username = registerDto.Username,
                    PasswordHash = hashedPassword,
                    Email = registerDto.Email,
                    Role = registerDto.Role
                };

                await _workshopDb.Users.AddAsync(newUser);
                await _workshopDb.SaveChangesAsync();

                return new ServiceResponse.GeneralResponse(
                    IsSuccess: true,
                    StatusCode: StatusCodes.Status201Created,
                    Message: "User registered successfully"
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse.GeneralResponse(
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status500InternalServerError,
                    Message: $"Internal server error {ex.Message}"
                );
            }
        }

        public async Task<ServiceResponse.TokenResponse> LoginAsync(LoginDTO loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                return new ServiceResponse.TokenResponse(
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "Invalid login request",
                    AccessToken: string.Empty,
                    RefreshToken: string.Empty
                );

            // Agrupar todas las validaciones en una lista
            var validationResponses = new List<ServiceResponse.GeneralResponse?>
                {
                    UserValidator.ValidateUsername(loginDto.Username),
                    UserValidator.ValidatePassword(loginDto.Password),
                };

            // Comprobar si alguna validación falló
            var errorResponse = validationResponses.FirstOrDefault(r => r != null);
            if (errorResponse != null)
            {
                return new ServiceResponse.TokenResponse(
                    IsSuccess: false,
                    StatusCode: errorResponse.StatusCode,
                    Message: errorResponse.Message,
                    AccessToken: string.Empty,
                    RefreshToken: string.Empty
                );
            }

            try
            {
                // Check if user exists
                var existingUser = await _workshopDb.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

                if (existingUser == null || loginDto.Username != existingUser.Username)
                    return new ServiceResponse.TokenResponse(
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status404NotFound,
                        Message: "User does not exist",
                        AccessToken: string.Empty,
                        RefreshToken: string.Empty
                    );

                // Verify password
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, loginDto.Password);

                if (result == PasswordVerificationResult.Failed)
                    return new ServiceResponse.TokenResponse(
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status401Unauthorized,
                        Message: "Wrong password",
                        AccessToken: string.Empty,
                        RefreshToken: string.Empty
                    );

                // Create token response
                var response = await CreateTokenResponse(existingUser);

                return new ServiceResponse.TokenResponse(
                    IsSuccess: response.IsSuccess,
                    StatusCode: response.StatusCode,
                    Message: response.Message,
                    AccessToken: response.AccessToken,
                    RefreshToken: response.RefreshToken
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse.TokenResponse(
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status500InternalServerError,
                    Message: $"Internal server error {ex.Message}",
                    AccessToken: string.Empty,
                    RefreshToken: string.Empty
                );
            }
        }


        public async Task<ServiceResponse.TokenResponse> RefreshTokenAsync(RefreshTokenRequestDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.RefreshToken) || request.UserId == Guid.Empty)
            {
                return new ServiceResponse.TokenResponse(
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "Invalid request data",
                    AccessToken: string.Empty,
                    RefreshToken: string.Empty
                );
            }

            try
            {
                // Validate refresh token
                var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
                if (user == null)
                {
                    return new ServiceResponse.TokenResponse(
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status400BadRequest,
                        Message: "Invalid refresh token or token expired",
                        AccessToken: string.Empty,
                        RefreshToken: string.Empty
                    );
                }

                // Check if access token has expired
                if (user.TokenExpiryTime.HasValue && user.TokenExpiryTime > DateTime.UtcNow)
                {
                    return new ServiceResponse.TokenResponse(
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status400BadRequest,
                        Message: "Access token is still valid",
                        AccessToken: string.Empty,
                        RefreshToken: string.Empty
                    );
                }

                // Create new token and refresh token
                //var newAccessToken = CreateToken(user);  // This will update the user's token and token expiry time

                //// You do NOT need to regenerate the refresh token here, only the access token
                //return new ServiceResponse.TokenResponse(
                //    IsSuccess: true,
                //    StatusCode: StatusCodes.Status200OK,
                //    Message: "New access token generated successfully",
                //    AccessToken: newAccessToken,
                //    RefreshToken: request.RefreshToken // Keep the same refresh token
                //);
                var response = await CreateTokenResponse(user);

                return new ServiceResponse.TokenResponse(
                    IsSuccess: response.IsSuccess,
                    StatusCode: response.StatusCode,
                    Message: response.Message,
                    AccessToken: response.AccessToken,
                    RefreshToken: response.RefreshToken
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse.TokenResponse(
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status500InternalServerError,
                    Message: $"Internal server error {ex.Message}",
                    AccessToken: string.Empty,
                    RefreshToken: string.Empty
                );
            }
        }
        //Method to create token response
        private async Task<ServiceResponse.TokenResponse> CreateTokenResponse(User user)
        {
            var accessToken = CreateToken(user);
            var refreshToken = await GenerateAndSaveRefreshTokenAsync(user);

            return new ServiceResponse.TokenResponse(
                IsSuccess: true,
                StatusCode: StatusCodes.Status200OK,
                Message: "User logged in successfully",
                AccessToken: accessToken,
                RefreshToken: refreshToken
            );
        }

        //validate refresh token
        private async Task<User> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _workshopDb.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshToken != refreshToken 
                || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return null;
            }
            return user;
        }

        //Method to generate refresh token
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        // Generate and save refresh token
        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(8);

            await _workshopDb.SaveChangesAsync();
            return refreshToken;
        }

        //Method to create token
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("email", user.Email)
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

        
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(10);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            // Guardar el token y la expiración en la base de datos
            user.Token = token;
            user.TokenExpiryTime = expiration;
            _workshopDb.Users.Update(user); 
            _workshopDb.SaveChanges();

            return token;
        }
    }
}
