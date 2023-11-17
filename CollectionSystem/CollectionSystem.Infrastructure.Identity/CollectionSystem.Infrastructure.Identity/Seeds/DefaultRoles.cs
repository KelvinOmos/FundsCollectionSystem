using CollectionSystem.Application.Enums;
using CollectionSystem.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CollectionSystem.Infrastructure.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static void SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles            
            try
            {
                 var r = roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString())).Result;
                 r= roleManager.CreateAsync(new IdentityRole(Roles.User.ToString())).Result;
                 r=roleManager.CreateAsync(new IdentityRole(Roles.GroupAdmin.ToString())).Result;                 
            }
            catch(Exception ex)
            {

            }
        }
    }
}
