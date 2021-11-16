using SharedLibrary.Abstract;

namespace SharedLibrary.Response
{
    public class ResponseResult<TResult> : ResponseResultBase
    {
        public TResult Result { get; set; } 
    }
}