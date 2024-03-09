using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Project;

public class GraphEdgeModel
{
    [Required] public string Source { get; set; } = null!;
    [Required] public string Target { get; set; } = null!;
}
