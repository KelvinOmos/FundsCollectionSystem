using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using CollectionSystem.Application.DTOs.Account;

namespace CollectionSystem.Infrastructure.Identity.Mappings
{
    public class AppMapper: Profile
    {
        public AppMapper()
        {
            CreateMap<IdentityRole, ApplicationRoleDto>();
            CreateMap<IdentityUserRole<string>, ApplicationUserRoleDto>();
        }
    }
}
