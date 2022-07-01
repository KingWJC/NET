using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADF.Utility
{
    /// <summary>
    /// 要求必须是几个int值之一
    /// </summary>
    public class IntValuesAttribute : BaseValidateAttribute
    {
        private int[] _values = null;
        public IntValuesAttribute(params int[] values)
        {
            this._values = values;
        }

        public override bool Validate(object obj)
        {
            return !obj.IsNullOrEmpty() && _values != null && int.TryParse(obj.ToString(), out int iValue) && _values.Contains(iValue);
        }
    }
}
