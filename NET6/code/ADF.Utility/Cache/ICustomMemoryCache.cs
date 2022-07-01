using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADF.Utility
{
    public interface ICustomMemoryCache
    {
        void Remove(object key);
        bool TryGetValue(object key, out object value);

        void Set(object key, object value);
    }
}
