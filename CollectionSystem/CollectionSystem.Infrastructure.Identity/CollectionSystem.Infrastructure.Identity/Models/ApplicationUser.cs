using CollectionSystem.Application.DTOs.Account;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollectionSystem.Domain.Common;

namespace CollectionSystem.Infrastructure.Identity.Models
{
    public abstract class AuditableBaseEntity
    {
        [Key]
        public virtual Guid Id { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
    }

    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Status { get; set; }                
        public List<RefreshToken> RefreshTokens { get; set; }
        public bool OwnsToken(string token)
        {
            return this.RefreshTokens?.Find(x => x.Token == token) != null;
        }
        public bool Collector { get; set; }
        public float TotalAmountSaved { get; set; }
        public bool OccupiedGroup { get; set; }

    }
}