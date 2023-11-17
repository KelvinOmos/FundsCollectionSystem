using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using CollectionSystem.WebApp.Enums;

namespace CollectionSystem.WebApp.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string? LastName { get; set; }
        public UserStatuses Status { get; set; }
        public string Role { get; set; }
    }    

    public class RoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }        
    }

    public class newUserDTO
    {
        [Required] public string FirstName { get; set; }
        //[Required] 
        public string? LastName { get; set; }
        [Required][EmailAddress] public string Email { get; set; }
        //[Required][MinLength(6)]
        public string? UserName { get; set; }
        [Required][MinLength(6)] public string Password { get; set; }
        [Required][Compare("Password")] public string ConfirmPassword { get; set; }
        [Required] public string Role { get; set; }               

        public SelectList? Roles { get; set; }       
    }

    public class viewUserDTO
    {
        public string Id { get; set; }
    }

    public class deleteUserDTO
    {
        public string Id { get; set; }
    }

    public class UserStatusDTO
    {
        public string UserId { get; set; }
        public UserStatuses Status { get; set; }
        public string? ReasonForChange { get; set; }
    }

    public class loginRequest
    {
        public string userID { get; set; }
        public string password { get; set; }
    }

    public class loginResponse
    {
        public bool succeeded { get; set; }
        public string responseCode { get; set; }
        public string message { get; set; }
        public string errors { get; set; }
        public loginResponseData data { get; set; }
    }

    public class loginResponseData
    {
        public string FullName { get; set; }
        public string Token { get; set; }
        public int MakerChecker { get; set; }
        public int Status { get; set; }
    }    
}
