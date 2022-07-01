using ADF.Utility;

namespace ADF.Entity
{
    /// <summary>
    /// 用户
    /// </summary>
    [Table("Base_User")]
    public class User : BaseModel
    {
        /// <summary>
        /// 登录名
        /// </summary>
        [Length(2, 10)]
        [Required]
        public String LoginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 用户状态  0正常 1冻结 2删除
        /// </summary>
        [Required]
        [IntValues(1, 2, 4, 8)]
        public int State { get; set; }

        /// <summary>
        /// 用户类型  1 普通用户 2管理员 4超级管理员
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        public string CreatorId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime LastModifyTime { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        public string LastModifierId { get; set; }
    }
}
