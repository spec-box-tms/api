
using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Teams;

public class UpdateTeamUserRequest
{
    [Required]
    public bool IsAdmin { get; set; }
}
