using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace MyDatabase;

public class TransferDbContext : DbContext
{
    public TransferDbContext(DbContextOptions<TransferDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public TransferDbContext() : base()
    {
    }

    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Transfers> Transfers { get; set; } = null!;
    public virtual DbSet<TransfersStatistic> TransfersStatistics { get; set; } = null!;
    public virtual DbSet<Role> Roles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder build)
    {
        build.Entity<User>(conf =>
        {
            conf.HasKey(u => u.Id);
            conf.Property(d => d.Id).ValueGeneratedOnAdd();
            conf.HasOne(u => u.Role).WithMany(r => r.Users);
            conf.HasIndex(u => u.Name).HasDatabaseName("IX_Users_Name").HasMethod("hash");
        });

        build.Entity<Role>(conf =>
        {
            conf.HasKey(u => u.Id);
            conf.Property(d => d.Id).ValueGeneratedOnAdd();
            conf.HasMany(r => r.Users).WithOne(u => u.Role);
        });

        build.Entity<Transfers>(conf =>
        {
            conf.HasKey(t => t.Id);
            conf.Property(d => d.Id).ValueGeneratedOnAdd();
            conf.HasOne(t => t.User).WithMany(u => u.Transfers).HasForeignKey(u => u.UserId);
        });

        build.Entity<TransfersStatistic>(conf =>
        {
            conf.HasKey(t => t.HashId);
            conf.HasIndex(t => t.HashId).HasDatabaseName("IX_TransferStatistic_HashId").HasMethod("hash");
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetConverter>();
    }
}