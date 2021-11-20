using Domain.Enums;

namespace Application.Abstract
{
    public class ResponseResultBase
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
}