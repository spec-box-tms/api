namespace SpecBox.WebApi.Model.Auth;

public class AccessTokenModel
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
