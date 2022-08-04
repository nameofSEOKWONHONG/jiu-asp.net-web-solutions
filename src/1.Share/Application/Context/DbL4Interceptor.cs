using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OpenXmlPowerTools;

namespace Application.Context
{
    /// <summary>
    /// dbcommand intercepter
    /// </summary>
    public class DbL4Interceptor : DbCommandInterceptor
    {
        public static ConcurrentDictionary<IDbCommand, DateTime> CommandStartTimes = new();
        public static ConcurrentDictionary<string, TimeSpan> CommandDurations = new();
        
        private readonly DbL4Provider _dbL4Provider;
        public DbL4Interceptor(DbL4Provider dbL4Provider)
        {
            _dbL4Provider = dbL4Provider;
        }
        public override InterceptionResult<DbCommand> CommandCreating(CommandCorrelatedEventData eventData, InterceptionResult<DbCommand> result)
        {
            return base.CommandCreating(eventData, result);
        }

        public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            return base.CommandCreated(eventData, result);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            var end = base.ReaderExecuted(command, eventData, result);
            AccumulateTime(command);
            return end;
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            CommandStartTimes.TryAdd(command, DateTime.Now);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
        {
            var end = base.ScalarExecuted(command, eventData, result);
            AccumulateTime(command);
            return end;
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            CommandStartTimes.TryAdd(command, DateTime.Now);
            return base.ScalarExecuting(command, eventData, result);
        }

        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            var end = base.NonQueryExecuted(command, eventData, result);
            AccumulateTime(command);
            return end;
        }

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            CommandStartTimes.TryAdd(command, DateTime.Now);
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var end = base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
            AccumulateTime(command);
            return end;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            CommandStartTimes.TryAdd(command, DateTime.Now);
            command.CommandText = _dbL4Provider.ReplaceTable(command.CommandText);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var end = base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
            AccumulateTime(command);
            return end;
        }

        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            CommandStartTimes.TryAdd(command, DateTime.Now);
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }
        
        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            CommandStartTimes.TryAdd(command, DateTime.Now);
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var end = base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
            AccumulateTime(command);
            return end;
        }
        
        public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            base.CommandFailed(command, eventData);
        }
        
        public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return base.CommandFailedAsync(command, eventData, cancellationToken);
        }

        public override InterceptionResult DataReaderDisposing(DbCommand command, DataReaderDisposingEventData eventData,
            InterceptionResult result)
        {
            return base.DataReaderDisposing(command, eventData, result);
        }
        
        private void AccumulateTime(DbCommand command) {
            if (CommandStartTimes.TryRemove(command, out
                    var commandStartTime)) {
                var commandDuration = DateTime.Now - commandStartTime;
                CommandDurations.AddOrUpdate(command.CommandText, commandDuration, (_, accumulated) => commandDuration + accumulated);
            }
        }
    }
}