using System.ComponentModel.DataAnnotations;
using SpecBox.Domain.Lib;

namespace SpecBox.WebApi.Model.Users;

public class UserResponse : IConcurrencyControl
{
    [Required]
    public string Id { get; set; } = null!;

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
    [Required]
    public Guid RowVersion { get; set; }
}
