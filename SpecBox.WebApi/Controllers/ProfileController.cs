using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecBox.WebApi.Model.Users;
using SpecBox.WebApi.Services;

namespace SpecBox.WebApi.Controllers;

[Authorize]
[ApiController, Route("profile")]
public class ProfileController(ApplicationDbContext db, IMapper mapper) : Controller
{
    /// <summary>
    /// Профиль пользователя
    /// </summary>
    [HttpGet(Name = "GetProfile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponse>> Profile(CurrentUserService currentUserService)
    {
        var user = await currentUserService.GetUser(db);

        return Json(mapper.Map<UserResponse>(user));
    }

    /// <summary>
    /// Изменить профиль пользователя
    /// </summary>
    [HttpPatch(Name = "UpdateProfile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> UpdateProfile([FromBody] UpdateUserRequest request, CurrentUserService currentUserService)
    {
        var user = await currentUserService.GetUser(db);

        try
        {
            if (EntityUpdater.ApplyChanges(user, request))
                await db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict();
        }

        return Json(mapper.Map<UserResponse>(user));
    }
}
