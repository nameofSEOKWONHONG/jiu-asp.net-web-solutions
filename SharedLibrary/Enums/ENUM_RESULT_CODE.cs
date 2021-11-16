using eXtensionSharp;

namespace SharedLibrary.Enums
{
    public class ENUM_RESULT_CODE : XEnumBase<ENUM_RESULT_CODE>
    {
        public static ENUM_RESULT_CODE OK = Define("00");
        public static ENUM_RESULT_CODE FAIL = Define("-1");
    }
}