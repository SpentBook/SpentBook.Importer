using Microsoft.Extensions.DependencyInjection;
using SpentBook.Importer.Application.UseCases;

namespace SpentBook.Importer.Infrastructure.UseCases
{
    public static class UseCaseExtensions
    {
        public static IServiceCollection ConfigureUseCases(this IServiceCollection services)
        {
            services.AddTransient<IOfxFileImporterUseCase, OfxFileImporterUseCase>();
            return services;
        }
    }
}
