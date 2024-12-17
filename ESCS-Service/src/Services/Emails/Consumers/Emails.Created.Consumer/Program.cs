using Core.Infrastructure.Dependencies;
using Emails.Application.Constants;
using Emails.Application.Services;
using Emails.Created.Consumer.Consume;
using Emails.Infrastructure;
using Emails.Infrastructure.Services;
using MassTransit;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog(SerilogElasticDependencies.Configure);


builder.Services.AddInfrastructure(builder.Configuration, Assembly.GetExecutingAssembly(), AppContext.BaseDirectory, x =>
{
    x.Host = builder.Configuration["MessageBroker:Host"];
    x.UserName = builder.Configuration["MessageBroker:UserName"];
    x.Password = builder.Configuration["MessageBroker:Password"];
    x.Consumers = (cfg) =>
    {
        cfg.AddConsumer<EmailCreatedConsumer>();

        return cfg;
    };
    x.Endpoints = (context, cfg) =>
    {
        cfg.ReceiveEndpoint(RabbitMQConstant.EmailCreatedQueueName, e =>
        {
            //e.PrefetchCount = 1; // Controls message prefetching for faster processing
            //e.UseConcurrencyLimit(1); // Allows up to 3 concurrent message is processed


            ////retry when exception occurred for 6 times after 10 second delay
            //e.UseMessageRetry(r => r.Interval(6, TimeSpan.FromMilliseconds(10000)));

            ////2nd level retry
            ////remove message from queue and redeiverd to queue at a futuretime
            //e.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));
            // Add the CorrelationIdMiddleware
            //e.UseConsumeFilter(typeof(CorrelationIdMiddleware<>), context);

            e.ConfigureConsumer<EmailCreatedConsumer>(context);
        });
    };

});


builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");

app.Run();
