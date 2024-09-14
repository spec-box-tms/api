using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Auth;

public class RefreshTokenExchangeModel
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}
