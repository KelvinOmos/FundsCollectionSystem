using CollectionSystem.Application.DTOs.Account;
using CollectionSystem.Application.Interfaces;
using CollectionSystem.Application.Wrappers;
using CollectionSystem.Domain.Entities;
using CollectionSystem.Domain.Settings;
using CollectionSystem.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using CollectionSystem.Service.Application.Interfaces.Identity;

namespace CollectionSystem.Infrastructure.Identity.Services
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;        
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;           
        private readonly ILogger<RoleService> _logger;
        private readonly IHttpContextAccessor _httpContext;
        protected HttpContext HttpContext => _httpContext.HttpContext;

        public RoleService(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager, IOptions<JWTSettings> jwtSettings,
            SignInManager<ApplicationUser> signInManager, ILogger<RoleService> logger, IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;            
            _signInManager = signInManager;
            _logger = logger;
            _httpContext = httpContext;            
        }        

        public async Task<Response<List<ApplicationRoleDto>>> GetRoles()
        {
            _logger.LogInformation("Entering Get Roles Service");
            try
            {
                var roles = _roleManager.Roles.ToList();
                if (roles != null)
                {
                    var rolesOutput = new List<ApplicationRoleDto>();
                    foreach (var role in roles)
                    {
                        rolesOutput.Add(new ApplicationRoleDto() { Id = role.Id, Name = role.Name });
                    }
                    return new Response<List<ApplicationRoleDto>>(data: rolesOutput, succeeded: true, code: (long)ApiResponseCodes.OK, message: "Successful");
                }
                return new Response<List<ApplicationRoleDto>>();
            }
            catch (Exception eex)
            {
                Log.Information($"Get Roles Exception :: {eex.ToString()}");
                return await Task.FromResult(new Response<List<ApplicationRoleDto>>() { Code = (long)ApiResponseCodes.EXCEPTION, Description = $"{((eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message)}" });
            }
        }
        
        public async Task<Response<ApplicationRoleDto>> GetRoleByName(string roleName)
        {
            _logger.LogInformation("Entering Get Role By Name Service");
            try
            {
                Response<ApplicationRoleDto> response = new Response<ApplicationRoleDto>();
                var role = _roleManager.Roles.FirstOrDefault(r => r.Name == roleName);
                if (role != null)
                {
                    response.Succeeded = true;
                    response.Data = new ApplicationRoleDto()
                    {
                        Id = role.Id,
                        Name = role.Name
                    };
                    return response;
                }
                return response;
            }
            catch (Exception eex)
            {
                Log.Information($"Get Role By Name Exception :: {eex.ToString()}");
                return await Task.FromResult(new Response<ApplicationRoleDto>() { Code = (long)ApiResponseCodes.EXCEPTION, Description = $"{((eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message)}" });
            }           
        }

        public async Task<Response<ApplicationRoleDto>> GetRoleById(UpdateRoleDTO request)
        {
            _logger.LogInformation("Entering Get Role By Id Service");
            try
            {
                Response<ApplicationRoleDto> response = new Response<ApplicationRoleDto>();
                var role = _roleManager.Roles.FirstOrDefault(r => r.Id == request.Id.ToString());
                if (role != null)
                {
                    response.Succeeded = true;
                    response.Data = new ApplicationRoleDto()
                    {
                        Id = role.Id,
                        Name = role.Name
                    };
                    response.Code = (long)ApiResponseCodes.OK;
                    return response;
                }
                return response;
            }
            catch (Exception eex)
            {
                Log.Information($"Get Role By Id Exception :: {eex.ToString()}");
                return await Task.FromResult(new Response<ApplicationRoleDto>() { Code = (long)ApiResponseCodes.EXCEPTION, Description = $"{((eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message)}" });
            }            
        }

        public async Task<Response<bool>> CreateRole(CreateRoleDTO roleDTO)
        {
            _logger.LogInformation("Entering Create Role Service");
            try
            {
                Response<bool> response = new Response<bool>();
                if (roleDTO != null || !string.IsNullOrEmpty(roleDTO.Name))
                {
                    string roleName = roleDTO.Name;
                    var roleExist = await GetRoleByName(roleName);
                    if (!roleExist.Succeeded)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(roleName));
                        response.Succeeded = true;
                        response.Data = true;
                        response.Description = "Role created successfully";
                        response.Code = (long)ApiResponseCodes.OK;
                    }
                    else
                    {
                        response.Succeeded = false;
                        response.Data = false;
                        response.Description = "Role exists";
                        response.Code = (long)ApiResponseCodes.FAIL;
                    }
                    return response;
                }
                response.Description = "Role name cannot be empty";
                response.Succeeded = false;
                return response;
            }
            catch (Exception eex)
            {
                Log.Information($"Create Role Exception :: {eex.ToString()}");
                return await Task.FromResult(new Response<bool>() { Code = (long)ApiResponseCodes.EXCEPTION, Description = $"{((eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message)}" });
            }
        }

        public async Task<Response<bool>> UpdateRole(UpdateRoleDTO roleDTO)
        {
            _logger.LogInformation("Entering Update Role Service");
            try
            {
                Response<bool> response = new Response<bool>();
                if (roleDTO != null || !string.IsNullOrEmpty(roleDTO.Name))
                {
                    var role = _roleManager.Roles.FirstOrDefault(r => r.Id == roleDTO.Id.ToString());
                    if (role != null)
                    {                        
                        role.Name = roleDTO.Name;
                        await _roleManager.UpdateAsync(role);
                        response.Succeeded = true;
                        response.Data = true;
                        response.Description = "Role updated successfully";
                        response.Code = (long)ApiResponseCodes.OK;
                    }
                    else
                    {
                        response.Succeeded = false;
                        response.Data = false;
                        response.Description = "Role does not exists";
                        response.Code = (long)ApiResponseCodes.FAIL;
                    }
                    return response;
                }
                response.Description = "Role name cannot be empty";
                response.Succeeded = false;
                return response;
            }
            catch (Exception eex)
            {
                Log.Information($"Update Role Exception :: {eex.ToString()}");
                return await Task.FromResult(new Response<bool>() { Code = (long)ApiResponseCodes.EXCEPTION, Description = $"{((eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message)}" });
            }

        }

        public async Task<Response<bool>> DeleteRole(DeleteRoleDTO roleDTO)
        {
            _logger.LogInformation("Entering Delete Role Service");
            try
            {
                Response<bool> response = new Response<bool>();
                if (roleDTO != null || !string.IsNullOrEmpty(roleDTO.Name))
                {
                    string roleName = roleDTO.Name;
                    var roleExist = _roleManager.Roles.FirstOrDefault(r => r.Name == roleName);
                    if (roleExist != null)
                    {
                        await _roleManager.DeleteAsync(roleExist);
                        response.Succeeded = true;
                        response.Data = true;
                        response.Description = "Role deleted successfully";
                        response.Code = (long)ApiResponseCodes.OK;
                    }
                    else
                    {
                        response.Succeeded = false;
                        response.Data = false;
                        response.Description = "Role does not exists";
                        response.Code = (long)ApiResponseCodes.FAIL;
                    }
                    return response;
                }
                response.Description = "Role name cannot be empty";
                response.Succeeded = false;
                return response;
            }
            catch (Exception eex)
            {
                Log.Information($"Delete Role Exception :: {eex.ToString()}");
                return await Task.FromResult(new Response<bool>() { Code = (long)ApiResponseCodes.EXCEPTION, Description = $"{((eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message)}" });
            }            
        }
    }
}
 