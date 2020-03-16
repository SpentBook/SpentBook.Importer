using Hangfire;
using Hangfire.Storage.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpentBook.Importer.Bradesco.Infrastructure.Schedulers.Timer;
using System;
using System.Transactions;

namespace SpentBook.Importer.Bradesco.Infrastructure.Schedulers.HangFire
{
    public static class SchedulerHangFireExtensions
    {
        public static IServiceCollection ConfigureScheduleHangFire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<TimerHostedService>();

            // MYSQL
            {
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

                var connectionString = configuration.GetConnectionString("HangFireMysql");
                var storage = new MySqlStorage(connectionString, options);
                GlobalConfiguration.Configuration.UseStorage(storage);
            }

            // SQL SERVER
            // {
            //     var connectionString = this.Configuration.GetConnectionString("HangFireSqlServer");
            //     services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            // }

            return services;
        }
    }
}
