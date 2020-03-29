using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpentBook.Importer.Domain.Repository;
using SpentBook.Importer.Infrastructure;
using SpentBook.Importer.Infrastructure.EntityFramework;
using SpentBook.Importer.Infrastructure.File;
using SpentBook.Importer.Infrastructure.Schedulers.Timer;
using SpentBook.Importer.Infrastructure.UseCases;
using System;

namespace SpentBook.Importer
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
