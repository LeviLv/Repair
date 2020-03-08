using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repair.EntityFramework.Domain;

namespace Repair.Entitys
{
    public class RepairListDTO
    {
        public int Id { get; set; }

        public int Status { get; set; }

        public string statusName { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int? RepairManId { get; set; }

        public User RepairMan { get; set; }

        public string Remake { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}