using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Auth;

public class UserResponse
{
    [Required]
    public string Login { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }
}
