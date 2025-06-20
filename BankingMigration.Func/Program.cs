using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BankingMigration.Domain.BankAccounts;
using BankingMigration.Domain.Jobs;
using BankingMigration.Domain.Migration;
using BankingMigration.Domain.Notifications;
using BankingMigration.Func.Migration;
using BankingMigration.Data.Context;
using BankingMigration.Data.BankAccounts;
using BankingMigration.Data.Migration.Jobs;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:MigrationJobsDb");
builder.Services.AddDbContext<MigrationJobsDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IJobsRepository, JobsRepository>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<IJobNotifications, AzureServiceBusClient>();
builder.Services.AddScoped<IMigrationService, MigrationApiClient>();

builder.Build().Run();
