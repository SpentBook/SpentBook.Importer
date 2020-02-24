using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpentBook.Importer.Bradesco.Application.UseCases;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpentBook.Importer.Bradesco.HostServices
{
    public class Email
    {
        public void A()
        {
            Console.WriteLine("OPA!!");
        }
    }

    public class OfxFileImporterHostedService : BackgroundService
    {
        private readonly ILogger<OfxFileImporterHostedService> _logger;
        private readonly IOfxFileImporterUseCase _useCase;

        public OfxFileImporterHostedService(
            ILogger<OfxFileImporterHostedService> logger,
            IOfxFileImporterUseCase useCase
        )
        {
            //_backgroundJobs = backgroundJobs;
            _logger = logger;
            _useCase = useCase;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Starting bradesco importer");
            stoppingToken.Register(() => _logger.LogDebug("Stop bradesco importer"));

            //_backgroundJobs.Enqueue(() => _logger.LogInformation("TESTE: " + DateTime.Now.ToString()));

            //BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world!"));
            BackgroundJob.Enqueue<Email>((x) => x.A());
            RecurringJob.AddOrUpdate<Email>((x) => x.A(), "*/1 * * * * *");
            //BackgroundJob.Schedule<Email>((x) => x.A(), new TimeSpan(0, 0, 1));

            var options = new BackgroundJobServerOptions()
            {
                SchedulePollingInterval = TimeSpan.FromMilliseconds(1000)
            };

            using (var server = new BackgroundJobServer(options))
            {
                while (!stoppingToken.IsCancellationRequested);
            }


            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    await _useCase.Run();
            //}
        }


    }
}
