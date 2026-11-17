using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.API.Models;

namespace TaskManager.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b); // required for the Identity tables

        b.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);

        b.Entity<TaskItem>(e =>
        {
            e.HasQueryFilter(t => !t.IsDeleted);
            e.Property(t => t.Status).HasConversion<string>();
            e.Property(t => t.Priority).HasConversion<string>();

            e.HasOne(t => t.AssignedToUser).WithMany()
             .HasForeignKey(t => t.AssignedToUserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.CreatedByUser).WithMany()
             .HasForeignKey(t => t.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.Category).WithMany(c => c.Tasks)
             .HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.SetNull);
        });
    }
}