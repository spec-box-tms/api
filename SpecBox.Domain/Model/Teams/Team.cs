using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Lib;
using SpecBox.Domain.Model.Users;

namespace SpecBox.Domain.Model.Teams;

[Table("Team")]
public class Team : BaseEntity
{
    public Guid Id { get; set; }
    public required string Code { get; set; } = null!;
    public required string Title { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public List<User> Users { get; } = [];
}
