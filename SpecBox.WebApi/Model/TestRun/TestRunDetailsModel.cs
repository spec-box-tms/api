using System.ComponentModel.DataAnnotations;
using SpecBox.WebApi.Model.Common;

namespace SpecBox.WebApi.Model.TestRun;

public class TestRunDetailsModel
{
  [Required] public ProjectVersionModel Project { get; set; } = null!;
  [Required] public TestRunModel TestRun { get; set; } = null!;
}
