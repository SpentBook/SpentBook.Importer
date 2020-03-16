using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using SpentBook.Importer.Bradesco.Application.UseCases;

namespace SpentBook.Importer.Bradesco.Infrastructure.Schedulers.Timer
{
    public class TimerHostedService : BackgroundService
    {
        private readonly ILogger<TimerHostedService> _logger;
        private readonly IOfxFileImporterUseCase _useCase;

        public TimerHostedService(
            ILogger<TimerHostedService> logger,
            IOfxFileImporterUseCase useCase
        )
        {
            //_backgroundJobs = backgroundJobs;
            _logger = logger;
            _useCase = useCase;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting bradesco importer");
            cancellationToken.Register(() => _logger.LogDebug("Stop bradesco importer"));

            while (!cancellationToken.IsCancellationRequested)
            {
                await _useCase.Run();
                await Task.Delay(1000);
            }
        }
    }
}
