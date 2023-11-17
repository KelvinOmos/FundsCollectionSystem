using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CollectionSystem.Application.DTOs.Account;
using CollectionSystem.Application.Wrappers;

namespace CollectionSystem.Service.Application.Interfaces.Identity
{
    public interface IRoleService
    {
        Task<Response<bool>> CreateRole(CreateRoleDTO roleDTO);
        Task<Response<bool>> UpdateRole(UpdateRoleDTO roleDTO);
        Task<Response<bool>> DeleteRole(DeleteRoleDTO roleDTO);
        Task<Response<List<ApplicationRoleDto>>> GetRoles();
        Task<Response<ApplicationRoleDto>> GetRoleByName(string roleName);
        Task<Response<ApplicationRoleDto>> GetRoleById(UpdateRoleDTO request);
    }
}
