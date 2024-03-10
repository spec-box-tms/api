using System.ComponentModel.DataAnnotations;
using SpecBox.WebApi.Model.Common;

namespace SpecBox.WebApi.Model.TestRun;

public class ProjectTestRunsModel
{
  [Required] public ProjectVersionModel Project { get; set; } = null!;
  [Required] public TestRunModel[] TestRuns { get; set; } = null!;
  [Required] public TestRunConfigurationsModel Configurations { get; set; } = null!;
}
