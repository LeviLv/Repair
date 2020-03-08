using Repair.Entitys;

namespace Repair.Models
{
    public class QueryUserModel : PageBase
    {
        public string mobile { get; set; }
        
        public bool? IsRepair { get; set; }
    }
}