//using Microsoft.AspNetCore.Mvc;
//using Serilog;
//using Microsoft.AspNetCore.Authorization;
//using CollectionSystem.Service.Application.Interfaces.Identity;
//using CollectionSystem.Application.DTOs.Account;

//namespace CollectionSystem.WebApi.Controllers
//{
//    [Authorize]
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RoleController : ControllerBase
//    {
//        private readonly IRoleService _roleService;

//        public RoleController(IRoleService roleService)
//        {            
//            _roleService = roleService;
//        }        

//        [HttpPost("create-role")]
//        public async Task<IActionResult> CreateRole(CreateRoleDTO request)
//        {
//            Log.Information("Create Role Service");
//            return Ok(await _roleService.CreateRole(request));
//        }

//        [HttpPost("update-role")]
//        public async Task<IActionResult> UpdateRole(UpdateRoleDTO request)
//        {
//            Log.Information("Update Role Service");
//            return Ok(await _roleService.UpdateRole(request));
//        }

//        [HttpPost("delete-role")]
//        public async Task<IActionResult> DeleteRole(DeleteRoleDTO request)
//        {
//            Log.Information("Delete Role Service");
//            return Ok(await _roleService.DeleteRole(request));
//        }

//        [HttpGet("get-all-roles")]
//        public async Task<IActionResult> GetRoles()
//        {
//            Log.Information("Get Roles Service");
//            return Ok(await _roleService.GetRoles());
//        }

//        [HttpPost("get-role-by-name")]
//        public async Task<IActionResult> GetRoleByName(string roleName)
//        {
//            Log.Information("Get Role By Name Service");
//            return Ok(await _roleService.GetRoleByName(roleName));
//        }

//        [HttpPost("get-role-by-id")]
//        public async Task<IActionResult> GetRoleById(UpdateRoleDTO request)
//        {
//            Log.Information("Get Role By Id Service");
//            return Ok(await _roleService.GetRoleById(request));
//        }
//    }
//}
