using System;
using System.Collections.Generic;
using System.Text;

namespace CollectionSystem.Domain.Settings
{
    public class JWTSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int DurationInMinutes { get; set; }
        public string SecretKey { get; set; }
    }
}