using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpentBook.Importer.Bradesco.Domain.Repository;
using SpentBook.Importer.Bradesco.Infrastructure;
using SpentBook.Importer.Bradesco.Infrastructure.EntityFramework;
using SpentBook.Importer.Bradesco.Infrastructure.File;
using SpentBook.Importer.Bradesco.Infrastructure.Schedulers.Timer;
using SpentBook.Importer.Bradesco.Infrastructure.UseCases;
using System;

namespace SpentBook.Importer.Bradesco
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostEnvironment env, ILogger<Startup> logger)
        {
            Configuration = configuration;

            logger.LogInformation("DEBUG: EnvironmentName: " + env.EnvironmentName);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureAppSettings(Configuration);
            services.ConfigureDatabase(Configuration);

            services.ConfigureUseCases();
            services.ConfigureFile();
            //services.ConfigureScheduleHangFire(Configuration);
            services.ConfigureScheduleTimer(Configuration);
        }

        public void Configure(ServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<DatabaseContext>();
            dbContext.Database.Migrate();
        }
    }
}
