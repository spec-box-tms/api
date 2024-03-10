using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.TestRun;

public class TestRunConfigurationsModel
{
    [Required] public string[] Configurations { get; set; } = null!;
    [Required] public string[] Environments { get; set; } = null!;
}