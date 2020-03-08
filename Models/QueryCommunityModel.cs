using Repair.Entitys;

namespace Repair.Models
{
    public class QueryCommunityModel : PageBase
    {
        public string Name { get; set; }
        
        public int? AdminId { get; set; }
    }
}