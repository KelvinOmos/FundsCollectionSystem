using CollectionSystem.Application.DTOs.Account;
using CollectionSystem.Application.Interfaces;
using CollectionSystem.Application.Wrappers;
using CollectionSystem.Domain.Entities;
using CollectionSystem.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Data;
using Microsoft.AspNetCore.Http;
using CollectionSystem.Infrastructure.Identity.Contexts;
using CollectionSystem.Service.Application.Interfaces.Identity;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CollectionSystem.Application.Enums;

namespace CollectionSystem.Infrastructure.Identity.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserRoleService> _logger;
        private readonly IHttpContextAccessor _httpContext;
        protected HttpContext HttpContext => _httpContext.HttpContext;
        private readonly IdentityContext _identityContext;

        public UserRoleService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            ILogger<UserRoleService> logger, IHttpContextAccessor httpContext, IdentityContext identityContext)
        {
            _identityContext = identityContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _httpContext = httpContext;
        }

        public async Task<Response<List<ApplicationUserRoleDto>>> GetAllUserRoles(string userId)
        {
            List<ApplicationUserRoleDto> userRoles = new List<ApplicationUserRoleDto>();
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                string retResponse = $"No Profile details found for {userId}.";
                Log.Information(retResponse);
                return new Response<List<ApplicationUserRoleDto>>(succeeded: false, message: retResponse, responseCode: "02", data: null);
            }
            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles.Where(r => roleNames.Contains(r.Name)).ToList();

            foreach (var role in roles)
            {
                userRoles.Add(new ApplicationUserRoleDto()
                {
                    Role = role.Name,
                    RoleId = Guid.Parse(role.Id),
                    UserId = Guid.Parse(user.Id)
                });
            }

            return new Response<List<ApplicationUserRoleDto>>(succeeded: true, message: "FETCH USER ROLES SUCCESS", responseCode: "00", data: userRoles);
        }

        public async Task<Response<string>> AddUserToRole(string userID, string role)
        {
            try
            {
                if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(role))
                {
                    return null;
                }
                var user = await _userManager.FindByEmailAsync(userID);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(userID);
                }
                if (user == null)
                {
                    user = await _userManager.FindByIdAsync(userID);
                }
                if (user == null)
                {
                    string retResponse = $"No Profile details found for {userID}.";
                    Log.Information(retResponse);
                    return new Response<string>(succeeded: false, message: retResponse, responseCode: "02", data: null);
                }
                var result = _userManager.AddToRoleAsync(user, role);
                var response = new Response<string>(result.Result.Succeeded.ToString(), $"User successfully added to role {role}");
                return response;
            }
            catch (Exception ex)
            {
                return new Response<string>(succeeded: false, message: ex?.Message, responseCode: "02", data: null);
            }

        }

        public async Task<Response<bool>> AddUserRole(AddApplicationUserRoles request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (request.userId == null)
                {
                    return await Task.FromResult(new Response<bool>(succeeded: false, message: "User cannot be empty", data: false, code: (long)ApiResponseCodes.INVALID_REQUEST));
                }
                if (request.roleNames.Count <= 0)
                {
                    return await Task.FromResult(new Response<bool>(succeeded: false, message: "Role cannot be empty", data: false, code: (long)ApiResponseCodes.INVALID_REQUEST));
                }
                string userID = request.userId.ToString();
                var user = await _userManager.FindByIdAsync(userID);               
                if (user == null)
                {
                    string retResponse = $"No Profile details found for {userID}.";
                    Log.Information(retResponse);
                    return new Response<bool>(succeeded: false, message: retResponse, code: (long)ApiResponseCodes.INVALID_REQUEST, data: false);
                }
                Response<bool> response = new Response<bool>();
                var userRoleExist = false;
                foreach(var roleName in request.roleNames)
                {
                    userRoleExist = await _userManager.IsInRoleAsync(user, roleName);
                    if (userRoleExist) { break; }
                }                
                if (userRoleExist)
                {
                    return await Task.FromResult(new Response<bool>(succeeded: false, message: "User has one or more role", data: false, code: (long)ApiResponseCodes.INVALID_REQUEST));
                }
                var result = await _userManager.AddToRolesAsync(user, request.roleNames);
                response = new Response<bool>() { Succeeded = result.Succeeded, Code = (long)ApiResponseCodes.OK, Description = "Successful" };               
                return response;
            }
            catch (Exception eex)
            {
                Log.Information($"Get Roles Exception :: {eex.ToString()}");
                return await Task.FromResult(new Response<bool>() { Code = (long)ApiResponseCodes.EXCEPTION, Description = $"{((eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message)}" });
            }
        }

        public async Task<Response<bool>> DeleteUserRole(DeleteApplicationUserRoleDto request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (request.UserId == null)
                {
                    return await Task.FromResult(new Response<bool>(succeeded: false, message: "User cannot be empty", data: false, code: (long)ApiResponseCodes.INVALID_REQUEST));
                }
                if (request.RoleId == null)
                {
                    return await Task.FromResult(new Response<bool>(succeeded: false, message: "Role cannot be empty", data: false, code: (long)ApiResponseCodes.INVALID_REQUEST));
                }
                Response<bool> response = new Response<bool>();
                var roleExist = _identityContext.UserRoles.FirstOrDefault(x => x.UserId == request.UserId.ToString() && x.RoleId == request.RoleId.ToString());
                if (roleExist != null)
                {
                    _identityContext.UserRoles.Remove(roleExist);
                    await _identityContext.SaveChangesAsync();
                    return await Task.FromResult(new Response<bool>(succeeded: true, message: "Role removed from user successfully", data: true, code: (long)ApiResponseCodes.OK));
                }
                return await Task.FromResult(new Response<bool>(succeeded: false, message: "User does not have the role", data: false, code: (long)ApiResponseCodes.INVALID_REQUEST));
            }
            catch (Exception eex)
            {
                Log.Information($"Get Roles Exception :: {eex.ToString()}");
                return await Task.FromResult(new Response<bool>() { Code = (long)ApiResponseCodes.EXCEPTION, Description = $"{((eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message)}" });
            }
        }

        public async Task<Response<bool>> DeleteBatchUserRole(DeleteBatchApplicationUserRoleDto userRoleDto)
        {
            try
            {
                if (userRoleDto == null) throw new ArgumentNullException(nameof(userRoleDto));
                if (userRoleDto.UserId == null)
                {
                    return await Task.FromResult(new Response<bool>(succeeded: false, message: "User cannot be empty", data: false, code: (long)ApiResponseCodes.INVALID_REQUEST));
                }
                if (userRoleDto.RoleNames.Count < 1)
                {
                    return await Task.FromResult(new Response<bool>(succeeded: false, message: "Role cannot be empty", data: false, code: (long)ApiResponseCodes.INVALID_REQUEST));
                }
                
                Response<bool> response = new Response<bool>();
                var userRolesFromDb = _identityContext.UserRoles.Where(x => x.UserId == userRoleDto.UserId.ToString()).ToList();
                if(userRolesFromDb.Count() > 0)
                {
                    string userID = userRoleDto.UserId.ToString();
                    var user = await _userManager.FindByIdAsync(userID);
                    if (user == null)
                    {
                        string retResponse = $"No Profile details found for {userID}.";
                        Log.Information(retResponse);
                        return new Response<bool>(succeeded: false, message: retResponse, code: (long)ApiResponseCodes.INVALID_REQUEST, data: false);
                    }
                    var result = _userManager.RemoveFromRolesAsync(user, userRoleDto.RoleNames);
                    response = new Response<bool>() { Succeeded = true, Code = (long)ApiResponseCodes.OK, Description = "Successful" };
                }
                response = new Response<bool>() { Succeeded = false, Code = (long)ApiResponseCodes.INVALID_REQUEST, Description = "User has no role to delete" };
                return response;
            }
            catch (Exception eex)
            {
                Log.Information($"Get Roles Exception :: {eex.ToString()}");
                return await Task.FromResult(new Response<bool>() { Code = (long)ApiResponseCodes.EXCEPTION, Description = $"{((eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message)}" });
            }
        }

        public async Task<Response<bool>> UpdateUserRole(UpdateApplicationUserRoleDto request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (request.UserId == null)
                {
                    return await Task.FromResult(new Response<bool>(succeeded: false, message: "User Id cannot be empty", data: false, code: (long)ApiResponseCodes.INVALID_REQUEST));
                }
                string userID = request.UserId.ToString();
                var user = await _userManager.FindByIdAsync(userID);
                if (user == null)
                {
                    string retResponse = $"No Profile details found for {userID}.";
                    Log.Information(retResponse);
                    return new Response<bool>(succeeded: false, message: retResponse, code: (long)ApiResponseCodes.INVALID_REQUEST, data: false);
                }
                Response<bool> response = new Response<bool>();
                var userRolesFromDb = _identityContext.UserRoles.Where(x => x.UserId == userID).ToList();
                if (userRolesFromDb.Any())
                {
                    _identityContext.UserRoles.RemoveRange(userRolesFromDb);
                    await _identityContext.SaveChangesAsync();                    
                    if(request.RoleNames.Count() > 0)
                    {
                      await _userManager.AddToRolesAsync(user, request.RoleNames);
                    }
                    return new Response<bool>() { Succeeded = true, Code = (long)ApiResponseCodes.OK, Description = "Successful" };
                }                
                return new Response<bool>() { Succeeded = false, Code = (long)ApiResponseCodes.INVALID_REQUEST, Description = "User has no role" };
            }
            catch (Exception eex)
            {
                Log.Information($"Get Roles Exception :: {eex.ToString()}");
                return await Task.FromResult(new Response<bool>() { Code = (long)ApiResponseCodes.EXCEPTION, Description = $"{((eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message)}" });
            }
        }

        public async Task<Response<List<ApplicationUserRoleDto>>> GetUserRole(GetUserRolesDto userRoleDto)
        {
            if (string.IsNullOrWhiteSpace(Convert.ToString(userRoleDto.userId)))
                return await Task.FromResult(new Response<List<ApplicationUserRoleDto>>() { Succeeded = false, Message = "User ID cannot be null", Code = (long)ApiResponseCodes.INVALID_REQUEST });
            var RetVal = from userRole in _identityContext.UserRoles.AsTracking().Where(x => x.UserId == userRoleDto.userId.ToString())
                         join user in _identityContext.Users.AsTracking() on userRole.UserId equals user.Id
                         join role in _identityContext.Roles.AsTracking() on userRole.RoleId equals role.Id
                         select new ApplicationUserRoleDto
                         {
                             //Id = userRole.Id,
                             UserId = Guid.Parse(userRole.UserId),
                             User = user.FirstName,
                             RoleId = Guid.Parse(role.Id),
                             Role = role.Name
                         };
            if (RetVal.Any())
            {
                var data = new List<ApplicationUserRoleDto>();
                foreach (var item in RetVal)
                {
                    data.Add(new ApplicationUserRoleDto() { Id = item.Id, UserId = item.UserId, User = item.User, Role = item.Role, RoleId = item.RoleId });
                }
                return await Task.FromResult(new Response<List<ApplicationUserRoleDto>>() { Succeeded = true, Data = data, Message = "Successful", Code = (long)ApiResponseCodes.OK });
            }
            return await Task.FromResult(new Response<List<ApplicationUserRoleDto>>(succeeded: false, message: "User Role doesn't exist", data: null, code: (long)ApiResponseCodes.FAIL));
        }

        public async Task<Response<List<ApplicationUserRoleDto>>> GetAllUserRoles()
        {
            _logger.LogInformation("Entering Get User Roles Service");
            var userRoles = from userRole in _identityContext.UserRoles.AsTracking()
                            join user in _identityContext.Users.AsTracking().Where(u=>u.Status != (int)UserStatuses.Deleted) on userRole.UserId equals user.Id
                            join role in _identityContext.Roles.AsTracking() on userRole.RoleId equals role.Id
                            select new ApplicationUserRoleDto
                            {
                                //Id = userRole.Id,
                                UserId = Guid.Parse(userRole.UserId),
                                User = user.FirstName,
                                RoleId = Guid.Parse(role.Id),
                                Role = role.Name
                            };
            if (userRoles.Any())
            {
                var userRolesOutput = new List<ApplicationUserRoleDto>();
                foreach (var userRole in userRoles)
                {
                    userRolesOutput.Add(new ApplicationUserRoleDto() { Id = userRole.Id, UserId = userRole.UserId, RoleId = userRole.RoleId, User = userRole.User, Role = userRole.Role });
                }
                return new Response<List<ApplicationUserRoleDto>>(succeeded: true, code: (long)ApiResponseCodes.OK, message: "Successful", data: userRolesOutput);
            }
            return new Response<List<ApplicationUserRoleDto>>();
        }
    }
}
