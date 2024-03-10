using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.TestRun;

public class TestResultHistoryModel
{
  [Required] public Guid Id { get; set; }
  [Required] public Guid TestRunId { get; set; }
  [Required] public string TestRunTitle { get; set; } = null!;
  public string? Version { get; set; } = null!;
  [Required] public string Status { get; set; } = null!;
  public string? Report { get; set; }
  [Required] public DateTime CompletedAt { get; set; }
}
