using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Common;

public class VersionModel
{
    [Required] public string Version { get; set; } = null!;
    [Required] public DateTime UpdatedAt { get; set; }
}
