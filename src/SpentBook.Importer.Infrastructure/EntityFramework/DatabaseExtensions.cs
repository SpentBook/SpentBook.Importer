using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpentBook.Importer.Domain.Repository;
using SpentBook.Importer.Infrastructure.Configuration;

namespace SpentBook.Importer.Infrastructure.EntityFramework
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>((provider, options) =>
            {
                var appSettings = provider.GetRequiredService<IOptions<AppSettings>>();
                ConfigureDbContextOptions(configuration, appSettings.Value, options);
            });

            services.AddTransient<ITransactionRepository, TransactionRepository>();

            return services;
        }

        public static void ConfigureDbContextOptions(IConfiguration configuration, AppSettings appSettings, DbContextOptionsBuilder options)
        {
            options.EnableSensitiveDataLogging(true);

            switch (appSettings.DataBaseName)
            {
                case "MySql":
                    options.UseMySql(
                        connectionString: configuration.GetConnectionString("DatabaseMySql"),
                        mySqlOptionsAction: opt => opt.MigrationsAssembly(appSettings.MigrationAssemblyMySql)
                    );
                    break;
                default:
                    options.UseSqlServer(
                        connectionString: configuration.GetConnectionString("DatabaseSqlServer"),
                        sqlServerOptionsAction: opt => opt.MigrationsAssembly(appSettings.MigrationAssemblySqlServer)
                    );
                    break;
            }
        }
    }
}
