using Microsoft.EntityFrameworkCore;
using SpecBox.Domain.Model.Teams;

namespace SpecBox.Domain;

public partial class SpecBoxDbContext
{
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<TeamUser> TeamUsers { get; set; } = null!;
}
