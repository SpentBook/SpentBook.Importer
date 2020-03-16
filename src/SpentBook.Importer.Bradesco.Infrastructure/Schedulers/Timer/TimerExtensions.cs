using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SpentBook.Importer.Bradesco.Infrastructure.Schedulers.Timer
{
    public static class TimerExtensions
    {
        public static IServiceCollection ConfigureScheduleTimer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<TimerHostedService>();
            return services;
        }
    }
}
