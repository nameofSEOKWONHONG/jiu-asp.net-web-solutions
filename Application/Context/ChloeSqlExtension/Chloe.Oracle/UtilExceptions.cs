using System.Reflection;

namespace Chloe.Oracle
{
    static class UtilExceptions
    {
        public static NotSupportedException NotSupportedMethod(MethodInfo method)
        {
            return new NotSupportedException(string.Format("Does not support method '{0}'.", Utils.ToMethodString(method)));
        }
    }
}
