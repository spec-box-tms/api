using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.TestRun;

public class CreateTestRun
{
  [Required] public string Title { get; set; } = null!;
  public string? Description { get; set; }
}
