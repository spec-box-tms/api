using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Project;

public class FeatureRelationsModel
{
    [Required] public GraphNodeModel[] Nodes { get; set; } = null!;
    [Required] public GraphEdgeModel[] Edges { get; set; } = null!;
}
