using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Chloe.Data
{
    public class DataReaderDecorator : IDataReader, IDisposable, IDataRecord
    {
        IDataReader _reader;

        public DataReaderDecorator(IDataReader reader)
        {
            PublicHelper.CheckNull(reader);
            this._reader = reader;
        }

        protected IDataReader PersistedReader { get { return this._reader; } }

        #region IDataReader
        public virtual int Depth { get { return this._reader.Depth; } }
        public virtual bool IsClosed { get { return this._reader.IsClosed; } }
        public virtual int RecordsAffected { get { return this._reader.RecordsAffected; } }

        public virtual void Close()
        {
            this._reader.Close();
        }
        public virtual DataTable GetSchemaTable()
        {
            return this._reader.GetSchemaTable();
        }
        public virtual bool NextResult()
        {
            return this._reader.NextResult();
        }
        public virtual bool Read()
        {
            return this._reader.Read();
        }

#if netfx
        public virtual async Task<bool> ReadAsync()
#else
        public virtual async ValueTask<bool> ReadAsync()
#endif
        {
            DbDataReader dbDataReader = this._reader as DbDataReader;
            if (dbDataReader != null)
            {
                return await dbDataReader.ReadAsync();
            }

            return this.Read();
        }

        public virtual void Dispose()
        {
            this._reader.Dispose();
        }
        #endregion

        #region IDataRecord
        public virtual int FieldCount { get { return this._reader.FieldCount; } }

        public virtual object this[int i] { get { return this._reader[i]; } }
        public virtual object this[string name] { get { return this._reader[name]; } }

        public virtual bool GetBoolean(int i)
        {
            return this._reader.GetBoolean(i);
        }
        public virtual byte GetByte(int i)
        {
            return this._reader.GetByte(i);
        }
        public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this._reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }
        public virtual char GetChar(int i)
        {
            return this._reader.GetChar(i);
        }
        public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return this._reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }
        public virtual IDataReader GetData(int i)
        {
            return this._reader.GetData(i);
        }
        public virtual string GetDataTypeName(int i)
        {
            return this._reader.GetDataTypeName(i);
        }
        public virtual DateTime GetDateTime(int i)
        {
            return this._reader.GetDateTime(i);
        }
        public virtual decimal GetDecimal(int i)
        {
            return this._reader.GetDecimal(i);
        }
        public virtual double GetDouble(int i)
        {
            return this._reader.GetDouble(i);
        }
        public virtual Type GetFieldType(int i)
        {
            return this._reader.GetFieldType(i);
        }
        public virtual float GetFloat(int i)
        {
            return this._reader.GetFloat(i);
        }
        public virtual Guid GetGuid(int i)
        {
            return this._reader.GetGuid(i);
        }
        public virtual short GetInt16(int i)
        {
            return this._reader.GetInt16(i);
        }
        public virtual int GetInt32(int i)
        {
            return this._reader.GetInt32(i);
        }
        public virtual long GetInt64(int i)
        {
            return this._reader.GetInt64(i);
        }
        public virtual string GetName(int i)
        {
            return this._reader.GetName(i);
        }
        public virtual int GetOrdinal(string name)
        {
            return this._reader.GetOrdinal(name);
        }
        public virtual string GetString(int i)
        {
            return this._reader.GetString(i);
        }
        public virtual object GetValue(int i)
        {
            return this._reader.GetValue(i);
        }
        public virtual int GetValues(object[] values)
        {
            return this._reader.GetValues(values);
        }
        public virtual bool IsDBNull(int i)
        {
            return this._reader.IsDBNull(i);
        }
        #endregion
    }
}
