using System;
using CollectionSystem.Application.DTO.SavingsGroup;
using CollectionSystem.Application.DTOs.Account;
using CollectionSystem.Application.Interfaces;
using CollectionSystem.Application.Interfaces.SavingsGroup;
using CollectionSystem.Application.Wrappers;
using CollectionSystem.Domain.Entities;
using CollectionSystem.Infrastructure.Identity.Contexts;
using CollectionSystem.Infrastructure.Identity.Models;
using CollectionSystem.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CollectionSystem.Infrastructure.Shared.Services
{
    public class SavingsGroupService : ISavingsGroupService
    {
        private readonly ApplicationDbContext _db;
        private readonly IdentityContext _identityContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticatedUserService _authuser;
        private readonly IConfiguration _config;

        public SavingsGroupService(ApplicationDbContext db, IAuthenticatedUserService authUser, IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _authuser = authUser;
            _config = config;
            _userManager = userManager;
        }

        public async Task<Response<bool>> CreateSavingsGroup(NewSavingsGroupDTO request)
        {
            Log.Information("WELCOME TO CREATE SAVINGS GROUP SERVICE");
            var retStatus = false;
            var retResponse = "";
            DateTime presentDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "W. Central Africa Standard Time");
            try
            {
                #region VALIDATE USER
                if (string.IsNullOrEmpty(_authuser.UserId))
                    return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = "User ID cannot be null" });
                #endregion
                #region VALIDATE ENTRY
                if (string.IsNullOrWhiteSpace(request.GroupName))
                    return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = "Enter Savings Group Name" });
                if (string.IsNullOrWhiteSpace(request.Description))
                    return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = "Enter group description" });
                if (request.MaximumCapacity <= 0)
                    return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = "Enter maximum capacity" });
                if (request.Amount <= 0.0)
                    return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = "Enter Amount" });
                var existingUserSavingsGroup = (from v in _db.UserSavingsGroups where v.UserID == request.GroupAdminUserId select v);
                if (existingUserSavingsGroup.Any())
                    return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = $"User belongs to a group already!" });

                #endregion
                #region SAVE ENTRY TO DATABASE
                var incomingData = new SavingsGroup()
                {
                    GroupName = request.GroupName,
                    Description = request.Description,
                    Amount = request.Amount,
                    MaximumCapacity = request.MaximumCapacity,
                    GroupAdminUserId = _authuser.UserId
                };

                var user = await _userManager.FindByIdAsync(_authuser.UserId);
                if (user == null)
                {
                    return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = $"User does not exist" });
                }

                using (var TranX = _db.Database.BeginTransaction())
                {
                    await _db.SavingsGroups.AddAsync(incomingData);
                    await _db.SaveChangesAsync();

                    var groupId = _db.SavingsGroups.FirstOrDefault(sg => sg.GroupName == incomingData.GroupName).Id;
                    var userSavingsGroup = new UserSavingsGroup()
                    {
                        SavingsGroupID = groupId,
                        UserID = _authuser.UserId,
                    };

                    await _db.UserSavingsGroups.AddAsync(userSavingsGroup);
                    await _db.SaveChangesAsync();

                    user.OccupiedGroup = true;
                    var result = _userManager.UpdateAsync(user);

                    await TranX.CommitAsync();
                }

                retStatus = true;
                retResponse = "Sucessful";
                Log.Information($"NEW SAVINGS GROUP ({request.GroupName}) SAVED SUCCESSFUL");
                return await Task.FromResult(new Response<bool>() { Succeeded = true, Message = retResponse, Code = (long)ApiResponseCodes.OK });
                #endregion
            }
            catch (Exception eex)
            {
                Log.Information($"NEW SAVINGS GROUP EXCEPTION:: {eex?.ToString()}");
                retResponse = (eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message;
                return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = $"{retResponse}" });
            }
        }

        public async Task<Response<SavingsGroup>> GetSavingsGroupById(GetSavingsGroupByIdDTO request)
        {
            Log.Information("WELCOME TO FETCH SAVINGS GROUP BY ID SERVICE");
            var retStatus = false;
            var retResponse = "";
            DateTime presentDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "W. Central Africa Standard Time");
            try
            {
                #region VALIDATE USER
                if (string.IsNullOrEmpty(_authuser.UserId))
                    return await Task.FromResult(new Response<SavingsGroup>() { Succeeded = false, Message = "User ID cannot be null" });
                #endregion
                #region VALIDATE ENTRY
                if (request == null || string.IsNullOrWhiteSpace(Convert.ToString(request.Id)))
                    return await Task.FromResult(new Response<SavingsGroup>() { Succeeded = false, Message = "ID Cannot be null" });
                #endregion
                #region FETCH ENTRIES TO DATABASE
                var savingsGroup = (from v in _db.SavingsGroups where v.Id == request.Id select v).FirstOrDefault();
                if (savingsGroup == null)
                    return await Task.FromResult(new Response<SavingsGroup>() { Succeeded = false, Message = $"SAVINGS GROUP Not Found for ID: {request.Id}" });

                retStatus = true;
                retResponse = "Successful";
                Log.Information($"SUCCESSFUL");
                return await Task.FromResult(new Response<SavingsGroup>() { Succeeded = true, Message = retResponse, Data = savingsGroup });
                #endregion
            }
            catch (Exception eex)
            {
                Log.Information($"SAVINGSGROUPBYID EXCEPTION:: {eex?.ToString()}");
                retResponse = (eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message;
                return await Task.FromResult(new Response<SavingsGroup>() { Succeeded = false, Message = $"{retResponse}" });
            }
        }

        public async Task<Response<List<ApplicationUserDto>>> GetSavingsGroupMembers(GetSavingsGroupMemberDTO request)
        {
            Log.Information("WELCOME TO FETCH SAVINGS GROUP MEMBERS SERVICE");
            var retStatus = false;
            var retResponse = "";
            DateTime presentDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "W. Central Africa Standard Time");
            try
            {
                #region VALIDATE USER
                if (string.IsNullOrEmpty(_authuser.UserId))
                    return await Task.FromResult(new Response<List<ApplicationUserDto>>() { Succeeded = false, Message = "User ID cannot be null" });
                #endregion
                #region VALIDATE ENTRY
                if (request.GroupId < 0)
                    return await Task.FromResult(new Response<List<ApplicationUserDto>>() { Succeeded = false, Message = "Group ID Cannot be null" });
                #endregion
                #region FETCH ENTRIES TO DATABASE
                var userSavingsGroup = (from v in _db.UserSavingsGroups where v.SavingsGroupID == request.GroupId select v);
                if (userSavingsGroup == null)
                    return await Task.FromResult(new Response<List<ApplicationUserDto>>() { Succeeded = false, Message = $"SAVINGS GROUP Not Found for ID: {request.GroupId}" });

                var users = _userManager.Users.ToList();
                List<ApplicationUserDto> result = new List<ApplicationUserDto>();
                foreach(var userSavingGroup in userSavingsGroup)
                {
                    var user = users.FirstOrDefault(u => u.Id == userSavingGroup.UserID);
                    result.Add(new ApplicationUserDto()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        TotalAmountSaved = user.TotalAmountSaved
                    });
                }
                retStatus = true;
                retResponse = "Successful";
                Log.Information($"SUCCESSFUL");
                return await Task.FromResult(new Response<List<ApplicationUserDto>>() { Succeeded = true, Message = retResponse, Data = result });
                #endregion
            }
            catch (Exception eex)
            {
                Log.Information($"SAVINGSGROUPMEMBER EXCEPTION:: {eex?.ToString()}");
                retResponse = (eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message;
                return await Task.FromResult(new Response<List<ApplicationUserDto>>() { Succeeded = false, Message = $"{retResponse}" });
            }
        }

        public async Task<Response<List<SavingsGroup>>> GetSavingsGroups()
        {
            Log.Information("WELCOME TO FETCH SAVINGS GROUP LIST SERVICE");
            var retStatus = false;
            var retResponse = "";
            DateTime presentDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "W. Central Africa Standard Time");
            try
            {
                #region VALIDATE USER
                if (string.IsNullOrEmpty(_authuser.UserId))
                    return await Task.FromResult(new Response<List<SavingsGroup>>() { Succeeded = false, Message = "User ID cannot be null" });
                #endregion
                #region FETCH ENTRIES TO DATABASE
                var savingsGroups = (from v in _db.SavingsGroups select v).ToList();
                retStatus = true;
                retResponse = "Successful";
                Log.Information($"SUCCESSFUL with ({savingsGroups.Count()}) saving groups");
                return await Task.FromResult(new Response<List<SavingsGroup>>() { Succeeded = true, Message = retResponse, Data = savingsGroups,Code = (long)ApiResponseCodes.OK,Description = retResponse });
                #endregion
            }
            catch (Exception eex)
            {
                Log.Information($"FETCH SAVINGS GROUPS EXCEPTION:: {eex?.ToString()}");
                retResponse = (eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message;
                return await Task.FromResult(new Response<List<SavingsGroup>>() { Succeeded = false, Message = $"{retResponse}" });
            }
        }

        public async Task<Response<SavingsGroup>> GetUserSavingsGroup(GetUserSavingsGroupDTO request)
        {
            Log.Information("WELCOME TO FETCH USER SAVINGS GROUP BY USER ID SERVICE");
            var retStatus = false;
            var retResponse = "";
            DateTime presentDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "W. Central Africa Standard Time");
            try
            {
                #region VALIDATE USER
                if (string.IsNullOrEmpty(_authuser.UserId))
                    return await Task.FromResult(new Response<SavingsGroup>() { Succeeded = false, Message = "User ID cannot be null" });
                #endregion
                #region VALIDATE ENTRY
                if (request == null || string.IsNullOrWhiteSpace(Convert.ToString(request.UserId)))
                    return await Task.FromResult(new Response<SavingsGroup>() { Succeeded = false, Message = "User ID Cannot be null" });
                #endregion
                #region FETCH ENTRIES TO DATABASE
                var userSavingsGroup = (from v in _db.UserSavingsGroups where v.UserID == request.UserId select v).FirstOrDefault();
                if (userSavingsGroup == null)
                    return await Task.FromResult(new Response<SavingsGroup>() { Succeeded = false, Message = $"User does not exist or does not belong to a group" });

                var savingsGroup = (from v in _db.SavingsGroups where v.Id == userSavingsGroup.SavingsGroupID select v).FirstOrDefault();
                if (savingsGroup == null)
                {
                    return await Task.FromResult(new Response<SavingsGroup>() { Succeeded = false, Message = $"Group with ID ({userSavingsGroup.SavingsGroupID}) does not exist" });
                }
                retStatus = true;
                retResponse = "Successful";
                Log.Information($"SUCCESSFUL");
                return await Task.FromResult(new Response<SavingsGroup>() { Succeeded = true, Message = retResponse, Data = savingsGroup, Code = (long)ApiResponseCodes.OK });
                #endregion
            }
            catch (Exception eex)
            {
                Log.Information($"USERSAVINGSGROUPBYID EXCEPTION:: {eex?.ToString()}");
                retResponse = (eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message;
                return await Task.FromResult(new Response<SavingsGroup>() { Succeeded = false, Message = $"{retResponse}" });
            }
        }

        public async Task<Response<bool>> JoinSavingsGroup(JoinSavingsGroupDTO request)
        {
            Log.Information("WELCOME TO JOIN SAVINGS GROUP BY USER ID SERVICE");
            var retStatus = false;
            var retResponse = "";
            DateTime presentDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "W. Central Africa Standard Time");
            try
            {
                #region VALIDATE USER
                if (string.IsNullOrEmpty(_authuser.UserId))
                    return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = "User ID cannot be null" });
                #endregion
                #region VALIDATE ENTRY
                if (request == null || string.IsNullOrWhiteSpace(Convert.ToString(request.UserId)))
                    return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = "User ID Cannot be null" });
                #endregion
                #region FETCH ENTRIES TO DATABASE
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = $"User does not exist" });
                }
                var userSavingsGroup = (from v in _db.UserSavingsGroups where v.UserID == request.UserId select v);
                if (userSavingsGroup.Any())
                    return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = $"User belongs to a group already!" });

                var savingsGroup = (from v in _db.SavingsGroups where v.Id == request.SavingsGroupId select v).ToList();
                if (!savingsGroup.Any())
                {
                    return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = $"Group with ID ({request.SavingsGroupId}) does not exist" });
                }
                if (savingsGroup.FirstOrDefault().MaximumCapacity == savingsGroup.Count)
                {
                    return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = $"Maximum number of members reached" });
                }
                var userSavingsGroupData = new UserSavingsGroup()
                {
                    SavingsGroupID = request.SavingsGroupId,
                    UserID = request.UserId
                };

                using (var TranX = _db.Database.BeginTransaction())
                {
                    await _db.UserSavingsGroups.AddAsync(userSavingsGroupData);
                    await _db.SaveChangesAsync();

                    user.OccupiedGroup = true;
                    var result = _userManager.UpdateAsync(user);

                    await TranX.CommitAsync();
                }
                retStatus = true;
                retResponse = "Successful";
                Log.Information($"SUCCESSFUL");
                return await Task.FromResult(new Response<bool>() { Succeeded = true, Message = retResponse, Data = true });
                #endregion
            }
            catch (Exception eex)
            {
                Log.Information($"USERSAVINGSGROUPBYID EXCEPTION:: {eex?.ToString()}");
                retResponse = (eex?.InnerException != null) ? eex?.InnerException.Message : eex?.Message;
                return await Task.FromResult(new Response<bool>() { Succeeded = false, Message = $"{retResponse}" });
            }
        }
    }
}

