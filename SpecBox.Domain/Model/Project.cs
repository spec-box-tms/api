using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Lib;

namespace SpecBox.Domain.Model;

[Table("Project")]
public class Project
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string? Version { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? RepositoryUrl { get; set; }
    [IsUtc]
    public DateTime? CreatedAt { get; set; }
    [IsUtc]
    public DateTime? UpdatedAt { get; set; }
}
