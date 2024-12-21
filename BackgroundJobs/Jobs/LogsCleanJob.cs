using Microsoft.Data.SqlClient;

namespace SimpleArchitecture.BackgroundJobs.Jobs;

public class LogsCleanJob
{
    private readonly string _loggingDbConnectionString;

    private readonly ILogger<LogsCleanJob> _logger;

    public LogsCleanJob(IConfiguration configuration, ILogger<LogsCleanJob> logger)
    {
        _logger = logger;
        _loggingDbConnectionString = configuration.GetConnectionString("SqlServerLogging")!;
    }

    public async Task Run()
    {
        await using var sqlConnection = new SqlConnection(_loggingDbConnectionString);
        
        var query = @"DELETE FROM ApplicationLogs WHERE TIMESTAMP < DATEADD(DAY, -30, GETUTCDATE());";
        
        await using var sqlCommand = new SqlCommand(query);
        
        sqlCommand.Connection = sqlConnection;
        
        await sqlConnection.OpenAsync();
        
        var deletedRows = sqlCommand.ExecuteNonQuery();
        
        _logger.LogWarning($"Logs clean job cleaned {deletedRows} rows");
        
        await sqlConnection.CloseAsync();
    }
}