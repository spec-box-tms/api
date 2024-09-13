using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Lib;

namespace SpecBox.Domain.Model.Users;

[Table("PasswordAuth")]
public class PasswordAuth : IUpdatedAt
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public required User User { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public required string Hash { get; set; } = null!;

    [Required]
    public required Guid Salt { get; set; }

    [IsUtc]
    public required DateTime UpdatedAt { get; set; }
}
