using Microsoft.AspNetCore.Mvc;
using CollectionSystem.Service.Application.Interfaces.Identity;
using CollectionSystem.Application.DTOs.Account;

namespace CollectionSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthenticationRequest request)
        {
            return Ok(await _authService.Token(request));
        }
    }
}
