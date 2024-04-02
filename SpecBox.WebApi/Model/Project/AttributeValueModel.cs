using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Project;

public class AttributeValueModel
{
  [Required] public string Code { get; set; } = null!;
  public string? Title { get; set; }

  [Required] public AttributeModel Attribute { get; set; } = null!;
}
