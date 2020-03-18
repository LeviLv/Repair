using System;

namespace Repair.Entitys
{
    public class RepairManDTO : UserDTO
    {
        public string CommunityName { get; set; }
        
        public string AdminName { get; set; }

        public string CommunityIds { get; set; }
    }
}