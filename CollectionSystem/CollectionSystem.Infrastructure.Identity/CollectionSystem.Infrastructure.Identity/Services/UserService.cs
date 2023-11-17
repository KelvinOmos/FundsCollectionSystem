using CollectionSystem.Application.DTOs.Account;
using CollectionSystem.Application.Interfaces;
using CollectionSystem.Application.Wrappers;
using CollectionSystem.Domain.Entities;
using CollectionSystem.Domain.Settings;
using CollectionSystem.Infrastructure.Identity.Helpers;
using CollectionSystem.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using CollectionSystem.Application.Enums;
using CollectionSystem.Service.Application.Interfaces.Identity;

namespace CollectionSystem.Infrastructure.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContext;
        protected HttpContext HttpContext => _httpContext.HttpContext;

        public UserService(UserManager<ApplicationUser> userManager, IOptions<JWTSettings> jwtSettings, SignInManager<ApplicationUser> signInManager, ILogger<AuthService> logger, IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
            _logger = logger;
            _httpContext = httpContext;
        }

        public async Task<Response<bool>> EnrollAsync(RegisterRequest request, string origin)
        {
            Log.Information("WELCOME TO ENROLL USER SERVICE");
            bool retStatus = false;
            var retResponse = "";
            try
            {
                var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
                if (userWithSameUserName != null)
                {
                    retResponse = $"UserName {request.UserName} already exist";
                    Log.Error(retResponse);
                    return await Task.FromResult(new Response<bool>() { Code = (long)ApiResponseCodes.INVALID_REQUEST, Description = retResponse, Data = false });
                }
                var user = new Models.ApplicationUser
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName
                };
                var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
                if (userWithSameEmail == null)
                {
                    user.EmailConfirmed = true;
                    user.Status = (int)UserStatuses.Active;
                    var result = await _userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, request.Role);
                        retStatus = true;
                        retResponse = $"User {request.UserName} Enrolled successfully";
                        Log.Information(retResponse);
                        return await Task.FromResult(new Response<bool>() { Code = (long)ApiResponseCodes.OK, Description = retResponse, Data = true });
                    }
                    else
                    {
                        retResponse = $"Fail to Save New User :: ";
                        List<string> errors = new List<string>();
                        foreach (var error in result.Errors)
                        {
                            string errorVal = error.Description;
                            retResponse += errorVal;
                            errors.Add(errorVal);
                        }
                        Log.Error(retResponse);
                        return await Task.FromResult(new Response<bool>() { Code = (long)ApiResponseCodes.ERROR, Description = retResponse, Errors = errors });
                    }
                }
                else
                {
                    retResponse = $"Email - {request.Email} used already exist";
                    Log.Error(retResponse);
                    return await Task.FromResult(new Response<bool>() { Code = (long)ApiResponseCodes.INVALID_REQUEST, Description = retResponse, Data = false });
                }
            }
            catch (Exception eex)
            {
                Log.Error($"USER ENROLLMENT EXCEPTION:: {eex?.ToString()}");
                retResponse = eex?.Message;
                return await Task.FromResult(new Response<bool>() { Code = (long)ApiResponseCodes.EXCEPTION, Description = retResponse });
            }
            //finally
            //{
            //    var auditData = await _audit.ServAddAsync(new AuditTrail
            //    {
            //        Function = "Enroll User",
            //        Request = JsonConvert.SerializeObject(request),
            //        ApiResponse = retResponse,
            //        Status = retStatus.ToString(),
            //        Created = DateTime.Now
            //    });
            //}
        }

        public async Task<Response<List<ApplicationUserDto>>> GetAllUserAsync()
        {
            Log.Information("WELCOME TO GET ALL USERS SERVICE");
            bool retStatus = false;
            var retResponse = "";
            var userRespList = new List<ApplicationUserDto>();
            try
            {
                var usersObj = _userManager.Users.Where(u => u.Status != (int)UserStatuses.Deleted).ToList();
                if (usersObj == null)
                {
                    Log.Information($"No User Profile found");
                    retResponse = $"No User Profile found";
                    return await Task.FromResult(new Response<List<ApplicationUserDto>>(succeeded: false, message: $"No Profiles found.", responseCode: "02", data: null));
                }
                foreach (var user in usersObj)
                {
                    var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
                    userRespList.Add(new ApplicationUserDto() { Id = user.Id, UserName = user.UserName, Email = user.Email, LastName = user.LastName, FirstName = user.FirstName, Status = user.Status, Role = role });
                }
                return await Task.FromResult(new Response<List<ApplicationUserDto>>() { Data = userRespList, Description = "Users Profile Fetch Successfull", Message = "Users Profile Fetched Successfully", Succeeded = true, Code = (long)ApiResponseCodes.OK });
            }
            catch (Exception ex)
            {
                Log.Error($"ALL USERS EXCEPTION:: {ex?.ToString()}");
                retResponse = ex?.Message;
                return await Task.FromResult(new Response<List<ApplicationUserDto>>(succeeded: false, message: $"Error:: {ex?.Message}", data: null));
            }
        }

        public async Task<Response<ApplicationUserDto>> GetUserByIdAsync(viewUserDTO request)
        {
            Log.Information("WELCOME TO GET USER SERVICE");
            bool retStatus = false;
            var retResponse = "";
            try
            {
                var userObj = _userManager.Users.FirstOrDefault(u => u.Id == request.Id);
                if (userObj == null)
                {
                    Log.Information($"No User Profile found");
                    retResponse = $"No User Profile found";
                    return await Task.FromResult(new Response<ApplicationUserDto>(succeeded: false, message: $"No Profiles found.", responseCode: "02", data: null));
                }

                var userResp = new ApplicationUserDto() { Id = userObj.Id, UserName = userObj.UserName, LastName = userObj.LastName, FirstName = userObj.FirstName, Status = userObj.Status, Email = userObj.Email };

                userResp.Role = _userManager.GetRolesAsync(userObj).Result.FirstOrDefault();

                return await Task.FromResult(new Response<ApplicationUserDto>() { Data = userResp, Description = "User Profile Fetch Successful", Message = "User Profile Fetch Successful", Succeeded = true, Code = (long)ApiResponseCodes.OK });
            }
            catch (Exception ex)
            {
                Log.Error($"GET USER EXCEPTION:: {ex?.ToString()}");
                retResponse = ex?.Message;
            }
            return await Task.FromResult(new Response<ApplicationUserDto>(succeeded: false, message: $"Error:: {retResponse}", data: null));
        }

        public async Task<Response<string>> UpdateUserStatus(UserStatusDTO userStatusDTO)
        {
            try
            {
                if (userStatusDTO == null || string.IsNullOrEmpty(userStatusDTO.UserId) || userStatusDTO.Status < 0)
                {
                    return null;
                }
                string userID = userStatusDTO.UserId;
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
                    return new Response<string>(succeeded: false, message: retResponse, code: (long)ApiResponseCodes.INVALID_REQUEST, data: null);
                }
                string oldUserName = user.UserName;

                if (userStatusDTO.Status.Equals(UserStatuses.Deleted))
                {
                    return await DeleteUser(new deleteUserDTO() { Id = user.Id });
                }
                user.Status = (int)(UserStatuses)userStatusDTO.Status;
                await _userManager.UpdateAsync(user);
                return new Response<string>(succeeded: true, message: $"User Updated Successfully!", code: (long)ApiResponseCodes.OK, data: null);
            }
            catch (Exception ex)
            {
                return new Response<string>(succeeded: false, message: ex?.Message, code: (long)ApiResponseCodes.EXCEPTION, data: null);
            }
        }

        public async Task<Response<string>> DeleteUser(deleteUserDTO request)
        {
            try
            {
                if (request == null)
                {
                    return null;
                }
                string userID = request.Id;
                if (string.IsNullOrEmpty(userID))
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
                    return new Response<string>(succeeded: false, message: retResponse, code: (long)ApiResponseCodes.INVALID_REQUEST, data: null);
                }
                string oldUserName = user.UserName;
                string email = $"{user.Email}";
                string username = $"{user.UserName}";
                user.Email = $"{RandomString()}{email}{RandomString()}";
                user.UserName = $"{RandomString()}{username}{RandomString()}";
                user.Status = (int)UserStatuses.Deleted;
                await _userManager.UpdateAsync(user);
                return new Response<string>(succeeded: true, message: $"User ({oldUserName}) Deleted Successfully!", code: (long)ApiResponseCodes.OK, data: null);
            }
            catch (Exception ex)
            {
                return new Response<string>(succeeded: false, message: ex?.Message, code: (long)ApiResponseCodes.EXCEPTION, data: null);
            }
        }

        private string RandomString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[7];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }
}
