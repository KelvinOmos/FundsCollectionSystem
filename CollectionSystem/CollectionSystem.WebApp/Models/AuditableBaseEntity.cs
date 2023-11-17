﻿namespace CollectionSystem.WebApp.Models
{
    public abstract class AuditableBaseEntity
    {
        public virtual long Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
