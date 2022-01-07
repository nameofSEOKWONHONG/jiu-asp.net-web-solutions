using System.Reflection;

namespace Chloe.Oracle
{
    static class UtilConstants
    {
        public const string ParameterNamePlaceholer = ":";
        public static readonly string ParameterNamePrefix = ParameterNamePlaceholer + "P_";
        public static readonly string OutputParameterNamePrefix = ParameterNamePlaceholer + "R_";
        public const int InElements = 1000; /* oracle 限定 in 表达式的最大个数 */

        #region MemberInfo constants

        /* TimeSpan */
        public static readonly PropertyInfo PropertyInfo_TimeSpan_TotalDays = typeof(TimeSpan).GetProperty("TotalDays");
        public static readonly PropertyInfo PropertyInfo_TimeSpan_TotalHours = typeof(TimeSpan).GetProperty("TotalHours");
        public static readonly PropertyInfo PropertyInfo_TimeSpan_TotalMinutes = typeof(TimeSpan).GetProperty("TotalMinutes");
        public static readonly PropertyInfo PropertyInfo_TimeSpan_TotalSeconds = typeof(TimeSpan).GetProperty("TotalSeconds");
        public static readonly PropertyInfo PropertyInfo_TimeSpan_TotalMilliseconds = typeof(TimeSpan).GetProperty("TotalMilliseconds");

        #endregion

    }
}
