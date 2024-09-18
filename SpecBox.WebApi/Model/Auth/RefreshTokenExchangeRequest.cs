using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Auth;

public class RefreshTokenExchangeRequest
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}
