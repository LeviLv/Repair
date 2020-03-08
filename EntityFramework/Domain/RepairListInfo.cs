using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repair.EntityFramework.Domain
{
    [Table("RepairListInfo")]
    public class RepairListInfo : IHasCreatedTime
    {
        public RepairListInfo()
        {
            CreateTime = DateTime.Now;
        }
        
        [Key]
        public int Id { get; set; }

        public int ListId { get; set; }

        public int Status { get; set; }

        [MaxLength(200)]
        public string Remake { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
