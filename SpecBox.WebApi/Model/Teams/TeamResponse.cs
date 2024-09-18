
using System.ComponentModel.DataAnnotations;
using SpecBox.WebApi.Model.Auth;

namespace SpecBox.WebApi.Model.Teams;

public class TeamResponse
{
    public Guid Id { get; set; }

    [Required]
    public string Code { get; set; } = null!;

    [Required]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public List<UserResponse> Users { get; set; } = [];
}