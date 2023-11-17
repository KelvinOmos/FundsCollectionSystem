using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CollectionSystem.Application.DTOs.Account;
using CollectionSystem.Application.Wrappers;

namespace CollectionSystem.Service.Application.Interfaces.Identity
{
    public interface IUserService
    {
        Task<Response<List<ApplicationUserDto>>> GetAllUserAsync();
        Task<Response<ApplicationUserDto>> GetUserByIdAsync(viewUserDTO request);
        Task<Response<string>> UpdateUserStatus(UserStatusDTO userStatusDTO);
        Task<Response<string>> DeleteUser(deleteUserDTO request);
        Task<Response<bool>> EnrollAsync(RegisterRequest request, string origin);
    }
}
