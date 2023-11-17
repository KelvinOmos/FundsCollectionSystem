using System;
using System.Collections.Generic;
using System.Text;

namespace CollectionSystem.Application.DTOs.Account
{
    public class UserRolesDTO
    {
        public string UserId { get; set; }
        public List<string> RoleIds { get; set; }        
        public List<string> RoleNames { get; set; }
        public List<string> OldRoleNamesVM { get; set; }
        public string OldRoleNames { get; set; }
        public string OldPermissionNames { get; set; }        
        public ApplicationUserDto userData { get; set; }
    }
}
