
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Model.Users;

namespace SpecBox.Domain.Model.Teams;

[Table("TeamUser")]
public class TeamUser
{
    [Key]
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public bool IsAdmin { get; set; }

    public required User User { get; set; } = null!;
    public required Team Team { get; set; } = null!;
}
