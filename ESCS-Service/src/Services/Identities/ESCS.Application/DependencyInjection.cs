using Core.Application;
using ESCS.Application.Extensions;
using ESCS.Application.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ESCS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ApiKeyAuthorizationFilter>();

            ArgumentNullException.ThrowIfNull(services, nameof(services));

            services.AddCoreApplication(Assembly.GetExecutingAssembly());

            TokenExtension.Initialize(configuration);

            return services;
        }


    }
}
