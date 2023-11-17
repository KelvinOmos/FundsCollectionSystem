using CollectionSystem.Application.Enums;
using CollectionSystem.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionSystem.Infrastructure.Identity.Seeds
{
    public static class DefaultSuperAdmin
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            //Seed Default User
            try
            {
                var defaultUser = new ApplicationUser
                {
                    UserName = "superadmin",
                    Email = "superadmin@savings.com",
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,                    
                    Status = (int)UserStatuses.Active
                };
                if (userManager.Users.All(u => u.Id != defaultUser.Id))
                {
                    var user = userManager.FindByEmailAsync(defaultUser.Email).Result;
                    if (user == null)
                    {
                        var r = userManager.CreateAsync(defaultUser, "Password@2023").Result;
                        user = userManager.FindByEmailAsync(defaultUser.Email).Result;
                        r = userManager.AddToRoleAsync(user, Roles.SuperAdmin.ToString()).Result;
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
