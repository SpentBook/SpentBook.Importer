using Microsoft.Extensions.DependencyInjection;

namespace SpentBook.Importer.Bradesco.HostServices
{
    public static class HostedServicesExtensions
    {
        public static IServiceCollection ConfigureHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<OfxFileImporterHostedService>();
            return services;
        }
    }
}
