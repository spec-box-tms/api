using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.TestRun;

public class TestRunModel
{
  [Required] public Guid Id { get; set; }
  [Required] public string Title { get; set; } = null!;
  public string? Description { get; set; }
  [Required] public string ProjectCode { get; set; } = null!;
  public string? Version { get; set; } 
  [Required] public DateTime CreatedAt {get;set;}
  public DateTime? StartedAt {get;set;}
  public DateTime? CompletedAt {get;set;}
}
