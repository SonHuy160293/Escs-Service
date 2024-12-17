using Core.Application.Services;
using Core.Infrastructure.DependencyModels;
using Core.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Dependencies
{
    public static class DistributedCacheDependencies
    {
        public static IServiceCollection AddDistributedCache(this IServiceCollection services, DistributedCacheOptions options)
        {
            services.AddStackExchangeRedisCache(setup =>
            {
                setup.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
                {
                    EndPoints = {
                        options.Endpoints
                    },
                    Password = options.Password,
                    DefaultDatabase = options.Database
                };
            });

            services.AddSingleton<ICacheProvider, CacheProvider>();

            return services;
        }
    }
}
