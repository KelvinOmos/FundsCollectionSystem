using Microsoft.AspNetCore.Mvc;
using Serilog;
using CollectionSystem.Application.DTOs.Account;
using CollectionSystem.Service.Application.Interfaces.Identity;

namespace CollectionSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }       
        
        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollAsync(RegisterRequest request)
        {
            return Ok(await _userService.EnrollAsync(request, GenerateIPAddress()));
        }
       
        [HttpGet("fetch-users")]
        //[Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetUsers()
        {
            Log.Information("Fetching Users Request");          
            return Ok(await _userService.GetAllUserAsync());
        }
        
        [HttpPost("get-user-by-id")]
        //[Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetUserById(viewUserDTO request)
        {
            Log.Information("Fetching User By Id Request");
            return Ok(await _userService.GetUserByIdAsync(request));
        }
        
        [HttpPost("update-user-status")]
        public async Task<IActionResult> UpdateUserStatus(UserStatusDTO userStatusDTO)
        {
            Log.Information("Update User Request");
            return Ok(await _userService.UpdateUserStatus(userStatusDTO));
        }

        [HttpPost("delete-user")]
        public async Task<IActionResult> DeleteUser(deleteUserDTO request)
        {
            Log.Information("Delete User Request");
            return Ok(await _userService.DeleteUser(request));
        }

        private string GenerateIPAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}