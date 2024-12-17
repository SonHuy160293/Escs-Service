using Core.Infrastructure;
using Core.Infrastructure.Middlewares;
using Elastic.Clients.Elasticsearch;
using Identity.Cache.Interfaces;
using Identity.Cache.Services;
using Logs.API.Interfaces;
using Logs.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Cấu hình ElasticsearchClient
builder.Services.AddSingleton<ElasticsearchClient>(sp =>
{
    var settings = new ElasticsearchClientSettings(new Uri("http://8.218.77.62:9200/"))
        .DefaultIndex("application-logs-emails-api-development-*"); // Thay đổi theo tên index của bạn

    var client = new ElasticsearchClient(settings);
    return client;
});

builder.Services.AddCoreInfrastructure(opt =>
{

    // Distributed Cache
    opt.EnableDistributedCache = true;
    opt.DistributedCacheOptions = new Core.Infrastructure.DependencyModels.DistributedCacheOptions
    {
        Database = Convert.ToInt32(builder.Configuration["DistributedCache:Database"]),
        Endpoints = builder.Configuration["DistributedCache:Endpoints"],
        Password = builder.Configuration["DistributedCache:Password"]
    };





    //// Authentication
    //opt.EnableAuthentication = true;
    //opt.TokenOptions = configuration.GetSection("TokenOptions");

});

builder.Services.AddScoped<ICachedEndpointUserRepository, CachedEndpointUserRepository>();

builder.Services.AddScoped<ILogService, LogService>();

builder.Services.AddTransient<IIdentityGrpcService, IdentityGrpcService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
