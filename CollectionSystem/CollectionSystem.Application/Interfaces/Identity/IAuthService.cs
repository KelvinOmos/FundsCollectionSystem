using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CollectionSystem.Application.DTOs.Account;
using CollectionSystem.Application.Wrappers;

namespace CollectionSystem.Service.Application.Interfaces.Identity
{
    public interface IAuthService
    {
        Task<Response<AuthenticationResponse>> Token(AuthenticationRequest request);
    }
}
