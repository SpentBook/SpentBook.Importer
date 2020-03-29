using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SpentBook.Importer.Infrastructure.Configuration;
using SpentBook.Importer.Infrastructure.EntityFramework;

namespace SpentBook.Importer.Migrations
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var appSettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(appSettings);
            
            var options = new DbContextOptionsBuilder<DatabaseContext>();
            DatabaseExtensions.ConfigureDbContextOptions(configuration, appSettings, options);

            return new DatabaseContext(options.Options);
        }
    }
}
