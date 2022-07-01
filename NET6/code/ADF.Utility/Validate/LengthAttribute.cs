using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADF.Utility
{
    /// <summary>
    /// 数据长度范围，左闭右开
    /// </summary>
    public class LengthAttribute : BaseValidateAttribute
    {
        private int _min;
        private int _max;

        public LengthAttribute(int min, int max)
        {
            this._min = min;
            this._max = max;
        }

        public override bool Validate(object obj)
        {
            return !obj.IsNullOrEmpty() && obj.ToString().Length >= _min && obj.ToString().Length < _max;
        }
    }
}
