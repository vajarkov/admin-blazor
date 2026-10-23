using admin_blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace admin_blazor.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Config> Configs => Set<Config>();
    public DbSet<Logspay> Logspays => Set<Logspay>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Course> Courses => Set<Course>();

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

        modelBuilder.Entity<Config>(entity =>
        {
            entity.ToTable("config");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id).HasColumnName("id");
            entity.Property(c => c.PriceKzt).HasColumnName("pricekzt");
        });

        modelBuilder.Entity<Logspay>(entity =>
        {
            entity.ToTable("logspay");
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Id).HasColumnName("id");
            entity.Property(l => l.Time).HasColumnName("time");
            entity.Property(l => l.UserId).HasColumnName("user");
            entity.Property(l => l.Cancellation).HasColumnName("cancellation");
            entity.Property(l => l.Lvl).HasColumnName("lvl");
            entity.Property(l => l.Kzt).HasColumnName("kzt");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("teacher");
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id).HasColumnName("id");
            entity.Property(t => t.Login).HasColumnName("login");
            entity.Property(t => t.Education).HasColumnName("education");
            entity.Property(t => t.Expert).HasColumnName("expert");
            entity.Property(t => t.Password).HasColumnName("password");
            entity.Property(t => t.Description).HasColumnName("description");
            entity.Property(t => t.Youtube).HasColumnName("youtube");
            entity.Property(t => t.Cabinet).HasColumnName("cabinet");
            entity.Property(t => t.Price).HasColumnName("price");
            entity.Property(t => t.Avatar).HasColumnName("avatar");
            entity.Property(t => t.Price1).HasColumnName("price1");
            entity.Property(t => t.Price2).HasColumnName("price2");
            entity.Property(t => t.Price3).HasColumnName("price3");
            entity.Property(t => t.Telegram).HasColumnName("telegram");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("courses");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(c => c.VId)
                .HasColumnName("v_id");

            entity.Property(c => c.Name)
                .HasColumnName("name")
                .HasMaxLength(191);

            entity.Property(c => c.Price)
                .HasColumnName("price");

            entity.Property(c => c.Duration)
                .HasColumnName("duration");

            entity.Property(c => c.Level)
                .HasColumnName("level")
                .HasMaxLength(191);

            entity.Property(c => c.Description)
                .HasColumnName("description")
                .HasColumnType("text");
        });
    }
}
