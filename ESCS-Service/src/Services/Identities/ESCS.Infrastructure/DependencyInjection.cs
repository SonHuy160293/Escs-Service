using Core.Infrastructure;
using ESCS.Application.Services;
using ESCS.Domain.Interfaces;
using ESCS.Infrastructure.Persistences;
using ESCS.Infrastructure.Services;
using Identity.Cache.Interfaces;
using Identity.Cache.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserEmailConfiguration.Cache.Interfaces;
using UserEmailConfiguration.Cache.Services;

namespace ESCS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            var connectionString = configuration.GetConnectionString("OracleConnection");


            services.AddDbContext<EscsDbContext>((sp, options) =>
            {
                options.UseOracle(connectionString);
            });



            services.AddCoreInfrastructure(opt =>
            {

                // Http Client
                opt.EnableHttpClient = true;

                //redis cache
                opt.EnableDistributedCache = true;
                opt.DistributedCacheOptions = new Core.Infrastructure.DependencyModels.DistributedCacheOptions
                {
                    Database = Convert.ToInt32(configuration["DistributedCache:Database"]),
                    Endpoints = configuration["DistributedCache:Endpoints"],
                    Password = configuration["DistributedCache:Password"]
                };

                // Authentication
                opt.EnableAuthentication = true;
                opt.TokenOptions = configuration.GetSection("Jwt");

            });
            services.AddScoped<ICachedUserEmailConfigRepository, CachedUserEmailConfigRepository>();
            services.AddScoped<ICachedEndpointUserRepository, CachedEndpointUserRepository>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserApiKeyRepository, UserApiKeyRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IUserEmailServiceConfigurationRepository, UserEmailServiceConfigurationRepository>();
            services.AddScoped<IServiceEndpointRepository, ServiceEndpointRepository>();
            services.AddScoped<IKeyAllowedEndpointRepository, KeyAllowedEndpointRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<ITokenService, TokenService>();



            services.AddScoped<IUnitOfWork, UnitOfWork>();



            return services;
        }

        public static IApplicationBuilder UseApplication(this IApplicationBuilder builder)
        {
            builder.UseCoreInfrastructure();

            return builder;
        }
    }
}

