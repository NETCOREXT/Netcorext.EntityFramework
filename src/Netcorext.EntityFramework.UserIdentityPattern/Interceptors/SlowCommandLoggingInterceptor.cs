using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Netcorext.EntityFramework.UserIdentityPattern.Interceptors;

public class SlowCommandLoggingInterceptor : DbCommandInterceptor
{
    private readonly long _slowCommandLoggingThreshold;
    private readonly ILogger<SlowCommandLoggingInterceptor> _logger;

    public SlowCommandLoggingInterceptor(ILoggerFactory loggerFactory, long slowCommandLoggingThreshold)
    {
        _slowCommandLoggingThreshold = slowCommandLoggingThreshold;
        _logger = loggerFactory.CreateLogger<SlowCommandLoggingInterceptor>();
    }

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        LogSlowCommand(command.CommandText, eventData.Duration);

        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = new())
    {
        LogSlowCommand(command.CommandText, eventData.Duration);

        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        LogSlowCommand(command.CommandText, eventData.Duration);

        return base.ScalarExecuted(command, eventData, result);
    }

    public override ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object? result, CancellationToken cancellationToken = new())
    {
        LogSlowCommand(command.CommandText, eventData.Duration);

        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        LogSlowCommand(command.CommandText, eventData.Duration);

        return base.NonQueryExecuted(command, eventData, result);
    }

    public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = new())
    {
        LogSlowCommand(command.CommandText, eventData.Duration);

        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    private void LogSlowCommand(string commandText, TimeSpan duration)
    {
        if (duration.TotalMilliseconds > _slowCommandLoggingThreshold)
        {
            _logger.LogWarning("Detected slow SQL command: ({Duration})\n{CommandText}", duration, commandText);
        }
    }
}
