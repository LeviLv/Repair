using Repair.EntityFramework.Domain;

namespace Repair.Entitys
{
    public class CommunityDTO : Community
    {
        public string AdminName { get; set; }
        
        public string RepairManName { get; set; }
    }
}