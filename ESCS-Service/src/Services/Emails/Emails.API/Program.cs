using Core.Application.Services;
using Core.Infrastructure.Dependencies;
using Core.Infrastructure.Services;
using Emails.API.Filters;
using Emails.Application;
using Emails.Infrastructure;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog(SerilogElasticDependencies.Configure);

builder.Services.AddScoped<ValidateSendMailRequestFilter>();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration, Assembly.GetExecutingAssembly(), AppContext.BaseDirectory, x =>
{
    x.Host = builder.Configuration["MessageBroker:Host"];
    x.UserName = builder.Configuration["MessageBroker:UserName"];
    x.Password = builder.Configuration["MessageBroker:Password"];
    x.Consumers = (cfg) =>
    {


        return cfg;
    };
    x.Endpoints = (context, cfg) =>
    {

    };

});

builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseApplication();

app.UseAuthorization();

app.MapControllers();

app.Run();
