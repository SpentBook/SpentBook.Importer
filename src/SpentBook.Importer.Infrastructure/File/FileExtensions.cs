using Microsoft.Extensions.DependencyInjection;
using SpentBook.Importer.Domain.File;
using SpentBook.Importer.Infrastructure.File.Ofx;

namespace SpentBook.Importer.Infrastructure.File
{
    public static class FileExtensions
    {
        public static IServiceCollection ConfigureFile(this IServiceCollection services)
        {
            services.AddTransient<IFileAvailabilityChecker, FileAvailabilityChecker>();
            services.AddTransient<IFileDirectoryUtils, FileDirectoryUtils>();

            // Mappers
            services.AddTransient<IOfxMapper, OfxMapper>();

            return services;
        }
    }
}
