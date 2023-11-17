using System;
using System.ComponentModel.DataAnnotations;
using CollectionSystem.Domain.Common;

namespace CollectionSystem.Application.DTO.SavingsGroup
{
    public class NewSavingsGroupDTO
    {
        [Required] public String GroupName { get; set; }
        public String? Description { get; set; }
        [Required] public float Amount { get; set; }
        [Required] public int MaximumCapacity { get; set; }        
        public string? GroupAdminUserId { get; set; }
    }
    public class UpdateSavingsGroupDTO
    {
        public long Id { get; set; }
        [Required] public String GroupName { get; set; }
        public String Description { get; set; }
        [Required] public float Amount { get; set; }
        [Required] public int MaximumCapacity { get; set; }        
    }
    public class RemoveSavingsGroupDTO
    {
        public long Id { get; set; }
    }
    public class GetSavingsGroupByIdDTO
    {
        public long Id { get; set; }
    }
    public class GetSavingsGroupMemberDTO
    {
        public long GroupId { get; set; }
    }    
    public class JoinSavingsGroupDTO
    {
        public long SavingsGroupId { get; set; }
        public string UserId { get; set; }
    }
    public class GetUserSavingsGroupDTO
    {        
        public string UserId { get; set; }
    }
    public class SavingsGroupDTO : AuditableBaseEntity
	{       
        public String GroupName { get; set; }
        public String Description { get; set; }
        public float Amount { get; set; }
        public int MaximumCapacity { get; set; }        
        public string? GroupAdminUserId { get; set; }
    }
}

