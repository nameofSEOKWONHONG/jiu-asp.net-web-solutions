using System.Linq.Expressions;

namespace Chloe.Query
{
    public class ScopeParameterDictionary : Dictionary<ParameterExpression, IObjectModel>
    {
        public ScopeParameterDictionary()
        {
        }
        public ScopeParameterDictionary(int capacity) : base(capacity)
        {
        }
        public IObjectModel Get(ParameterExpression parameter)
        {
            IObjectModel model;
            if (!this.TryGetValue(parameter, out model))
            {
                throw new Exception("Can not find the ParameterExpression");
            }

            return model;
        }

        public ScopeParameterDictionary Clone()
        {
            return this.Clone(this.Count);
        }
        public ScopeParameterDictionary Clone(int capacity)
        {
            ScopeParameterDictionary ret = new ScopeParameterDictionary(capacity);
            foreach (var kv in this)
            {
                ret.Add(kv.Key, kv.Value);
            }

            return ret;
        }
        public ScopeParameterDictionary Clone(ParameterExpression key, IObjectModel valueOfkey)
        {
            ScopeParameterDictionary ret = this.Clone(this.Count + 1);
            ret[key] = valueOfkey;
            return ret;
        }
    }
}
