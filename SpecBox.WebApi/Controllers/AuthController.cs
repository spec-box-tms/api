using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecBox.Domain.Model.Users;
using SpecBox.WebApi.Model.Auth;
using SpecBox.WebApi.Services;

namespace SpecBox.WebApi.Controllers;

[ApiController, Route("auth")]
public class AuthController(ApplicationDbContext db, AuthService auth) : Controller
{
    /// <summary>
    /// Регистрация новых пользователей
    /// </summary>
    [HttpPost("register", Name = "Register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccessTokenResponse>> Register([FromBody] UserRegisterRequest request)
    {
        if (await db.Users.AnyAsync(u => u.Login == request.Login))
        {
            ModelState.AddModelError(nameof(UserRegisterRequest.Login), "Already exists");
            return ValidationProblem();
        }
        if (await db.Users.AnyAsync(u => u.Email == request.Email))
        {
            ModelState.AddModelError(nameof(UserRegisterRequest.Email), "Already exists");
            return ValidationProblem();
        }

        var salt = Guid.NewGuid();

        var identity = new UserIdentity
        {
            Login = request.Login,
            Salt = salt
        };

        var hasher = new PasswordHasher<UserIdentity>();
        var hash = hasher.HashPassword(identity, request.Password);

        var user = new User
        {
            Login = request.Login,
            Email = request.Email,
            Name = request.Name,
        };

        var passwordAuth = new PasswordAuth
        {
            User = user,
            Hash = hash,
            Salt = salt,
            UpdatedAt = DateTime.Now
        };

        await using var tran = await db.Database.BeginTransactionAsync();

        db.Users.Add(user);
        db.PasswordAuths.Add(passwordAuth);

        await db.SaveChangesAsync();
        await tran.CommitAsync();

        return Json(await auth.CreateTokens(user));
    }

    /// <summary>
    /// Аутентификация пользователя
    /// </summary>
    [HttpPost("login", Name = "Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AccessTokenResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await db.Users.SingleOrDefaultAsync(user => user.Login.ToLower() == request.Login.ToLower());

        if (user == null)
            return Unauthorized();

        var passwordAuth = await db.PasswordAuths.SingleOrDefaultAsync(p => p.UserId == user.Id);

        if (passwordAuth == null)
            return Unauthorized();

        var identity = new UserIdentity
        {
            Login = user.Login,
            Salt = passwordAuth.Salt
        };

        var hasher = new PasswordHasher<UserIdentity>();
        var result = hasher.VerifyHashedPassword(identity, passwordAuth.Hash, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized();
        }

        return Json(await auth.CreateTokens(user));
    }

    /// <summary>
    /// Обновление токенов
    /// </summary>
    [HttpPost("refresh", Name = "Refresh tokens")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AccessTokenResponse>> Refresh([FromBody] RefreshTokenExchangeRequest request)
    {
        try
        {
            return Json(await auth.Exchange(request.RefreshToken));
        }
        catch
        {
            return Unauthorized("Refresh token is invalid");
        }
    }
}
