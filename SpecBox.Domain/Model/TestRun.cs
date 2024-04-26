using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Lib;

namespace SpecBox.Domain.Model;

[Table("TestRun")]
public class TestRun
{
  public Guid Id { get; set; }

  public string Title { get; set; } = null!;
  public string? Description { get; set; }
  [IsUtc]
  public DateTime CreatedAt { get; set; }
  [IsUtc]
  public DateTime? StartedAt { get; set; }
  [IsUtc]
  public DateTime? CompletedAt { get; set; }

  public int TotalCount { get; set; }
  public int PassedCount { get; set; }
  public int FailedCount { get; set; }
  public int BlockedCount { get; set; }  
  public int InvalidCount { get; set; }
  public int SkippedCount { get; set; }

  public string? Environment { get; set; }
  public string? Configuration { get; set; }

  public Guid ProjectId { get; set; }
  public Project Project { get; set; } = null!;
}
