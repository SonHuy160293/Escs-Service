using Core.Infrastructure.Dependencies;
using Core.Infrastructure.DependencyModels;
using Core.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services, Action<DependencyOptions> options)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));
            ArgumentNullException.ThrowIfNull(options, nameof(options));




            services.AddOptions<DependencyOptions>().Configure(options);

            var dependencyOptions = services.BuildServiceProvider().GetService<IOptions<DependencyOptions>>();



            if (dependencyOptions.Value.EnableDistributedCache)
                services.AddDistributedCache(dependencyOptions.Value.DistributedCacheOptions);



            if (dependencyOptions.Value.EnableHttpClient)
                services.AddCustomHttpClient();

            if (dependencyOptions.Value.EnableMessageBus)
                services.AddMessageBus(dependencyOptions.Value.MessageBusOptions);


            if (dependencyOptions.Value.EnableAuthentication)
            {
                services.Configure<TokenOptions>(dependencyOptions.Value.TokenOptions);
                services.AddCustomAuthentication();
            }

            return services;
        }

        public static IApplicationBuilder UseCoreInfrastructure(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<CorrelationMiddleware>();

            builder.UseMiddleware<HttpContextLoggingMiddleware>();

            //builder.UseMiddleware<ResponseLoggingMiddleware>();

            builder.UseMiddleware<ExceptionHandlingMiddleware>();

            //builder.UseMiddleware<RequestLoggingMiddleware>();

            //builder.UseMiddleware<CorrelationMiddleware>(); 

            return builder;
        }


    }
}