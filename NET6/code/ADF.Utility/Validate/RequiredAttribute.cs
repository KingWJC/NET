using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADF.Utility
{
    /// <summary>
    /// 必填项，要求非空
    /// </summary>
    public class RequiredAttribute : BaseValidateAttribute
    {
        public override bool Validate(object obj)
        {
            return !obj.IsNullOrEmpty();
        }
    }
}
