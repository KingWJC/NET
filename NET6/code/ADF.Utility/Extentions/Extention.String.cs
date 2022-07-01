using Newtonsoft.Json;
using System.Data;
using System.Text.RegularExpressions;

namespace ADF.Utility
{
    /// <summary>
    /// String的扩展方法
    /// </summary>
    public static partial class Extention
    {
        public static int ToInt(this string value)
        {
            int result;
            if (Int32.TryParse(value, out result))
                return result;
            else
                return 0;
        }

        public static bool IsMatch(this string value, string pattern)
        {
            return Regex.IsMatch(value, pattern);
        }

        /*
         * @description: json字符串转为DataTable
         * @param {Json字符串}
         * @return {*}
         */
        public static DataTable ToDataTable(this string jsonStr)
        {
            return jsonStr.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject<DataTable>(jsonStr);
        }
    }
}
