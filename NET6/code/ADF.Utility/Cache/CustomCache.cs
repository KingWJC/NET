using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADF.Utility
{
    /// <summary>
    /// 第三方封装的缓存--redis-memorycache
    /// </summary>
    public class CustomCache
    {
        private static Dictionary<string, object> CustomCacheDictionary = new Dictionary<string, object>();
        public static void Add(string key, object oValue)
        {
            CustomCacheDictionary.Add(key, oValue);
        }
        public static T Get<T>(string key)
        {
            return (T)CustomCacheDictionary[key];
        }
        public static bool Exists(string key)
        {
            return CustomCacheDictionary.ContainsKey(key);
        }
        public static void Remove(string key)
        {
            CustomCacheDictionary.Remove(key);
        }

        public static void Clear()
        {
            CustomCacheDictionary.Clear();
        }

        public static T GetByCache<T>(string key, Func<T> func)
        {
            T t = default(T);
            if (!Exists(key))
            {
                t = func.Invoke();
                CustomCache.Add(key, t);
            }
            else
            {
                t = Get<T>(key);
            }
            return t;
        }
    }
}
