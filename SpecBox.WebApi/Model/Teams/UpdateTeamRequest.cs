
using System.ComponentModel.DataAnnotations;
using SpecBox.Domain.Lib;

namespace SpecBox.WebApi.Model.Teams;

public class UpdateTeamRequest : IConcurrencyControl
{
    [MaxLength(100)]
    [MinLength(2)]
    public string? Title { get; set; }
    [MaxLength(1000)]
    public string? Description { get; set; }
    [Required]
    public Guid RowVersion { get; set; }
}
