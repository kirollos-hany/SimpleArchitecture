using Hangfire;

namespace SimpleArchitecture.BackgroundJobs.Activator;

public class HangfireJobActivator : JobActivator
{
    private readonly IServiceProvider _serviceContainer;

    public HangfireJobActivator(IServiceProvider serviceContainer)
    {
        _serviceContainer = serviceContainer;
    }

    public override object ActivateJob(Type jobType)
    {
        return _serviceContainer.GetService(jobType);
    }
}