using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repair.EntityFramework.Domain
{
    [Table("Users")]
    public class User : IHasCreatedTime, IHasModifyTime
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(20)]
        public string Mobile { get; set; }

        /// <summary>
        /// 居住小区id
        /// </summary>
        [MaxLength(20)]
        public int CommunityId { get; set; }

        /// <summary>
        /// 门牌号
        /// </summary>
        [MaxLength(20)]
        public string HomeNum { get; set; }

        /// <summary>
        /// 小区住址
        /// </summary>
        [MaxLength(256)]
        public string HomeAddress { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(20)]
        public string PassWord { get; set; } = "1234";

        /// <summary>
        /// 是否是维修工
        /// </summary>
        public bool IsRepairMan { get; set; } = false;
        
        /// <summary>
        /// 是否后台管理人员
        /// </summary>
        public bool IsAdmin  {get; set; } = false;
        
        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime ModifyTime { get; set; } = DateTime.Now;
    }
}
