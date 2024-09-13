using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecBox.Domain;
using SpecBox.Domain.Model.Users;
using SpecBox.WebApi.Model.Auth;
using SpecBox.WebApi.Services;

namespace SpecBox.WebApi.Controllers;

[ApiController, Route("auth")]
public class AuthController : Controller
{
    private readonly SpecBoxDbContext db;
    private readonly AuthService auth;
    private readonly ILogger logger;
    private readonly IMapper mapper;

    public AuthController(SpecBoxDbContext db, AuthService auth, ILogger<ProjectController> logger, IMapper mapper)
    {
        this.db = db;
        this.auth = auth;
        this.logger = logger;
        this.mapper = mapper;
    }

    /// <summary>
    /// Регистрация новых пользователей
    /// </summary>
    [HttpPost("register", Name = "Register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<AccessTokenModel>> Register([FromBody] UserRegisterModel request)
    {
        var salt = Guid.NewGuid();

        var identity = new UserIdentityModel
        {
            Login = request.Login,
            Salt = salt
        };

        var hasher = new PasswordHasher<UserIdentityModel>();
        var hash = hasher.HashPassword(identity, request.Password);

        var user = new User
        {
            Login = request.Login,
            Email = request.Email,
            Name = request.Name,
            UpdatedAt = DateTime.Now,
            CreatedAt = DateTime.Now,
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
    public async Task<ActionResult<AccessTokenModel>> Login([FromBody] LoginModel request)
    {
        var user = await db.Users.SingleOrDefaultAsync(user => user.Login.ToLower() == request.Login.ToLower());

        if (user == null)
            return Unauthorized("Login Password pair not found");

        var passwordAuth = await db.PasswordAuths.SingleOrDefaultAsync(p => p.UserId == user.Id);

        if (passwordAuth == null)
            return Unauthorized("Login Password pair not found");

        var identity = new UserIdentityModel
        {
            Login = user.Login,
            Salt = passwordAuth.Salt
        };

        var hasher = new PasswordHasher<UserIdentityModel>();
        var result = hasher.VerifyHashedPassword(identity, passwordAuth.Hash, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Login Password pair not found");
        }

        return Json(await auth.CreateTokens(user));
    }

    /// <summary>
    /// Обновление токенов
    /// </summary>
    [HttpPost("refresh", Name = "Refresh tokens")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AccessTokenModel[]>> Refresh([FromBody] RefreshTokenExchangeModel request)
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
