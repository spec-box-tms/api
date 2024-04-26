using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Lib;

namespace SpecBox.Domain.Model;

[Table("TestResult")]
public class TestResult
{
  public Guid Id { get; set; }

  public string Status { get; set; } = null!;
  public string? Report { get; set; }
  [IsUtc]
  public DateTime? StartedAt { get; set; }
  [IsUtc]
  public DateTime? CompletedAt { get; set; }
  [IsUtc]
  public DateTime UpdatedAt { get; set; }
  
  public Guid AssertionId { get; set; }
  public Assertion Assertion { get; set; } = null!;
  public Guid TestRunId { get; set; }
  public TestRun TestRun { get; set; } = null!;
}
