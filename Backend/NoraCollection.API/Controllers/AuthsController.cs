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
    }
}
