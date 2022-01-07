namespace Chloe.SqlServer.Odbc
{
    public static class MsSqlContextExtension
    {
        public static void BulkInsert<TEntity>(this IDbContext dbContext, List<TEntity> entities, string table = null, int? batchSize = null, int? bulkCopyTimeout = null, bool keepIdentity = false)
        {
            MsSqlContext msSqlContext = (MsSqlContext)dbContext;
            msSqlContext.BulkInsert<TEntity>(entities, table, batchSize, bulkCopyTimeout, keepIdentity);
        }
    }
}
