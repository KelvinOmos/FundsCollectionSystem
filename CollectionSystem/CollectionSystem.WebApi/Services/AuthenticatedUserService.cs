using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CollectionSystem.Application.Interfaces;

namespace CollectionSystem.Services
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");
                Role = httpContextAccessor.HttpContext?.User?.FindFirstValue("role");                
            }
            catch(Exception ex)
            {

            }
           
        }

        public string UserId { get; }
        public string Role { get; }
    }
}
