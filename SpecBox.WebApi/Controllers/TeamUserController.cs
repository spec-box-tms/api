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
[ApiController, Route("teams/{teamId}/users")]
public class TeamUserController(TeamService teamService, TeamUserService teamUserService, ApplicationDbContext db, IMapper mapper) : Controller
{
    /// <summary>
    /// Список пользователей команды
    /// </summary>
    [HttpGet(Name = "ListTeamUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TeamUserResponse[]>> ListUsers(string teamId)
    {
        var team = await teamService.GetTeamByIdAsync(teamId);

        if (team == null)
            return NotFound();

        if (!await teamUserService.IsCurrentUserTeamMemberAsync(team.Id))
            return Forbid();

        var teamUsers = await db.TeamUsers
            .Include(t => t.User)
            .Include(t => t.CreatedBy)
            .Include(t => t.UpdatedBy)
            .Where(t => t.TeamId == team.Id)
            .ToListAsync();

        return Json(mapper.Map<TeamUserResponse[]>(teamUsers));
    }

    /// <summary>
    /// Добавить пользователя в команду
    /// </summary>
    [HttpPost(Name = "CreateTeamUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TeamUserResponse>> CreateTeamUser(
        string teamId,
        [FromBody] CreateTeamUserRequest request)
    {
        var team = await teamService.GetTeamByIdAsync(teamId);
        if (team == null)
        {
            return NotFound();
        }

        if (!await teamUserService.IsCurrentUserTeamAdminAsync(team.Id))
            return Forbid();

        var user = await db.Users.SingleOrDefaultAsync(u => u.Login == request.Login);
        if (user == null)
        {
            ModelState.AddModelError(nameof(CreateTeamUserRequest.Login), "User login not found");
            return ValidationProblem();
        }

        var isUserExists = await teamUserService.IsUserTeamMemberAsync(team.Id, user.Id);
        if (isUserExists)
        {
            ModelState.AddModelError(nameof(CreateTeamUserRequest.Login), "User already exists");
            return ValidationProblem();
        }

        var teamUser = new TeamUser
        {
            Team = team,
            User = user,
            IsAdmin = request.IsAdmin
        };
        db.TeamUsers.Add(teamUser);

        await db.SaveChangesAsync();

        teamUser = await db.TeamUsers
            .Include(tu => tu.User)
            .Include(tu => tu.CreatedBy)
            .Include(tu => tu.UpdatedBy)
            .SingleAsync(tu => tu.Id == teamUser.Id);

        return Json(mapper.Map<TeamUserResponse>(teamUser));
    }

    /// <summary>
    /// Изменить пользователя команды
    /// </summary>
    [HttpPatch("{userLogin}", Name = "UpdateTeamUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TeamUserResponse>> UpdateTeamUser(
        string teamId,
        string userLogin,
        [FromBody] UpdateTeamUserRequest request)
    {
        var team = await teamService.GetTeamByIdAsync(teamId);
        if (team == null)
        {
            ModelState.AddModelError(nameof(teamId), "Team was not found");
            return NotFound();
        }

        if (!await teamUserService.IsCurrentUserTeamAdminAsync(team.Id))
            return Forbid();

        var teamUser = await db.TeamUsers.SingleOrDefaultAsync(tu => tu.TeamId == team.Id && tu.User.Login == userLogin);
        if (teamUser == null)
        {
            ModelState.AddModelError(nameof(userLogin), "User login not found");
            return NotFound();
        }

        if (request.IsAdmin == false && !await HasAnyOtherAdmin(team.Id, teamUser.Id))
        {
            ModelState.AddModelError(nameof(UpdateTeamUserRequest.IsAdmin), "Team has to have at least one admin");
            return ValidationProblem();
        }

        teamUser.IsAdmin = request.IsAdmin;

        await db.SaveChangesAsync();

        teamUser = await db.TeamUsers
            .Include(tu => tu.User)
            .Include(tu => tu.CreatedBy)
            .Include(tu => tu.UpdatedBy)
            .SingleAsync(tu => tu.Id == teamUser.Id);

        return Json(mapper.Map<TeamUserResponse>(teamUser));
    }

    /// <summary>
    /// Удалить пользователя команды
    /// </summary>
    [HttpDelete("{userLogin}", Name = "DeleteTeamUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteTeamUser(
        string teamId,
        string userLogin,
        CurrentUserService currentUserService)
    {
        var currentUser = await currentUserService.GetUser(db);

        var team = await teamService.GetTeamByIdAsync(teamId);
        if (team == null)
        {
            ModelState.AddModelError(nameof(teamId), "Team was not found");
            return NotFound();
        }

        var teamUserToRemove = await db.TeamUsers.SingleOrDefaultAsync(tu => tu.User.Login == userLogin && tu.TeamId == team.Id);

        if (teamUserToRemove == null)
        {
            ModelState.AddModelError(nameof(CreateTeamUserRequest.Login), "User login not found");
            return NotFound();
        }

        if (currentUser.Id != teamUserToRemove.UserId && !await teamUserService.IsCurrentUserTeamAdminAsync(team.Id))
            return Forbid();

        if (teamUserToRemove.IsAdmin && !await HasAnyOtherAdmin(team.Id, teamUserToRemove.Id))
        {
            ModelState.AddModelError(nameof(UpdateTeamUserRequest.IsAdmin), "Team has to have at least one admin");
            return ValidationProblem();
        }

        db.TeamUsers.Remove(teamUserToRemove);

        await db.SaveChangesAsync();

        return Ok();
    }

    private async Task<bool> HasAnyOtherAdmin(Guid teamId, Guid exceptId)
    {
        return await db.TeamUsers.AnyAsync(tu => tu.TeamId == teamId && tu.Id != exceptId && tu.IsAdmin);
    }
}
