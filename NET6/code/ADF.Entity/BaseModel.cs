using ADF.Utility;

namespace ADF.Entity
{
    /// <summary>
    /// 基础数据实体模型
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// ORM主键
        /// </summary>
        [Key]
        public int Id { get; set; } 
    }
}