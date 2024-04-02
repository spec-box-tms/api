using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Project;

public class AttributeModel
{
  [Required] public string Code { get; set; } = null!;
  public string? Title { get; set; }
}
