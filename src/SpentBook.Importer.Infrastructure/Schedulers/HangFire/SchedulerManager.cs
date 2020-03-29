using Hangfire;

namespace SpentBook.Importer.Infrastructure.Schedulers.HangFire
{
    public static class SchedulerManager
    {
        public static void Configure()
        {
            //_backgroundJobs.Enqueue(() => _logger.LogInformation("TESTE: " + DateTime.Now.ToString()));

            //BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world!"));
            BackgroundJob.Enqueue<Email>((x) => x.A());
            RecurringJob.AddOrUpdate<Email>((x) => x.A(), "*/1 * * * * *");
            //BackgroundJob.Schedule<Email>((x) => x.A(), new TimeSpan(0, 0, 1));
        }
    }
}
