namespace Chloe.RDBMS
{
    public interface ISqlBuilder
    {
        int Length { get; }

        string ToSql();
        ISqlBuilder Append(object obj);
        ISqlBuilder Append(object obj1, object obj2);
        ISqlBuilder Append(object obj1, object obj2, object obj3);
        ISqlBuilder Append(object obj1, object obj2, object obj3, object obj4);
        ISqlBuilder Append(object obj1, object obj2, object obj3, object obj4, object obj5);
        ISqlBuilder Append(params object[] objs);
    }

    public class SqlBuilder : ISqlBuilder
    {
        StringBuilder _sb = new StringBuilder();

        public int Length { get { return this._sb.Length; } }

        public string ToSql()
        {
            return this._sb.ToString();
        }
        public ISqlBuilder Append(object obj)
        {
            this._sb.Append(obj);
            return this;
        }
        public ISqlBuilder Append(object obj1, object obj2)
        {
            return this.Append(obj1).Append(obj2);
        }
        public ISqlBuilder Append(object obj1, object obj2, object obj3)
        {
            return this.Append(obj1).Append(obj2).Append(obj3);
        }
        public ISqlBuilder Append(object obj1, object obj2, object obj3, object obj4)
        {
            return this.Append(obj1).Append(obj2).Append(obj3).Append(obj4);
        }
        public ISqlBuilder Append(object obj1, object obj2, object obj3, object obj4, object obj5)
        {
            return this.Append(obj1).Append(obj2).Append(obj3).Append(obj4).Append(obj5);
        }
        public ISqlBuilder Append(params object[] objs)
        {
            if (objs == null)
                return this;

            for (int i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                this.Append(obj);
            }

            return this;
        }
    }

}
