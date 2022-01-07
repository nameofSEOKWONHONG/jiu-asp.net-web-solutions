
namespace Chloe.DbExpressions
{
    /// <summary>
    /// User as T1 , (select * from User) as T1
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Alias = {Alias}")]
    public class DbTableSegment
    {
        public DbTableSegment(DbExpression body, string alias, LockType @lock)
        {
            this.Body = body;
            this.Alias = alias;
            this.Lock = @lock;
        }

        /// <summary>
        /// User、(select * from User)
        /// </summary>
        public DbExpression Body { get; private set; }
        public string Alias { get; private set; }
        public LockType Lock { get; private set; }
    }
}
