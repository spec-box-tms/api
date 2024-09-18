
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpecBox.Domain.Model.Teams;

[Table("TeamUser")]
public class TeamUser
{
    [Key]
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public bool IsAdmin { get; set; }
}