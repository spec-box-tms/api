using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Auth;

public class UserRegisterModel
{
  [Required]
  [MaxLength(255)]
  public string Login { get; set; } = null!;

  [Required]
  [MaxLength(255)]
  public string Email { get; set; } = null!;

  [Required]
  [MaxLength(255)]
  public string Name { get; set; } = null!;

  [Required]
  public string Password { get; set; } = null!;
}
