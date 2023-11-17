using System;
using CollectionSystem.Domain.Common;

namespace CollectionSystem.Domain.Entities
{
	public class UserSavingsGroup : AuditableBaseEntity
	{       
        public String UserID { get; set; }
        public long SavingsGroupID { get; set; }       
    }
}