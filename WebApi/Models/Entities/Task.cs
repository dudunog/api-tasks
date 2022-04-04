using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasksApi.Models.Entities
{
    [Table("Task")]
    public partial class Task
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public bool IsCompleted { get; set; }

        [Column("TS")]
        public DateTime Ts { get; set; }
        public virtual User User { get; set; }
    }
}
