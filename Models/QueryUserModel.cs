using Repair.Entitys;

namespace Repair.Models
{
    public class QueryUserModel : PageBase
    {
        public int? AdminId { get; set; }
        public string mobile { get; set; }
        
        public bool? IsRepair { get; set; }
    }
}