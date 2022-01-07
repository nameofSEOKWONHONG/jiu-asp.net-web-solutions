namespace Chloe.Utility
{
    public class PairList<T1, T2> : List<Tuple<T1, T2>>
    {
        public PairList()
        {
        }
        public PairList(int capacity) : base(capacity)
        {
        }
        public void Add(T1 t1, T2 t2)
        {
            Tuple<T1, T2> tuple = new Tuple<T1, T2>(t1, t2);
            this.Add(tuple);
        }
    }
}
