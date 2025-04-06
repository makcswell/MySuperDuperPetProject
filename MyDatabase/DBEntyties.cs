using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDatabase;

[Table("hub_users", Schema = "users")]
public class User
{
    [Key] [Required] public int Id { get; set; }

    [Required] public string Name { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
    [Required] public Role Role { get; set; } = null!;
    public ICollection<Transfers>? Transfers { get; set; }
}

[Table("hub_roles", Schema = "users")]
public class Role
{
    [Key] [Required] public int Id { get; set; }
    [Required] public string Name { get; set; } = null!;
    [Required] public IEnumerable<string> Roles { get; set; } = [];
    public ICollection<User>? Users { get; set; }
}

[Table("hub_transfers", Schema = "transfers")]
public class Transfers
{
    [Key] [Required] public int Id { get; set; }
    [Required] public User? User { get; set; }
    [Required] [ForeignKey(nameof(User))] public int UserId { get; set; }
    [Required] public string PageFrom { get; set; } = null!;
    [Required] public string PageTo { get; set; } = null!;
    [Required] public DateTimeOffset TransferUTC { get; set; }
}

[Table("hub_transfers_statistic", Schema = "transfers")]
public class TransfersStatistic
{
    [Key] [Required] public string HashId { get; set; } = null!;
    [Required] public string From { get; set; } = null!;
    [Required] public string To { get; set; } = null!;
    [Required] public int Count { get; set; }
}