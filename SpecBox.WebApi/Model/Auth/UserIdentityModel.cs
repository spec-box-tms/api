namespace SpecBox.WebApi.Model.Auth;

public class UserIdentityModel
{
  public string Login { get; set; } = null!;
  public Guid Salt { get; set; }
}
