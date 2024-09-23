
using System.ComponentModel.DataAnnotations;
using SpecBox.Domain.Lib;

namespace SpecBox.WebApi.Model.Teams;

public class TeamResponse : IConcurrencyControl
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public Guid CreatedById { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
    [Required]
    public Guid UpdatedById { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedById { get; set; }
    [Required]
    public Guid RowVersion { get; set; }
}
