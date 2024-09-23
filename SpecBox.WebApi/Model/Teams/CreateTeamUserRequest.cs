
using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Teams;

public class CreateTeamUserRequest
{
    [Required]
    public string Login { get; set; } = null!;
    [Required]
    public bool IsAdmin { get; set; }
}
