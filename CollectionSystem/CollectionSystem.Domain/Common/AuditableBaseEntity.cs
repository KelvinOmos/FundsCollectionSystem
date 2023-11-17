using System;
using System.Collections.Generic;
using System.Text;

namespace CollectionSystem.Domain.Common
{
    public abstract class AuditableBaseEntity
    {
        public virtual long Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
