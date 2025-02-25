using dimax_front.Application.Interfaces;
using dimax_front.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dimax_front.Presentation.Controllers
{

    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<ActionResult> HandleRegister([FromBody] RegisterDTO registerDto)
        {
            var response = await _authService.RegisterAsync(registerDto);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });

            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });
        }

        [HttpPost("login")]
        public async Task<ActionResult> HandleLogin([FromBody] LoginDTO loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });
            
            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message, 
                response.AccessToken, response.RefreshToken });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> HandleRefreshToken(RefreshTokenRequestDTO request)
        {
            var response = await _authService.RefreshTokenAsync(request);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });

            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message, 
                response.AccessToken, response.RefreshToken });
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Your are authenticated");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult GetForRoles()
        {
            return Ok("Your are an admin");
        }
    }
}
