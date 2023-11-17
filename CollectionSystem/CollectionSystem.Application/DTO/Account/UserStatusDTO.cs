using System;
using System.Collections.Generic;
using System.Text;
using CollectionSystem.Application.Enums;

namespace CollectionSystem.Application.DTOs.Account
{
    public class UserStatusDTO
    {
        public string UserId { get; set; }
        public UserStatuses Status { get; set; }
        public string? ReasonForChange { get; set; }
    }
}
