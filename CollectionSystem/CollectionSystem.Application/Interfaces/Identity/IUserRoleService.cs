using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CollectionSystem.Application.DTOs.Account;
using CollectionSystem.Application.Wrappers;

namespace CollectionSystem.Service.Application.Interfaces.Identity
{
    public interface IUserRoleService
    {
        //Task<Response<List<ApplicationUserRoleDto>>> GetAllUserRoles(string userId);
        //Task<Response<string>> AddUserToRole(string userID, string role);

        Task<Response<bool>> AddUserRole(AddApplicationUserRoles rolePermissionDto);
        Task<Response<bool>> DeleteUserRole(DeleteApplicationUserRoleDto rolePermissionDto);
        Task<Response<bool>> DeleteBatchUserRole(DeleteBatchApplicationUserRoleDto rolePermissionDto);
        Task<Response<bool>> UpdateUserRole(UpdateApplicationUserRoleDto rolePermissionDto);
        Task<Response<List<ApplicationUserRoleDto>>> GetAllUserRoles();
        Task<Response<List<ApplicationUserRoleDto>>> GetUserRole(GetUserRolesDto userRoleDto);
    }
}
