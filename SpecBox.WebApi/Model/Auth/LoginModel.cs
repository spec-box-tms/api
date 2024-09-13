using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Auth;

public class LoginModel
{
  [Required]
  [MaxLength(255)]
  public string Login { get; set; } = null!;

  [Required]
  public string Password { get; set; } = null!;
}
