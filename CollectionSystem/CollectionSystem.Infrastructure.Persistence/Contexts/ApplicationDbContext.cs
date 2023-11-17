using Microsoft.EntityFrameworkCore;
using CollectionSystem.Domain.Common;
using CollectionSystem.Domain.Entities;
using CollectionSystem.Application.Interfaces;


namespace CollectionSystem.Infrastructure.Persistence.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IDateTimeService _dateTime;
        private readonly IAuthenticatedUserService _authenticatedUser;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeService dateTime, IAuthenticatedUserService authenticatedUser) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _dateTime = dateTime;
            _authenticatedUser = authenticatedUser;
        }

        public DbSet<SavingsGroup> SavingsGroups { get; set; }
        public DbSet<UserSavingsGroup> UserSavingsGroups { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                try
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.Created = _dateTime.NowUtc;
                            entry.Entity.CreatedBy = _authenticatedUser?.UserId;
                            break;
                        case EntityState.Modified:
                            entry.Entity.LastModified = _dateTime.NowUtc;
                            entry.Entity.LastModifiedBy = _authenticatedUser?.UserId;
                            break;
                    }
                }
                catch(Exception ex)
                {
                    
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //All Decimals will have 18,6 Range
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,6)");
            }
            base.OnModelCreating(builder);

            //builder.Entity<Currency>().ToTable("Currency");
            //builder.Entity<Currency>().HasData(
            //    new Currency { Id = 1, Country = "NIGERIA", Curr = "NGN", Code = "001" },
            //    new Currency { Id = 2, Country = "AMERICA", Curr = "USD", Code = "002" },
            //    new Currency { Id = 3, Country = "GREAT BRITAIN", Curr = "GBP", Code = "003" },
            //    new Currency { Id = 4, Country = "CANADA", Curr = "CAD", Code = "004" },
            //    new Currency { Id = 5, Country = "GHANA", Curr = "GHS", Code = "005" });
        }
    }
}
