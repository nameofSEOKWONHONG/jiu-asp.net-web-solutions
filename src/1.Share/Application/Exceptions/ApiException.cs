using System;
using System.Globalization;

namespace Application.Exceptions
{
    /// <summary>
    /// 응답 공통화
    /// </summary>
    public class ApiException : Exception
    {
        public int? Status { get; private set; } = 500;
        public object Value { get; private set; }
        
        public ApiException(int statue, object value) : base()
        {
            this.Status = statue;
            this.Value = value;
        }

        public ApiException(int status, object value, string message) : base(message)
        {
            this.Status = status;
            this.Value = value;
        }

        public ApiException(int status, object value, string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
            this.Status = status;
            this.Value = value;
        }
    }
}