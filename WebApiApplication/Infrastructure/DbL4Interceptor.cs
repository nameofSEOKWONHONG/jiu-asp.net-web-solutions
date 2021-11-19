using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebApiApplication.Infrastructure;

namespace WebApiApplication.Infrastructure
{
    public class DbL4Interceptor : DbCommandInterceptor
    {
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

        public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            base.CommandFailed(command, eventData);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            return base.ReaderExecuted(command, eventData, result);
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            
            return base.ReaderExecuting(command, eventData, result);
        }

        public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
        {
            return base.ScalarExecuted(command, eventData, result);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            return base.ScalarExecuting(command, eventData, result);
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

        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            return base.NonQueryExecuted(command, eventData, result);
        }

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            command.CommandText = _dbL4Provider.ReplaceTable(command.CommandText);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }
    }
}