using Chloe.Threading.Tasks;
using System.Collections;
using System.Data;
using System.Threading.Tasks;

namespace Chloe.Query.Internals
{
    internal class DataReaderEnumerator : IEnumerator<IDataReader>, IAsyncEnumerator<IDataReader>
    {
        bool _disposed;
        bool _hasFinished;
        Func<bool, Task<IDataReader>> _dataReaderCreator;
        IDataReader _reader;
        IDataReader _current;

        public DataReaderEnumerator(Func<bool, Task<IDataReader>> dataReaderCreator)
        {
            this._dataReaderCreator = dataReaderCreator;
        }

        public IDataReader Current { get { return this._current; } }

        object IEnumerator.Current { get { return this._current; } }

        public bool MoveNext()
        {
            return this.MoveNext(false).GetResult();
        }

        BoolResultTask IAsyncEnumerator<IDataReader>.MoveNextAsync()
        {
            return this.MoveNext(true);
        }

        public BoolResultTask MoveNextAsync()
        {
            return this.MoveNext(true);
        }

        async BoolResultTask MoveNext(bool @async)
        {
            if (this._hasFinished || this._disposed)
                return false;

            if (this._reader == null)
            {
                this._reader = await this._dataReaderCreator(@async);
            }

            bool hasData = await this._reader.Read(@async);
            if (hasData)
            {
                this._current = this._reader;
                return true;
            }
            else
            {
                this._reader.Close();
                this._current = default;
                this._hasFinished = true;
                return false;
            }
        }

        public void Dispose()
        {
            if (this._disposed)
                return;

            if (this._reader != null)
            {
                if (!this._reader.IsClosed)
                    this._reader.Close();
                this._reader.Dispose();
                this._reader = null;
            }

            this._hasFinished = true;
            this._current = default;
            this._disposed = true;
        }

        public ValueTask DisposeAsync()
        {
            this.Dispose();
            return default;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}
