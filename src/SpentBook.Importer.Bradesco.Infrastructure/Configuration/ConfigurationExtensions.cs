using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpentBook.Importer.Bradesco.Infrastructure.Configuration;
using System;

namespace SpentBook.Importer.Bradesco.Infrastructure
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection ConfigureAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(options => configuration.GetSection("AppSettings").Bind(options));
            return services;
        }
    }
}
