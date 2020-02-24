using Hangfire;
using Hangfire.Storage.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpentBook.Importer.Bradesco.HostServices;
using SpentBook.Importer.Bradesco.Infrastructure.DI.UseCases;
using System;
using System.Transactions;

namespace SpentBook.Importer.Bradesco
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostEnvironment env, ILogger<Startup> logger)
        {
            Configuration = BuildConfiguration(configuration, env, logger);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureHostedServices();
            services.ConfigureUseCases();

            //GlobalConfiguration.Configuration.UseSqlServerStorage(@"Server=(localdb)\MSSQLLocalDB;Database=Teste;Trusted_Connection=True;ConnectRetryCount=0");

            var connectionString = "Data Source=localhost;Initial Catalog=hangfire;User ID=root;Password=admin";
            var options = new MySqlStorageOptions
            {
                TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                QueuePollInterval = TimeSpan.FromSeconds(15),
                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                PrepareSchemaIfNecessary = true,
                DashboardJobListLimit = 50000,
                TransactionTimeout = TimeSpan.FromMinutes(1),
                TablesPrefix = "Hangfire"
            };

            var storage = new MySqlStorage(connectionString, options);

            GlobalConfiguration.Configuration.UseStorage(storage);

            //using (var server = new BackgroundJobServer())
            //{
            //    Console.WriteLine("Hangfire Server started. Press any key to exit...");
            //}
        }

        private IConfiguration BuildConfiguration(IConfiguration configuration, IHostEnvironment env, ILogger<Startup> logger)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName?.ToLower()}.json", optional: true);

            builder.AddEnvironmentVariables();

            var args = Environment.GetCommandLineArgs();
            if (args != null)
                builder.AddCommandLine(args);

            logger.LogInformation("DEBUG: ASPNETCORE_ENVIRONMENT: " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
            logger.LogInformation("DEBUG: IWebHostEnvironment.EnvironmentName: " + env.EnvironmentName);

            return builder.Build();
        }

    }
}
