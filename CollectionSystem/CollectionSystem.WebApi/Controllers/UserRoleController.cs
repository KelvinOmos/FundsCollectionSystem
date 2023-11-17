//using Microsoft.AspNetCore.Mvc;
//using Serilog;
//using Microsoft.AspNetCore.Authorization;
//using CollectionSystem.Service.Application.Interfaces.Identity;
//using CollectionSystem.Application.DTOs.Account;
//using CollectionSystem.Infrastructure.Identity.Services;

//namespace CollectionSystem.WebApi.Controllers
//{
//        [Authorize]
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserRoleController : ControllerBase
//    {
//        private readonly IUserRoleService _userRoleService;

//        public UserRoleController(IUserRoleService userRoleService)
//        {
//            _userRoleService = userRoleService;
//        }

//        [HttpPost("add-user-role")]
//        public async Task<IActionResult> AddUserRole(AddApplicationUserRoles request)
//        {
//            Log.Information("Add User Role Service");
//            return Ok(await _userRoleService.AddUserRole(request));
//        }

//        [HttpPost("get-user-role")]
//        public async Task<IActionResult> GetUserRole(GetUserRolesDto request)
//        {
//            Log.Information("Get User Role Service");
//            return Ok(await _userRoleService.GetUserRole(request));
//        }

//        [HttpGet("get-all-user-roles")]
//        public async Task<IActionResult> GetAllUserRoles()
//        {
//            Log.Information("Get All User Role Service");
//            return Ok(await _userRoleService.GetAllUserRoles());
//        }

//        [HttpPost("update-user-roles")]
//        public async Task<IActionResult> UpdateUserRole(UpdateApplicationUserRoleDto request)
//        {
//            Log.Information("Update User Role Service");
//            return Ok(await _userRoleService.UpdateUserRole(request));
//        }

//        [HttpPost("delete-user-role")]
//        public async Task<IActionResult> DeleteUserRole(DeleteApplicationUserRoleDto request)
//        {
//            Log.Information("Delete User Role Service");
//            return Ok(await _userRoleService.DeleteUserRole(request));
//        }

//        [HttpPost("delete-batch-user-role")]
//        public async Task<IActionResult> DeleteBatchUserRole(DeleteBatchApplicationUserRoleDto request)
//        {
//            Log.Information("Delete Batch User Role Service");
//            return Ok(await _userRoleService.DeleteBatchUserRole(request));
//        }
//    }
//}
