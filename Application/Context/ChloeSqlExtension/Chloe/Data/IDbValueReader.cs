using System.Data;

namespace Chloe.Data
{
    public interface IDbValueReader
    {
        object GetValue(IDataReader reader, int ordinal);
    }

    public class DbValueReader : IDbValueReader
    {
        Func<IDataReader, int, object> _getValueHandler;

        public DbValueReader(Func<IDataReader, int, object> getValueHandler)
        {
            this._getValueHandler = getValueHandler;
        }

        public object GetValue(IDataReader reader, int ordinal)
        {
            return this._getValueHandler(reader, ordinal);
        }
    }
}
