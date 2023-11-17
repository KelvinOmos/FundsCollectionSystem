using CollectionSystem.Application.Interfaces;
using CollectionSystem.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CollectionSystem.Domain.Settings;
using CollectionSystem.Application.Interfaces.SavingsGroup;
using Microsoft.AspNetCore.Builder;

namespace CollectionSystem.Infrastructure.Shared
{
    public static class ServiceRegistration
    {
        public static void LoadConfigurations(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));
        }

        public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration _config)
        {
            services.Configure<MailSettings>(_config.GetSection("MailSettings"));
            services.AddTransient<IDateTimeService, DateTimeService>();           
            services.AddTransient<ISavingsGroupService, SavingsGroupService>();            
        }
    }
}