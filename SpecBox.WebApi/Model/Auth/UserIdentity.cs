namespace SpecBox.WebApi.Model.Auth;

public class UserIdentity
{
    public required string Login { get; set; } = null!;
    public required Guid Salt { get; set; }
}
