
using System.ComponentModel.DataAnnotations;
using SpecBox.WebApi.Model.Users;

namespace SpecBox.WebApi.Model.Teams;

public class TeamUserResponse
{
    [Required]
    public Guid TeamId { get; set; }
    [Required]
    public bool IsAdmin { get; set; }
    [Required]
    public UserResponse User { get; set; } = null!;
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public UserResponse CreatedBy { get; set; } = null!;
    [Required]
    public DateTime UpdatedAt { get; set; }
    [Required]
    public UserResponse UpdatedBy { get; set; } = null!;
}
