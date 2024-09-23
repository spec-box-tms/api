using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Lib;

namespace SpecBox.Domain.Model.Users;

[Table("PasswordAuth")]
public class PasswordAuth : IUpdatedAt
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public required User User { get; set; } = null!;
    public required string Hash { get; set; } = null!;
    public required Guid Salt { get; set; }

    public required DateTime UpdatedAt { get; set; }
}
