using System.Reflection;

namespace ADF.Utility
{
    /// <summary>
    /// 类型映射
    /// </summary>
    public static partial class Extention
    {
        /// <summary>
        /// 获取类型的映射名称
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">类型实例</param>
        /// <returns></returns>
        public static string? GetMappingName<T>(this T t) where T : MemberInfo
        {
            if (t.IsDefined(typeof(BaseMappingAttribute), true))
            {
                var attribute = t.GetCustomAttribute<BaseMappingAttribute>();
                return attribute?.MappingName;
            }
            else
            {
                return t.Name;
            }
        }

        /// <summary>
        /// 获取数据实体类的映射表名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns>表名</returns>
        public static string? GetMappingTableName(this Type type)
        {
            if (type.IsDefined(typeof(TableAttribute), true))
            {
                var attribute = type.GetCustomAttribute<TableAttribute>();
                return attribute?.MappingName;
            }
            else
            {
                return type.Name;
            }
        }

        /// <summary>
        /// 获取除了主键之外的属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertiesWithNoKey(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => !p.IsDefined(typeof(KeyAttribute), true));
        }

        /// <summary>
        /// 获取主键属性(每个类型只能有一个主键）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo? GetKeyProperty(this Type type)
        {
            var key = type
                .GetProperties()
                .Where(p => p.IsDefined(typeof(KeyAttribute), true))
                .FirstOrDefault();

            return key;
        }

        /// <summary>
        /// 获取数据实体类的字段映射的列名称
        /// </summary>
        /// <param name="prop">字段</param>
        /// <returns>列名</returns>
        public static string? GetMappingColumnName(this PropertyInfo prop)
        {
            if (prop.IsDefined(typeof(ColumnAttribute), true))
            {
                var attribute = prop.GetCustomAttribute<ColumnAttribute>();
                return attribute?.MappingName;
            }
            else
            {
                return prop.Name;
            }
        }

        /// <summary>
        /// 数据有效性验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool ValidateModel<T>(this T t)
        {
            Type type = typeof(T);
            foreach (var prop in type.GetProperties())
            {
                if (prop.IsDefined(typeof(BaseValidateAttribute), true))
                {
                    object value = prop.GetValue(t, null);
                    var attributeList = prop.GetCustomAttributes<BaseValidateAttribute>();
                    foreach (var attr in attributeList)
                    {
                        if (!attr.Validate(value))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
