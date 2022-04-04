using Microsoft.EntityFrameworkCore;
using TasksApi.Models.Entities;

namespace TasksApi.Data
{
    public partial class TasksDbContext : DbContext
    {
        public TasksDbContext()
        {
        }

        public TasksDbContext(DbContextOptions<TasksDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set;}
        public virtual DbSet<Models.Entities.Task> Tasks { get; set;}
        public virtual DbSet<User> Users { get; set;}
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(entity => 
            {
                entity.Property(e => e.ExpiryDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Ts).HasColumnType("smalldatetime");
            });

            modelBuilder.Entity<Models.Entities.Task>(entity => 
            {
                entity.Property(e => e.Ts).HasColumnType("smalldatetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Ts).HasColumnType("smalldatetime");
            });
        }
    }
}
