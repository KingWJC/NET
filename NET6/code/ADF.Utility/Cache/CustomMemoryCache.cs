using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADF.Utility
{
    public class CustomMemoryCache : ICustomMemoryCache
    {
        public void Remove(object key)
        {
            CustomCache.Remove(key?.ToString());
        }

        public void Set(object key, object value)
        {
            CustomCache.Add(key?.ToString(), value);
        }

        public bool TryGetValue(object key, out object value)
        {
            if (!CustomCache.Exists(key?.ToString()))
            {
                value = null;
                return false;
            }
            else
            {
                value = CustomCache.Get<object>(key?.ToString());
                return true;
            }
        }
    }
}
