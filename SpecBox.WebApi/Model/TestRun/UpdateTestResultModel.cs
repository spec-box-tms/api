using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.TestRun;

public class UpdateTestResultModel
{
  [Required] public string Status { get; set; } = null!;
  public string? Report { get; set; }
  public DateTime updatedAt { get; set; }
}
