using Microsoft.EntityFrameworkCore;
using SpecBox.Domain.Model.Users;

namespace SpecBox.Domain;

public partial class SpecBoxDbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public DbSet<PasswordAuth> PasswordAuths { get; set; } = null!;

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
}
