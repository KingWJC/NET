namespace ADF.Utility
{
    /// <summary>
    /// 基础映射的特性
    /// </summary>
    public class BaseMappingAttribute : Attribute
    {
        private string? _mappingName = null;
        /// <summary>
        /// 映射的名称
        /// </summary>
        public string? MappingName { get { return _mappingName; } }  

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mappingName"></param>
        public BaseMappingAttribute(string mappingName)
        {
            this._mappingName= mappingName; 
        }
    }
}
