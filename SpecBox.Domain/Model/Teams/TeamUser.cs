
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Lib;
using SpecBox.Domain.Model.Users;

namespace SpecBox.Domain.Model.Teams;

[Table("TeamUser")]
public class TeamUser : ICreatedAt, ICreatedBy, IUpdatedAt, IUpdatedBy
{
    [Key]
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public bool IsAdmin { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedById { get; set; }

    public User CreatedBy { get; set; } = null!;
    public User UpdatedBy { get; set; } = null!;
    public required User User { get; set; } = null!;
    public required Team Team { get; set; } = null!;
}
