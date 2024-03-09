using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Project;

public class GraphNodeModel
{
    [Required] public string Id { get; set; } = null!;
    [Required] public string Title { get; set; } = null!;
}
