using Microsoft.EntityFrameworkCore;
using TaskManager.API.Models;

namespace TaskManager.API.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");

            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).HasColumnName("id");

            entity.Property(t => t.Title)
                  .HasColumnName("title")
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(t => t.Description)
                  .HasColumnName("description")
                  .HasMaxLength(500);

            entity.Property(t => t.IsCompleted)
                  .HasColumnName("is_completed")
                  .HasDefaultValue(false)
                  .IsRequired();

            entity.Property(t => t.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("NOW()")
                  .IsRequired();

            entity.Property(t => t.UpdatedAt)
                  .HasColumnName("updated_at")
                  .HasDefaultValueSql("NOW()")
                  .IsRequired();
        });
    }
}
