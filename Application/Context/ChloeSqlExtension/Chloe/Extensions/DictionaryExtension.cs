namespace System.Collections.Generic
{
    static class DictionaryExtension
    {
        /// <summary>
        /// 如果找到 key 对应的value，则返回 value。否则返回 default(TValue)
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue FindValue<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            TValue value;
            if (!dic.TryGetValue(key, out value))
                value = default(TValue);

            return value;
        }
    }
}
