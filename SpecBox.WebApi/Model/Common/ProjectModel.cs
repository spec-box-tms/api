using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Common;

public class ProjectModel
{
    [Required] public string Code { get; set; } = null!;
    [Required] public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? RepositoryUrl { get; set; }
    [Required] public VersionModel[] Versions { get; set; } = null!;
}
