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
    [MinLength(8)]
    [MaxLength(40)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$")]
    public string Password { get; set; } = null!;
}
