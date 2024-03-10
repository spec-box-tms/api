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

  [Required] public int TotalCount {get;set;}
  [Required] public int PassedCount {get;set;}
  [Required] public int FailedCount {get;set;}
  [Required] public int BlockedCount {get;set;}
  [Required] public int InvalidCount {get;set;}
  [Required] public int SkippedCount {get;set;}

  public string? Environment {get;set;}
  public string? Configuration {get;set;}
}
