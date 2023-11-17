using System;
using System.Collections.Generic;
using System.Text;

namespace CollectionSystem.Application.Interfaces
{
    public interface IAuthenticatedUserService
    {
        string UserId { get; }
        string Role { get; }
    }
}
