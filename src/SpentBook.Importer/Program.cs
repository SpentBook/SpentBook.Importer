using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpentBook.Importer.Infrastructure.EntityFramework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SpentBook.Importer
{
    public class Program
    {
        private const string _prefix = "HOST_";
        private const string _hostsettings = "hostsettings.json";

        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile(_hostsettings, optional: true);
                    builder.AddEnvironmentVariables(prefix: _prefix);
                    builder.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((host, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    builder.AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName?.ToLower()}.json", optional: true);
                    builder.AddEnvironmentVariables(prefix: _prefix);
                    builder.AddCommandLine(args);
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddTransient<Startup>();

                    var provider = services.BuildServiceProvider();
                    var startup = provider.GetRequiredService<Startup>();
                    startup.ConfigureServices(services);

                    provider = services.BuildServiceProvider();
                    startup.Configure(provider);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration);
                    logging.AddConsole();
                });

            //await builder.RunConsoleAsync();
            builder.Start();
            Console.ReadLine();
        }
    }
}
