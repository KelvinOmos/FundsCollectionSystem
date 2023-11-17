using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using CollectionSystem.WebApp.Enums;

namespace CollectionSystem.WebApp.Models
{
    public class ApplicationRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string LastName { get; set; }
        public UserStatuses Status { get; set; }
        public string Role { get; set; }
        public bool Collector { get; set; }
        public float TotalAmountSaved { get; set; }
        public bool OccupiedGroup { get; set; }
    }

    public class AppUserDto
    {

        //public virtual UserDto User { get; set; }
        public string Name { get; set; }
        public ApiResponseCodes Code { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public List<string> roles { get; set; }
        public string firstname { get; set; }
        public string middleName { get; set; }        

        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }
        public string Gender { get; set; }
        public string ActivationCode { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Wssignature { get; set; }
        public string FullName
        {
            get
            {
                return $"{Name} {Surname}";
            }
        }

        public DateTime? LastLoginDate { get; set; }
        public bool Activated { get; set; }
        public bool IsBranchUser { get; set; }
        public string BranchId { get; set; }
        public UserStatuses Status { get; set; }           //0-Active, 1-Disabled, 2-Deleted
        public bool Is2FAEnabled { get; set; }
        public bool HasSetUp2FA { get; set; }
        public string LastModifier { get; set; }

        public string CreatorName { get; set; }

    }

    public class UserRequestDto
    {        
        public string Name { get; set; }
        public int TotalCount { get; set; }
        public int PendingCount { get; set; }        
    }

    public class CreateRoleDTO
    {
        public string? Name { get; set; }
    }
    public class UpdateRoleDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
    public class DeleteRoleDTO
    {
        public string? Name { get; set; }
    }
    public class AddApplicationUserRoles
    {
        public Guid UserId { get; set; }
        public List<string> RoleNames { get; set; }

        public SelectList? users { get; set; }
        public SelectList? roles { get; set; }
    }
    public class DeleteApplicationUserRoleDto
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
    public class DeleteBatchApplicationUserRoleDto
    {
        public Guid UserId
        {
            get; set;
        }
        public List<string> RoleNames
        {
            get; set;
        }
    }
    public class UpdateApplicationUserRoleDto
    {
        public Guid UserId
        {
            get; set;
        }
        public List<string> RoleNames
        {
            get; set;
        }
    }
    public class ApplicationUserRoleDto
    {
        public string Name { get; set; }
        public Guid UserId
        {
            get; set;
        }
        public Guid RoleId
        {
            get; set;
        }

        public string User { get; set; }
        public string Role { get; set; }
    }
    public class GetUserRolesDto
    {
        public Guid UserId { get; set; }
    }
    public class GetApplicationUserRoleDto
    {
        public Guid UserId
        {
            get; set;
        }
        public List<Guid> RoleIds
        {
            get; set;
        }
    }
    public class UserRoleDTO
    {
        public Guid Id { get; set; }
        public string User { get; set; }
        public string Roles { get; set; }
    }
}
