using Microsoft.EntityFrameworkCore;

namespace SpecBox.WebApi.Services;

public class TeamUserService(ApplicationDbContext db, CurrentUserService currentUserService)
{
    public async Task<bool> IsUserTeamAdminAsync(Guid teamId, Guid userId)
    {
        var teamUser = await db.TeamUsers.SingleOrDefaultAsync(tu =>
            tu.TeamId == teamId &&
            tu.UserId == userId);

        if (teamUser == null)
        {
            return false;
        }

        return teamUser.IsAdmin;
    }

    public async Task<bool> IsUserTeamMemberAsync(Guid teamId, Guid userId)
    {
        var teamUser = await db.TeamUsers.SingleOrDefaultAsync(tu =>
            tu.TeamId == teamId &&
            tu.UserId == userId);

        return teamUser != null;
    }

    public async Task<bool> IsCurrentUserTeamAdminAsync(Guid teamId) 
    {
        return await IsUserTeamAdminAsync(teamId, currentUserService.GetUserId());
    }

    public async Task<bool> IsCurrentUserTeamMemberAsync(Guid teamId) 
    {
        return await IsUserTeamMemberAsync(teamId, currentUserService.GetUserId());
    }
}
