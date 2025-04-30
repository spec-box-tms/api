using System.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecBox.Domain.Model;
using SpecBox.Domain.Model.Teams;
using SpecBox.WebApi.Model.Common;
using SpecBox.WebApi.Model.Teams;
using SpecBox.WebApi.Services;

namespace SpecBox.WebApi.Controllers;

[Authorize]
[ApiController, Route("teams/{teamId}/projects")]
public class TeamProjectsController(TeamService teamService, TeamUserService teamUserService, ApplicationDbContext db, IMapper mapper) : Controller
{
    /// <summary>
    /// Список проектов команды
    /// </summary>
    [HttpGet(Name = "ListTeamProjects")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectResponse[]>> List(string teamId, CurrentUserService currentUserService)
    {
        var team = await teamService.GetTeamByIdAsync(teamId);

        if (team == null)
            return NotFound();

        if (!await teamUserService.IsCurrentUserTeamMemberAsync(team.Id))
            return Forbid();


        var projects = await db.Projects.Where(p => p.TeamId == team.Id).GroupBy(p => p.Code).Select(g => new ProjectResponse
        {
            Code = g.Key,
            Title = g.First().Title,
            Description = g.First().Description,
            RepositoryUrl = g.First().RepositoryUrl,
            Versions = g.Select(p => new VersionModel
            {
                Version = p.Version,
                UpdatedAt = p.UpdatedAt
            }).OrderBy(v => v.UpdatedAt).ToArray()
        }).ToArrayAsync();

        return Json(projects);
    }

}