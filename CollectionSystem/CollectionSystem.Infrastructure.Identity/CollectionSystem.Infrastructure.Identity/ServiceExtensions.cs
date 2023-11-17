using CollectionSystem.Application.Interfaces;
using CollectionSystem.Application.Wrappers;
using CollectionSystem.Domain.Settings;
using CollectionSystem.Infrastructure.Identity.Contexts;
using CollectionSystem.Infrastructure.Identity.Models;
using CollectionSystem.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Cryptography;
using CollectionSystem.Service.Application.Interfaces.Identity;
using CollectionSystem.Infrastructure.Identity.Seeds;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CollectionSystem.Infrastructure.Identity
{
    public static class ServiceExtensions
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("ServiceConnection"),
                    b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();
            #region Services
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IUserRoleService, UserRoleService>();
            services.AddTransient<IUserService, UserService>();
            #endregion


            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
            //var configuration = builder.Configuration;
            var key = RSA.Create();
            key.FromXmlString(configuration["JWTSettings:SecretKey"]);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JWTSettings:Issuer"],
                        ValidAudience = configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new RsaSecurityKey(key.ExportParameters(false))
                    };
                    o.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = context =>
                        {
                            context.NoResult();
                            if (!context.Response.HasStarted)
                                context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new Response<string>("Token validation failed, token expired", succeeded: false));
                            return context.Response.WriteAsync(result);

                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new Response<string>("You are not Authorized", succeeded: false));
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new Response<string>("You are not authorized to access this resource", succeeded: false));
                            return context.Response.WriteAsync(result);
                        },
                    };
                });


            //RUN SEEDS
            var provider = services.BuildServiceProvider();
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            try
            {
                var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
                //var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
                //DefaultRoles.SeedAsync(roleManager);
                DefaultSuperAdmin.SeedAsync(userManager);
                //DefaultUser.SeedAsync(userManager);

                //Log.Information("Finished Seeding Default Data");
                //Log.Information("Application Starting");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred seeding the DB");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            ServiceProvider = services.BuildServiceProvider();

        }
    }
}
