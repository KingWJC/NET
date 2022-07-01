namespace ADF.Utility
{
    /// <summary>
    /// 表名映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : BaseMappingAttribute
    {
        public TableAttribute(string mappingName) : base(mappingName)
        {
        }
    }
}
