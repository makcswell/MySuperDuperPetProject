using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySuperDuperPetProject.TransferDatabaseContext
{
    public class TransferDbContext : DbContext
    {
        public TransferDbContext(DbContextOptions<TransferDbContext> options) : base(options)
        {
            Database.EnsureCreated();
            //Database.Migrate();
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
                conf.HasIndex(u => u.Name).HasDatabaseName("IX_Users_Name").HasMethod("hash").IsUnique();
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
                conf.HasIndex(t => t.HashId).HasDatabaseName("IX_TransferStatistic_HashId").IsUnique();
            });
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<DateTimeOffset>()
                .HaveConversion<DateTimeOffsetConverter>();
        }
    }


    [Table("hub_users", Schema = "users")]
    public class User
    {
        [Key]
        [Required]
        public int Id { get; set; }
      
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public Role Role { get; set; } = null!;
        public ICollection<Transfers>? Transfers { get; set; }
    }
    [Table("hub_roles", Schema = "users")]
    public class Role
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public IEnumerable<string> Roles { get; set; } = [];
        public ICollection<User>? Users { get; set; }

    }

    [Table("hub_transfers", Schema = "transfers")]
    public class Transfers
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public User? User { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [Required]
        public string PageFrom { get; set; } = null!;
        [Required]
        public string PageTo { get; set; } = null!;
        [Required]
        public DateTimeOffset TransferUTC { get; set; }
    }
    [Table("hub_transfers_statistic", Schema = "transfers")]
    public class TransfersStatistic
    {
        [Key]
        [Required]
        public string HashId { get; set; } = null!;
        [Required]
        public string From { get; set; } = null!;
        [Required]
        public string To { get; set; } = null!;
        [Required]
        public int Count { get; set; }
    }
}
