using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ADF.DataAccess
{
    /// <summary>
    /// 表达式类型转换为SQL操作符
    /// </summary>
    public static class SqlOperator
    {
        public static string ToSqlOperator(this ExpressionType type)
        {
            switch (type)
            {
                case (ExpressionType.AndAlso):
                case (ExpressionType.And):
                    return "AND";
                case (ExpressionType.OrElse):
                case (ExpressionType.Or):
                    return "OR";
                case (ExpressionType.Not):
                    return "NOT";
                case (ExpressionType.NotEqual):
                    return "<>";
                case (ExpressionType.GreaterThan):
                    return ">";
                case (ExpressionType.GreaterThanOrEqual):
                    return ">=";
                case (ExpressionType.LessThan):
                    return "<";
                case (ExpressionType.LessThanOrEqual):
                    return "<=";
                case (ExpressionType.Equal):
                    return "=";
                default:
                    throw new ArgumentNullException("不支持该方法");
            }
        }
    }
}