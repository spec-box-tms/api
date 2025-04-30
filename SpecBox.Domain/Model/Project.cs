using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Lib;
using SpecBox.Domain.Model.Teams;

namespace SpecBox.Domain.Model;

[Table("Project")]
public class Project: ICreatedAt, IUpdatedAt
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Version { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? RepositoryUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? TeamId { get; set; } = null!;
    public Team? Team { get; set; } = null!;
}
