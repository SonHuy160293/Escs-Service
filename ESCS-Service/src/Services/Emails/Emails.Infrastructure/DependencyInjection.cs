using Core.Infrastructure;
using Core.Infrastructure.Dependencies;
using Core.Infrastructure.DependencyModels;
using Emails.Application.Services;
using Emails.Domain.Interfaces;
using Emails.Infrastructure.Middlewares;
using Emails.Infrastructure.Persistences;
using Emails.Infrastructure.Services;
using ESCS.Grpc.Protos;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using UserEmailConfiguration.Cache.Interfaces;
using UserEmailConfiguration.Cache.Services;

namespace Emails.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration,
            Assembly executingAssembly,
            string baseDirectory,
            Action<MessageBusOptions> messageBusOptions = null)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));

            services.AddGrpcClient<UserProtoService.UserProtoServiceClient>(o =>
            {
                o.Address = new Uri("http://localhost:5026"); // gRPC server address
            });

            services.AddMongoDb(configuration);
            services.Configure<MongoOptions>(configuration.GetSection("MongoSettings"));

            services.AddCoreInfrastructure(opt =>
            {

                // Distributed Cache
                opt.EnableDistributedCache = true;
                opt.DistributedCacheOptions = new Core.Infrastructure.DependencyModels.DistributedCacheOptions
                {
                    Database = Convert.ToInt32(configuration["DistributedCache:Database"]),
                    Endpoints = configuration["DistributedCache:Endpoints"],
                    Password = configuration["DistributedCache:Password"]
                };

                opt.EnableHttpClient = true;

                // Message Broker - Bus
                if (messageBusOptions != null)
                {
                    opt.EnableMessageBus = true;

                    services.AddOptions<MessageBusOptions>().Configure(messageBusOptions);

                    var messageBusOpt = services.BuildServiceProvider().GetService<IOptions<MessageBusOptions>>();

                    opt.MessageBusOptions = messageBusOpt.Value;
                }



                //// Authentication
                //opt.EnableAuthentication = true;
                //opt.TokenOptions = configuration.GetSection("TokenOptions");

            });


            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IEmailExceptionRepository, EmailExceptionRepository>();
            services.AddScoped<ICachedUserEmailConfigRepository, CachedUserEmailConfigRepository>();

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IIdentityGrpcService, IdentityGrpcService>();

            return services;
        }

        public static IApplicationBuilder UseApplication(this IApplicationBuilder builder)
        {

            builder.UseMiddleware<AuthorizedKeyMiddleware>();

            builder.UseCoreInfrastructure();

            return builder;
        }


    }
}
