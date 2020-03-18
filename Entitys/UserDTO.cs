using Repair.AutoMapper;
using Repair.EntityFramework.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repair.Entitys
{
    public class UserDTO : IHasAutoMapper<User>
    {
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 居住小区id
        /// </summary>
        public int CommunityId { get; set; }

        /// <summary>
        /// 门牌号
        /// </summary>
        public string HomeNum { get; set; }

        /// <summary>
        /// 小区住址
        /// </summary>
        public string HomeAddress { get; set; }

        /// <summary>
        /// 是否已经注册
        /// </summary>
        public bool IsRegister { get; set; }

        public DateTime? CreateTime { get; set; }
        
        public bool IsRepairMan { get; set; }
        
        public bool IsAdmin  {get; set; }

    }
}
