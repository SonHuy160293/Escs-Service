using Core.Application.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreApplication(this IServiceCollection services, Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));

            services.AddHttpContextAccessor();

            CorrelationIdProvider.Configure(services.BuildServiceProvider().GetService<IHttpContextAccessor>());
            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(assembly);
            });

            services.AddAutoMapper(assembly);


            return services;
        }


    }
}
