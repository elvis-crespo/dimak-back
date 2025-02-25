using dimax_front.Domain.DTOs;

namespace dimax_front.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse.GeneralResponse> RegisterAsync(RegisterDTO registerDto);
        Task<ServiceResponse.TokenResponse> LoginAsync(LoginDTO loginDto);
        Task<ServiceResponse.TokenResponse> RefreshTokenAsync(RefreshTokenRequestDTO request);
    }
}
