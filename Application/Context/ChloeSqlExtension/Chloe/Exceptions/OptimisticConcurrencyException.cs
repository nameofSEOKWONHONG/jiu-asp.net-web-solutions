namespace Chloe.Exceptions
{
    public class OptimisticConcurrencyException : ChloeException
    {
        public OptimisticConcurrencyException() : this("执行乐观并发更新/删除数据时受影响行数为 0")
        {

        }
        public OptimisticConcurrencyException(string message) : base(message)
        {

        }
    }
}
