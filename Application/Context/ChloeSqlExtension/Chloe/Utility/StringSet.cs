namespace Chloe.Utility
{
    public class StringSet : HashSet<string>
    {
        public StringSet() : base()
        {
        }
        public StringSet(IEnumerable<string> collection) : base(collection)
        {
        }
        public StringSet(IEnumerable<string> collection, IEqualityComparer<string> comparer) : base(collection, comparer)
        {
        }

        public StringSet Clone()
        {
            return new StringSet(this, this.Comparer);
        }
    }
}
