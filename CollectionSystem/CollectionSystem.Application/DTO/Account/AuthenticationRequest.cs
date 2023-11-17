using System;
using System.Collections.Generic;
using System.Text;

namespace CollectionSystem.Application.DTOs.Account
{
    public class AuthenticationRequest
    {
        public string UserID { get; set; }
        public string Password { get; set; }
    }
}
