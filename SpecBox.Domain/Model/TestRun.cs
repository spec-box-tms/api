using System.ComponentModel.DataAnnotations.Schema;

namespace SpecBox.Domain.Model;

[Table("TestRun")]
public class TestRun
{
  public Guid Id { get; set; }

  public string Title { get; set; } = null!;
  public string? Description { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? StartedAt { get; set; }
  public DateTime? CompletedAt { get; set; }

  public Guid ProjectId { get; set; }
  public Project Project { get; set; } = null!;
}
