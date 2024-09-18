using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Lib;

namespace SpecBox.Domain.Model.Users;

[Table("RefreshToken")]
public class RefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public string Token { get; set; } = null!;

    [IsUtc]
    public DateTime ExpireAt { get; set; }

    [IsUtc]
    public DateTime? UsedAt { get; set; } = null;
}
