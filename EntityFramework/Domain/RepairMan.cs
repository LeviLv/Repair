using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repair.EntityFramework.Domain
{
    [Table("RepairMan")]
    public class RepairMan
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// 负责的小区id
        /// </summary>
        [MaxLength(20)]
        public string CommunityIds { get; set; }
    }
}
