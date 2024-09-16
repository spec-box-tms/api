using System.ComponentModel.DataAnnotations;

namespace SpecBox.WebApi.Model.Auth;

public class AccessTokenModel
{
    [Required]
    public string AccessToken { get; set; } = null!;
    [Required]
    public string RefreshToken { get; set; } = null!;
}
