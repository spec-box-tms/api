
using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Teams;

public class CreateTeamRequest
{
    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[a-zA-Z_]+[a-zA-Z\d-_]*$")]
    public string Code { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }
}