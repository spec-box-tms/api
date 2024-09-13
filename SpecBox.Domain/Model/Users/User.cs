using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Lib;

namespace SpecBox.Domain.Model.Users;

[Table("User")]
public class User : ICreatedAt, IUpdatedAt
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public required string Login { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public required string Email { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public required string Name { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }
    [IsUtc]
    public required DateTime CreatedAt { get; set; }
    [IsUtc]
    public required DateTime UpdatedAt { get; set; }
}
