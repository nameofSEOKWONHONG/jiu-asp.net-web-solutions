using System.Data;

namespace Chloe.Data
{
    class InternalDataReader : DataReaderDecorator, IDataReader, IDisposable, IDataRecord
    {
        IAdoSession _adoSession;
        IDbCommand _cmd;
        List<OutputParameter> _outputParameters;
        bool _disposed = false;

        public InternalDataReader(IAdoSession adoSession, IDataReader reader, IDbCommand cmd, List<OutputParameter> outputParameters) : base(reader)
        {
            PublicHelper.CheckNull(adoSession);
            PublicHelper.CheckNull(cmd);

            this._adoSession = adoSession;
            this._cmd = cmd;
            this._outputParameters = outputParameters;
        }

        public override void Close()
        {
            if (this.PersistedReader.IsClosed)
            {
                return;
            }

            try
            {
                this.PersistedReader.Close();
                this.PersistedReader.Dispose();/* Tips：.NET Core 的 SqlServer 驱动 System.Data.SqlClient(4.1.0) 中，调用 DataReader.Dispose() 方法后才能拿到 Output 参数值，这算是坑爹么？？ */
                OutputParameter.CallMapValue(this._outputParameters);
            }
            finally
            {
                this._adoSession.Complete();
            }
        }

        public override void Dispose()
        {
            if (this._disposed)
                return;

            this.Close();
            this._cmd.Dispose();

            this._disposed = true;
        }
    }
}
