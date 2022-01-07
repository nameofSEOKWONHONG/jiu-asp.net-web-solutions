using Chloe.DbExpressions;

namespace Chloe.RDBMS
{
    /// <summary>
    /// 方法解析处理器。
    /// </summary>
    public interface IMethodHandler
    {
        /// <summary>
        /// 判断是否可以解析传入的方法。
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        bool CanProcess(DbMethodCallExpression exp);

        /// <summary>
        /// 解析传入的方法。
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="generator"></param>
        void Process(DbMethodCallExpression exp, SqlGeneratorBase generator);
    }
}
