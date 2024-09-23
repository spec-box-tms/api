using System.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecBox.Domain.Model.Teams;
using SpecBox.WebApi.Model.Teams;
using SpecBox.WebApi.Services;

namespace SpecBox.WebApi.Controllers;

[Authorize]
[ApiController, Route("teams")]
public class TeamController(TeamService teamService, TeamUserService teamUserService, ApplicationDbContext db, IMapper mapper) : Controller
{
    /// <summary>
    /// Список команд авторизованного пользователя
    /// </summary>
    [HttpGet(Name = "ListTeams")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TeamResponse[]>> List(CurrentUserService currentUserService)
    {
        var userId = currentUserService.GetUserId();

        var teams = await db.Teams
            .Include(t => t.TeamUsers).ThenInclude(tu => tu.User)
            .Where(t => t.DeletedAt == null && t.TeamUsers.Any(u => u.UserId == userId))
            .AsNoTracking()
            .ToListAsync();

        return Json(mapper.Map<TeamResponse[]>(teams));
    }

    /// <summary>
    /// Создать команду
    /// </summary>
    [HttpPost(Name = "CreateTeam")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TeamResponse>> Create([FromBody] CreateTeamRequest teamRequest, CurrentUserService currentUserService)
    {
        var user = await currentUserService.GetUser(db);

        var team = new Team
        {
            Title = teamRequest.Title,
            Description = teamRequest.Description
        };

        var teamUser = new TeamUser
        {
            Team = team,
            User = user,
            IsAdmin = true
        };

        db.Teams.Add(team);
        db.TeamUsers.Add(teamUser);

        await db.SaveChangesAsync();

        return Json(mapper.Map<TeamResponse>(team));
    }

    /// <summary>
    /// Изменить команду
    /// </summary>
    [HttpPatch("{id}", Name = "UpdateTeam")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TeamResponse>> Update(
        string id,
        [FromBody] UpdateTeamRequest teamRequest)
    {
        var team = await teamService.GetTeamByIdAsync(id);

        if (team == null)
            return NotFound();

        if (!await teamUserService.IsCurrentUserTeamAdminAsync(team.Id))
            return Forbid();

        try
        {
            if (EntityUpdater.ApplyChanges(team, teamRequest))
                await db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict();
        }

        return Json(mapper.Map<TeamResponse>(team));
    }

    /// <summary>
    /// Удалить команду
    /// </summary>
    [HttpDelete("{id}", Name = "DeleteTeam")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TeamResponse>> Delete(
        string id)
    {
        var team = await teamService.GetTeamByIdAsync(id);

        if (team == null)
            return NotFound();

        if (!await teamUserService.IsCurrentUserTeamAdminAsync(team.Id))
            return Forbid();

        db.SoftDelete(team);
        await db.SaveChangesAsync();

        return Json(mapper.Map<TeamResponse>(team));
    }
}
