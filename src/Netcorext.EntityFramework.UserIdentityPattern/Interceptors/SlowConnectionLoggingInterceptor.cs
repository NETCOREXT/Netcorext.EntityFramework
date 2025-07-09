using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Netcorext.EntityFramework.UserIdentityPattern.Interceptors;

public class SlowConnectionLoggingInterceptor : DbConnectionInterceptor
{
    private readonly ILogger<SlowConnectionLoggingInterceptor> _logger;
    private readonly long _slowConnectionLoggingThreshold;

    public SlowConnectionLoggingInterceptor(ILoggerFactory loggerFactory, long slowConnectionLoggingThreshold)
    {
        _logger = loggerFactory.CreateLogger<SlowConnectionLoggingInterceptor>();
        _slowConnectionLoggingThreshold = slowConnectionLoggingThreshold;
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        LogSlowConnection(connection.DataSource, connection.Database, eventData.Duration);
        base.ConnectionOpened(connection, eventData);
    }

    public override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = new CancellationToken())
    {
        LogSlowConnection(connection.DataSource, connection.Database, eventData.Duration);
        return base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    private void LogSlowConnection(string dataSource, string database, TimeSpan duration)
    {
        if (duration.TotalMilliseconds > _slowConnectionLoggingThreshold)
        {
            _logger.LogWarning("Detected slow database connection: DataSource={dataSource}, Database={database} ({Duration})", duration, dataSource, database);
        }
    }
}
