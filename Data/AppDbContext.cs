
using admin_blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace admin_blazor.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnName("id");
            entity.Property(u => u.Phone).HasColumnName("phone");
            entity.Property(u => u.Name).HasColumnName("name");
            entity.Property(u => u.Surname).HasColumnName("surname");
            entity.Property(u => u.Password).HasColumnName("password");
            entity.Property(u => u.Balance).HasColumnName("balance");
            entity.Property(u => u.StudentType).HasColumnName("studenttype");
            entity.Property(u => u.CreatedAt).HasColumnName("created_at");
            entity.Property(u => u.UpdatedAt).HasColumnName("updated_at");
            entity.Property(u => u.Avatar).HasColumnName("avatar");
            entity.Property(u => u.UserType).HasColumnName("user_type");
            entity.Property(u => u.Ban).HasColumnName("ban");
            entity.Property(u => u.Verify).HasColumnName("verify");
            entity.Property(u => u.Sex).HasColumnName("sex");
            entity.Property(u => u.DateOfBirth).HasColumnName("dataofbirth");
        });
    }
}
