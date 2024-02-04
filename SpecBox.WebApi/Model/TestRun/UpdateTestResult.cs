using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.TestRun;

public class UpdateTestResult
{
  [Required] public string Status { get; set; } = null!;
  public string? Report { get; set; }
}
