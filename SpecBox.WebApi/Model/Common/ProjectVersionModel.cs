using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Common;

public class ProjectVersionModel
{
    [Required] public string Code { get; set; } = null!;
    [Required] public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? RepositoryUrl { get; set; }
    public string? Version { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}
