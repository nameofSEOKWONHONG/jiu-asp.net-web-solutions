using System.Data;
using System.Threading.Tasks;

namespace Chloe.Data
{
    public interface IQueryDataReader : IDataReader
    {
        bool AllowReadNextRecord { get; set; }
    }

    public class QueryDataReader : DataReaderDecorator, IQueryDataReader
    {
        bool _beOver;

        public QueryDataReader(IDataReader reader) : base(reader)
        {
        }

        public bool AllowReadNextRecord { get; set; } = true;

        public override bool Read()
        {
            /*
             * 有些驱动（Microsoft.Data.Sqlite）支持循环读取数据，如 reader.Read() 返回 false 以后再次调用 reader.Read() 会返回 true，即又可以从第一条数据开始读取数据了，坑爹- -
             */
            if (this._beOver)
                return false;

            if (!this.AllowReadNextRecord)
                return true;

            var ret = base.Read();

            if (ret == false)
                this._beOver = true;

            this.AllowReadNextRecord = true;
            return ret;
        }

#if netfx
        public override async Task<bool> ReadAsync()
#else
        public override async ValueTask<bool> ReadAsync()
#endif
        {
            /*
             * 有些驱动（Microsoft.Data.Sqlite）支持循环读取数据，如 reader.Read() 返回 false 以后再次调用 reader.Read() 会返回 true，即又可以从第一条数据开始读取数据了，坑爹- -
             */
            if (this._beOver)
                return false;

            if (!this.AllowReadNextRecord)
                return true;

            var ret = await base.ReadAsync();

            if (ret == false)
                this._beOver = true;

            this.AllowReadNextRecord = true;
            return ret;
        }
    }

}
