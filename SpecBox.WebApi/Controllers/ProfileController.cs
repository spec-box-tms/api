using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
}