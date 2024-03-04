using System.ComponentModel.DataAnnotations.Schema;

namespace SpecBox.Domain.Model;

[Table("TestResult")]
public class TestResult
{
  public Guid Id { get; set; }

  public string Status { get; set; } = null!;
  public string? Report { get; set; }
  public DateTime? StartedAt { get; set; }
  public DateTime? CompletedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  
  public Guid AssertionId { get; set; }
  public Assertion Assertion { get; set; } = null!;
  public Guid TestRunId { get; set; }
  public TestRun TestRun { get; set; } = null!;
}
