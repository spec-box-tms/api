using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Project.Feature;

public class FeatureResponse
{
    [Required] public string Code { get; set; } = null!;

    [Required] public string Title { get; set; } = null!;

    [Required] public List<AttributeValueModel> Attributes { get; } = new();
}
