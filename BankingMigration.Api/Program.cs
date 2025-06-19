using BankingMigration.Api.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.Jobs;
using BankingMigration.Domain.Jobs.UseCases;
using BankingMigration.Domain.Notifications;
using BankingMigration.Data.Context;
using BankingMigration.Data.Jobs;
using BankingMigration.Data.Migration;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("MigrationJobsDb");
builder.Services.AddDbContext<MigrationJobsDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI Container Configuration:
builder.Services.AddScoped<IJobsRepository, JobsRepository>();
builder.Services.AddScoped<IJobUseCases, JobUseCases>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<IJobNotifications, AzureServiceBusClient>();

// Auth0 Middleware
var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";
var audience = builder.Configuration["Auth0:Domain"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = domain;
        options.Audience = audience;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("run:migration", policy => policy.Requirements.Add(new
    HasScopeRequirement("admin", domain)));
});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var context = service.GetService<MigrationJobsDbContext>();
    DbInitializer.InitializeDb(context);
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
