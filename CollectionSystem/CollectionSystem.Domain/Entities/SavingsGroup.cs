using System;
using CollectionSystem.Domain.Common;

namespace CollectionSystem.Domain.Entities
{
	public class SavingsGroup : AuditableBaseEntity
	{       
        public String GroupName { get; set; }
        public String Description { get; set; }
        public float Amount { get; set; }
        public int MaximumCapacity { get; set; }        
        public string? GroupAdminUserId { get; set; }
    }
}