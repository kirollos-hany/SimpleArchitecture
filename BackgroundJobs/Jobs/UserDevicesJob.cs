using Microsoft.EntityFrameworkCore;
using SimpleArchitecture.Data;

namespace SimpleArchitecture.BackgroundJobs.Jobs;

public class UserDevicesJob
{
    private readonly SimpleArchitectureDbContext _dbContext;

    private readonly ILogger<UserDevicesJob> _logger;

    public UserDevicesJob(SimpleArchitectureDbContext dbContext, ILogger<UserDevicesJob> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Run()
    {
        var count = await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM UserDevice WHERE LastActiveAt < DATEADD(DAY, -25, GETUTCDATE())");

        _logger.LogWarning($"User devices job cleaned ${count} devices");
    }
}