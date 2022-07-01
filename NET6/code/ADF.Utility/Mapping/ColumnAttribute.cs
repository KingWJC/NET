namespace ADF.Utility
{
    /// <summary>
    /// 列明映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : BaseMappingAttribute
    {
        public ColumnAttribute(string mappingName) : base(mappingName)
        {
        }
    }
}
