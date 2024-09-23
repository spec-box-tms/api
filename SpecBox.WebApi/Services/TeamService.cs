using Microsoft.EntityFrameworkCore;
using SpecBox.Domain.Model.Teams;
using SpecBox.WebApi.Lib;

namespace SpecBox.WebApi.Services;

public class TeamService(ApplicationDbContext db)
{
    public async Task<Team?> GetTeamByIdAsync(string id)
    {
        Guid teamId;
        if (!Base32CrockfordDecoder.TryParse(id, out teamId, true))
        {
            return null;
        }

        return await db.Teams.SingleOrDefaultAsync(t => t.Id == teamId && t.DeletedById == null);
    }
}
