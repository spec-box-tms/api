using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Lib;
using SpecBox.Domain.Model.Teams;

namespace SpecBox.Domain.Model.Users;

[Table("User")]
public class User : ICreatedAt, IUpdatedAt
{
    public Guid Id { get; set; }
    public required string Login { get; set; } = null!;
    public required string Email { get; set; } = null!;
    public required string Name { get; set; } = null!;
    public string? Description { get; set; }

    [IsUtc]
    public DateTime CreatedAt { get; set; }
    [IsUtc]
    public DateTime UpdatedAt { get; set; }

    public List<Team> Teams { get; set; } = [];
}
