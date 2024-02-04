using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.TestRun;

public class TestResultModel
{
  [Required] public Guid Id { get; set; }

  [Required] public string Status { get; set; } = null!;
  public string? Report { get; set; }
  public DateTime? CompletedAt { get; set; }

  public string AssertionTitle { get; set; } = null!;
  public string AssertionGroupTitle { get; set; } = null!;
  public string FeatureCode { get; set; } = null!;
  public string FeatureTitle { get; set; } = null!;
}
