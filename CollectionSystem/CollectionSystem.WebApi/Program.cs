using CollectionSystem.Application;
using CollectionSystem.Extensions;
using CollectionSystem.Infrastructure.Persistence;
//using CollectionSystem.Infrastructure.Service;
using CollectionSystem.Infrastructure.Shared;
using CollectionSystem.Services;
using Serilog;
using CollectionSystem.Application.Interfaces;
using CollectionSystem.Infrastructure.Identity;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddApplicationLayer();
builder.Services.AddPersistenceInfrastructure(config);
builder.Services.AddIdentityInfrastructure(config);
builder.Services.AddSharedInfrastructure(config);
builder.Services.AddApiVersioningExtension();
builder.Services.AddSwaggerExtension();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();
builder.Services.AddOptions();

builder.Services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();

Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Debug()
             .WriteTo.File("logs/CollectionSystem-.txt", rollingInterval: RollingInterval.Day)
             .CreateLogger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

IConfiguration configuration = app.Configuration;
IWebHostEnvironment environment = app.Environment;

app.UseSwaggerExtension();
app.UseErrorHandlingMiddleware();
app.MapControllers();

app.Run();