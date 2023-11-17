using System;
using CollectionSystem.Application.DTO.SavingsGroup;
using CollectionSystem.Application.DTOs.Account;
using CollectionSystem.Application.Wrappers;
using CollectionSystem.Domain.Entities;

namespace CollectionSystem.Application.Interfaces.SavingsGroup
{
	public interface ISavingsGroupService
	{
        Task<Response<bool>> CreateSavingsGroup(NewSavingsGroupDTO request);
        Task<Response<bool>> JoinSavingsGroup(JoinSavingsGroupDTO request);        
        Task<Response<List<Domain.Entities.SavingsGroup>>> GetSavingsGroups();
        Task<Response<List<ApplicationUserDto>>> GetSavingsGroupMembers(GetSavingsGroupMemberDTO request);
        Task<Response<Domain.Entities.SavingsGroup>> GetUserSavingsGroup(GetUserSavingsGroupDTO request);
        Task<Response<Domain.Entities.SavingsGroup>> GetSavingsGroupById(GetSavingsGroupByIdDTO request);
    }
}

