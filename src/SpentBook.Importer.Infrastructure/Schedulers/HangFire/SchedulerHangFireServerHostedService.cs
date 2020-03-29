using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpentBook.Importer.Application.UseCases;
using SpentBook.Importer.Infrastructure.Schedulers.Timer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpentBook.Importer.Infrastructure.Schedulers.HangFire
{
    public class Email
    {
        public void A()
        {
            Console.WriteLine("OPA!!");
        }
    }

    public class SchedulerHangFireServerHostedService : BackgroundService
    {
        private readonly ILogger<TimerHostedService> _logger;
        private readonly IOfxFileImporterUseCase _useCase;

        public SchedulerHangFireServerHostedService(
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
            SchedulerManager.Configure();

            var options = new BackgroundJobServerOptions()
            {
                SchedulePollingInterval = TimeSpan.FromMilliseconds(1000)
            };

            await new BackgroundJobServer(options).WaitForShutdownAsync(cancellationToken);
        }
    }
}
