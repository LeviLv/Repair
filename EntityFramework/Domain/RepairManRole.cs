using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repair.EntityFramework.Domain
{
    [Table("RepairManRole")]
    public class RepairManRole
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        public int CommunityId { get; set; }
    }
}