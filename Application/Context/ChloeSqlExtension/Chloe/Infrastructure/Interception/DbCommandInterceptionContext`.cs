namespace Chloe.Infrastructure.Interception
{
    public class DbCommandInterceptionContext<TResult>
    {
        Dictionary<string, object> _dataBag = new Dictionary<string, object>();
        public TResult Result { get; set; }
        /// <summary>
        /// 不为 null 则表示出异常。
        /// </summary>
        public Exception Exception { get; set; }
        public Dictionary<string, object> DataBag { get { return this._dataBag; } }
    }
}
