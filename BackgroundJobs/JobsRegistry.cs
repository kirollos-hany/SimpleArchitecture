using Hangfire;
using SimpleArchitecture.BackgroundJobs.Enums;
using SimpleArchitecture.BackgroundJobs.Jobs;

namespace SimpleArchitecture.BackgroundJobs;

public static class JobsRegistry
{
    public static void RegisterUserDevicesJob()
    {
        RecurringJob.AddOrUpdate<UserDevicesJob>(
            nameof(SystemBackgroundJobs.UserDevicesJob),
            job => job.Run(), Cron.Weekly, new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });
    }

    public static void RegisterSerilogCleanJob()
    {
        RecurringJob.AddOrUpdate<LogsCleanJob>(
            nameof(SystemBackgroundJobs.LogsCleanJob),
            job => job.Run(), Cron.Monthly, new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });
    }
}