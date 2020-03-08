using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repair.Entitys
{
    public class RepairListInfoDTO
    {
        public int Id { get; set; }

        public int ListId { get; set; }

        public int Status { get; set; }

        public string StatusName { get; set; }

        public string Remake { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
