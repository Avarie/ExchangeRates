using Hangfire;

namespace CurrencyAggregator.Tasks
{
    public class HangfireJobScheduler
    {
        public static void ScheduleJobs()
        {
            RecurringJob.RemoveIfExists(nameof(GetSourceDataTask));
            RecurringJob.AddOrUpdate<GetSourceDataTask>(nameof(GetSourceDataTask),
                task => task.Run(JobCancellationToken.Null),
                //Cron.Minutely()
                Cron.Hourly()
                );


            RecurringJob.RemoveIfExists(nameof(PrepareDataTask));
            RecurringJob.AddOrUpdate<PrepareDataTask>(nameof(PrepareDataTask),
                task => task.Run(JobCancellationToken.Null),
                Cron.Daily(3, 30)
                );
        }
    }
}