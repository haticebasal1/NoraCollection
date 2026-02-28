using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoraCollection.API.Controllers.BaseController;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.AuthDtos;

namespace NoraCollection.API.Controllers
{
    [Route("api/auths")]
    [ApiController]
    public class AuthsController : CustomControllerBase
    {
        private readonly IAuthService _authManager;

        public AuthsController(IAuthService authManager)
        {
            _authManager = authManager;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authManager.LoginAsync(loginDto);
            return CreateResult(response);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var response = await _authManager.RegisterAsync(registerDto);
            return CreateResult(response);
        }
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] TokenDto? tokenDto)
        {
            var response = await _authManager.RefreshTokenAsync(tokenDto?.RefreshToken);
            return CreateResult(response);
        }
        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout([FromBody] TokenDto? tokenDto)
        {
            var response = await _authManager.LogoutAsync(tokenDto?.RefreshToken);
            return CreateResult(response);
        }
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var response = await _authManager.ForgotPasswordAsync(forgotPasswordDto);
            return CreateResult(response);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var response = await _authManager.ResetPasswordAsync(resetPasswordDto);
            return CreateResult(response);
        }
    }
}
