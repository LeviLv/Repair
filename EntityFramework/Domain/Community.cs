using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repair.EntityFramework.Domain
{
    [Table("Community")]
    public class Community : IHasCreatedTime
    {
        [Key]
        public int? Id { get; set; }

        /// <summary>
        /// 小区名称
        /// </summary>
        [MaxLength(20)]
        public string Name { get; set; }
        
        public int AdminId { get; set; }
        
        public int RepairManId { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
