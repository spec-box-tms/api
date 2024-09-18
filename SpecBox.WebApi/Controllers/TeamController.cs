using System.Security.Claims;
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
public class TeamController(ApplicationDbContext db, IMapper mapper) : Controller
{
    /// <summary>
    /// Список команд авторизованного пользователя
    /// </summary>
    [HttpGet("", Name = "ListTeams")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TeamResponse>> List(CurrentUserService currentUserService)
    {
        var userId = currentUserService.GetUserId();

        var teams = await db.Teams
            .Include(t => t.Users)
            // .Where(t => t.Users.Any(u => u.Id == userId))
            .ToListAsync();

        return Json(mapper.Map<TeamResponse[]>(teams));
    }

    /// <summary>
    /// Список команд авторизованного пользователя
    /// </summary>
    [HttpPost("", Name = "CreateTeam")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TeamResponse>> Create([FromBody] CreateTeamRequest teamRequest)
    {
        var team = new Team
        {
            Code = teamRequest.Code,
            Title = teamRequest.Title,
            Description = teamRequest.Description
        };

        db.Teams.Add(team);
        await db.SaveChangesAsync();

        return Json(mapper.Map<TeamResponse>(team));
    }
}