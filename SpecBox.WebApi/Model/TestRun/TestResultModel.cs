using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.TestRun;

public class TestResultModel
{
  [Required] public Guid Id { get; set; }

  [Required] public string Status { get; set; } = null!;
  public string? Report { get; set; }
  [Required] public DateTime UpdatedAt { get; set; }
  public DateTime? StartedAt { get; set; }
  public DateTime? CompletedAt { get; set; }

  [Required] public string AssertionTitle { get; set; } = null!;
  [Required] public string AssertionGroupTitle { get; set; } = null!;
  [Required] public string FeatureCode { get; set; } = null!;
  [Required] public string FeatureTitle { get; set; } = null!;
}
