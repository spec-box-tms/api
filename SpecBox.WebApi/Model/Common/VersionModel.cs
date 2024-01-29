using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Common;

public class VersionModel
{
    public string? Version { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
