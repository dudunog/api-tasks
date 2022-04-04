using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasksApi.Models.Entities
{
    [Table("RefreshToken")]
    public partial class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string TokenHash { get; set; }
        
        [Required]
        [MaxLength(1000)]
        public string TokenSalt { get; set; }

        [Column("TS")]
        public DateTime Ts { get; set; }
        public DateTime ExpiryDate { get; set; }
        public virtual User User { get; set; }
    }
}
