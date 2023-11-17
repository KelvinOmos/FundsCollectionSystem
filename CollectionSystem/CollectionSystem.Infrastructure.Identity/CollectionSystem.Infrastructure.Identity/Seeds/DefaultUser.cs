using CollectionSystem.Application.Enums;
using CollectionSystem.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionSystem.Infrastructure.Identity.Seeds
{
    public static class DefaultUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            //Seed Default User
            try
            {
                var defaultUser = new ApplicationUser
                {
                    UserName = "initiator",
                    Email = "initiator@twig.com",
                    FirstName = "Initiator",
                    LastName = "Initiator",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                if (userManager.Users.All(u => u.Id != defaultUser.Id))
                {
                    var user = userManager.FindByEmailAsync(defaultUser.Email).Result;
                    if (user == null)
                    {
                        var r = userManager.CreateAsync(defaultUser, "Password@2023").Result;
                        user = userManager.FindByEmailAsync(defaultUser.Email).Result;
                        r = userManager.AddToRoleAsync(user, Roles.User.ToString()).Result;
                    }
                }
            }
            catch
            {

            }
        }
    }
}