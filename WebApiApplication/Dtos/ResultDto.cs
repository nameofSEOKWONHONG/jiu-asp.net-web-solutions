namespace WebApiApplication.Dtos
{
    public class ResultBase
    {
        private bool _success;
        public bool Success
        {
            get => _success;
            set
            {
                if (value) this.ResultCode = ENUM_RESULT_CODE.OK;
                else this.ResultCode = ENUM_RESULT_CODE.FAIL;
                _success = value;
            }
            
        }
        public string ResultCode { get; private set; }
        public string Message { get; set; }
    }

    public class ResultDto<TResult> : ResultBase
    {
        public TResult Result { get; set; } 
    }

    public class ENUM_RESULT_CODE
    {
        public static readonly string OK = "00";
        public static readonly string FAIL = "-1";
    }
}