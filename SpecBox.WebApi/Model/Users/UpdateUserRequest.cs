using System.ComponentModel.DataAnnotations;
using SpecBox.Domain.Lib;

namespace SpecBox.WebApi.Model.Users;

public class UpdateUserRequest : IConcurrencyControl
{
    [MaxLength(255)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(255)]
    public string? Name { get; set; }
    
    [MaxLength(255)]
    public string? Description { get; set; }
    
    [Required]
    public Guid RowVersion { get; set; }
}
