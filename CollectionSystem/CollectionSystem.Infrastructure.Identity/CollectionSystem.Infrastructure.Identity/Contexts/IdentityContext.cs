using CollectionSystem.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CollectionSystem.Application.Interfaces;

namespace CollectionSystem.Infrastructure.Identity.Contexts
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        private IAuthenticatedUserService _authenticatedUser;
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
            //_authenticatedUser = authenticatedUser;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Identity");
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "User");
            });           

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            _authenticatedUser = (IAuthenticatedUserService)ServiceExtensions.ServiceProvider?.GetService(typeof(IAuthenticatedUserService));
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                try
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.Created = DateTime.Now;
                            entry.Entity.CreatedBy = _authenticatedUser?.UserId;
                            break;
                        case EntityState.Modified:
                            entry.Entity.LastModified = DateTime.Now;
                            entry.Entity.LastModifiedBy = _authenticatedUser?.UserId;
                            break;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
