using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Project;

public class AssertionGroupModel
{
    [Required] public string Title { get; set; } = null!;
    [Required] public int Order { get; set; }
    [Required] public List<AssertionModel> Assertions { get; } = new();
}
