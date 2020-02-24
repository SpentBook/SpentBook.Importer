using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpentBook.Importer.Bradesco.HostServices;
using System.IO;
using System.Threading.Tasks;

namespace SpentBook.Importer.Bradesco
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddTransient<Startup>();

                    var provider = services.BuildServiceProvider();
                    var startup = provider.GetRequiredService<Startup>();
                    startup.ConfigureServices(services);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration);
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();
        }
    }
}
