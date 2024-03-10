using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.TestRun;

public class CreateTestRunModel
{
  [Required] public string Title { get; set; } = null!;
  public string? Description { get; set; }

  public string? Environment { get; set; }
  public string? Configuration { get; set; }
}
