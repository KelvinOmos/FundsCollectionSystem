using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CollectionSystem.Application.Interfaces;
using CollectionSystem.Infrastructure.Persistence.Repository;
using CollectionSystem.Infrastructure.Persistence.Contexts;

namespace CollectionSystem.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                   configuration.GetConnectionString("DefaultConnection")
            );
                //options.EnableSensitiveDataLogging();
            });

            #region Repositories
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));            
            #endregion
        }
    }
}
