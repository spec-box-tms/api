using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Lib;

namespace SpecBox.Domain.Model.Teams;

[Table("Team")]
public class Team : AuditableEntity, IConcurrencyControl
{
    public Guid Id { get; set; }
    public required string Title { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public List<TeamUser> TeamUsers { get; } = [];
    public Guid RowVersion { get; set; }
}
