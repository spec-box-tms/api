
using System.ComponentModel.DataAnnotations;
using SpecBox.WebApi.Model.Auth;

namespace SpecBox.WebApi.Model.Teams;

public class TeamUserResponse
{
    [Required]
    public Guid TeamId { get; set; }
    [Required]
    public bool IsAdmin { get; set; }
    [Required]
    public UserResponse User { get; set; } = null!;
}
