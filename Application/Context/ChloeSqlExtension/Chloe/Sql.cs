namespace Chloe
{
    public class Sql
    {
        /// <summary>
        /// 比较两个相同类型的对象值是否相等，仅支持在 lambda 表达式树中使用。
        /// 使用此方法与双等号（==）的区别是：a => Sql.Equals(a.Name, a.XName) 会被翻译成 a.Name == a.XName，而 a => a.Name == a.XName 则会被翻译成 a.Name == a.XName or (a.Name is null and a.XName is null)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool Equals<T>(T value1, T value2)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// 比较两个相同类型的对象值是否不相等，仅支持在 lambda 表达式树中使用。
        /// 使用此方法与不等号（!=）的区别是：a => Sql.NotEquals(a.Name, a.XName) 会被翻译成 a.Name != a.XName，而 a => a.Name != a.XName 则会被翻译成 a.Name != a.XName or (a.Name is null and a.XName is not null) or (a.Name is not null and a.XName is null)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool NotEquals<T>(T value1, T value2)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 比较两个相同类型的对象值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="compareType"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool Compare<T>(T value1, CompareType compareType, T value2)
        {
            throw new NotSupportedException();
        }

        public static T NextValueForSequence<T>(string sequenceName, string sequenceSchema)
        {
            throw new NotSupportedException();
        }

        public static int Count()
        {
            return 0;
        }
        public static long LongCount()
        {
            return 0;
        }

        /// <summary>
        /// 求最大值。考虑到满足条件的数据个数为零的情况，为避免报错，可在 lambda 中将相应字段强转成可空类型，如 query.Select(a => Sql.Max((double?)a.Price))。
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p"></param>
        /// <returns></returns>
        public static TResult Max<TResult>(TResult p)
        {
            return p;
        }
        /// <summary>
        /// 求最小值。考虑到满足条件的数据个数为零的情况，为避免报错，可在 lambda 中将相应字段强转成可空类型，如 query.Select(a => Sql.Min((double?)a.Price))。
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p"></param>
        /// <returns></returns>
        public static TResult Min<TResult>(TResult p)
        {
            return p;
        }

        public static int Sum(int p)
        {
            return p;
        }
        public static int? Sum(int? p)
        {
            return p;
        }
        public static long Sum(long p)
        {
            return p;
        }
        public static long? Sum(long? p)
        {
            return p;
        }
        public static decimal Sum(decimal p)
        {
            return p;
        }
        public static decimal? Sum(decimal? p)
        {
            return p;
        }
        public static double Sum(double p)
        {
            return p;
        }
        public static double? Sum(double? p)
        {
            return p;
        }
        public static float Sum(float p)
        {
            return p;
        }
        public static float? Sum(float? p)
        {
            return p;
        }

        public static double? Average(int p)
        {
            return p;
        }
        public static double? Average(int? p)
        {
            return p;
        }
        public static double? Average(long p)
        {
            return p;
        }
        public static double? Average(long? p)
        {
            return p;
        }
        public static decimal? Average(decimal p)
        {
            return p;
        }
        public static decimal? Average(decimal? p)
        {
            return p;
        }
        public static double? Average(double p)
        {
            return p;
        }
        public static double? Average(double? p)
        {
            return p;
        }
        public static float? Average(float p)
        {
            return p;
        }
        public static float? Average(float? p)
        {
            return p;
        }


        public static int? DiffYears(DateTime? dateTime1, DateTime? dateTime2)
        {
            throw new NotSupportedException();
        }
        public static int? DiffMonths(DateTime? dateTime1, DateTime? dateTime2)
        {
            throw new NotSupportedException();
        }
        public static int? DiffDays(DateTime? dateTime1, DateTime? dateTime2)
        {
            throw new NotSupportedException();
        }
        public static int? DiffHours(DateTime? dateTime1, DateTime? dateTime2)
        {
            throw new NotSupportedException();
        }
        public static int? DiffMinutes(DateTime? dateTime1, DateTime? dateTime2)
        {
            throw new NotSupportedException();
        }
        public static int? DiffSeconds(DateTime? dateTime1, DateTime? dateTime2)
        {
            throw new NotSupportedException();
        }
        public static int? DiffMilliseconds(DateTime? dateTime1, DateTime? dateTime2)
        {
            throw new NotSupportedException();
        }
        public static int? DiffMicroseconds(DateTime? dateTime1, DateTime? dateTime2)
        {
            throw new NotSupportedException();
        }
    }
}
