using Repair.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repair.EntityFramework.Domain
{
    [Table("RepairList")]
    public class RepairList
    {
        [Key]
        public int Id { get; set; }

        public int Status { get; set; } = (int)RepairStatusEnum.Init;

        public int UserId { get; set; }
        
        public int CommunityId { get; set; }

        public int? RepairManId { get; set; }

        [MaxLength(200)]
        public string Remake { get; set; }

        public DateTime? CreateTime { get; set; } = DateTime.Now;

        public DateTime? EndTime { get; set; }
    }
}
