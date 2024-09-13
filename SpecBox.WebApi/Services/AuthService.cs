using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpecBox.Domain;
using SpecBox.Domain.Model.Users;
using SpecBox.WebApi.Model.Auth;

namespace SpecBox.WebApi.Services;

public class AuthService
{
    private readonly SpecBoxDbContext db;
    private readonly SymmetricSecurityKey privateKey;

    public AuthService(SpecBoxDbContext db, IConfiguration configuration)
    {
        this.db = db;
        var privateKey = configuration["PrivateKey"] ?? "MySuperSecretPrivateKeyWithLengthMoreThan128bits";
        this.privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));
    }

    public async Task<AccessTokenModel> Exchange(string refreshToken)
    {
        var handler = new JwtSecurityTokenHandler();

        var tokenValidationParams = new TokenValidationParameters
        {
            IssuerSigningKey = privateKey,
            ValidateIssuer = false,
            ValidateAudience = false
        };

        SecurityToken securityToken;
        handler.ValidateToken(refreshToken, tokenValidationParams, out securityToken);

        RemoveExpiredTokens();

        var existingToken = await db.RefreshTokens.Include(t => t.User)
            .SingleOrDefaultAsync(t => t.Token == refreshToken);

        if (existingToken == null)
        {
            throw new Exception("Use of expired refresh token");
        }

        return await CreateTokens(existingToken.User);
    }

    public async Task<AccessTokenModel> CreateTokens(User user)
    {
        var handler = new JwtSecurityTokenHandler();

        var signature = new SigningCredentials(
                            privateKey,
                            SecurityAlgorithms.HmacSha256);

        var accessTokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = signature,
            Expires = DateTime.UtcNow.AddHours(7),
            Subject = GenerateClaims(user)
        };

        var refreshTokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = signature,
            Expires = DateTime.UtcNow.AddDays(10)
        };

        var accessToken = handler.CreateToken(accessTokenDescriptor);
        var refreshToken = handler.CreateToken(refreshTokenDescriptor);

        var accessTokenStr = handler.WriteToken(accessToken);
        var refreshTokenStr = handler.WriteToken(refreshToken);

        db.RefreshTokens.Add(new RefreshToken
        {
            User = user,
            Token = refreshTokenStr,
            ExpireAt = refreshTokenDescriptor.Expires.Value
        });

        await db.SaveChangesAsync();

        return new AccessTokenModel
        {
            AccessToken = accessTokenStr,
            RefreshToken = refreshTokenStr
        };
    }

    private async void RemoveExpiredTokens()
    {
        db.RefreshTokens.RemoveRange(
            db.RefreshTokens.Where(t =>
                t.ExpireAt < DateTime.UtcNow ||
                t.UsedAt < DateTime.UtcNow.AddSeconds(60)
            ));
        await db.SaveChangesAsync();
    }
    private ClaimsIdentity GenerateClaims(User user)
    {
        var ci = new ClaimsIdentity();

        ci.AddClaim(new Claim(ClaimTypes.Name, user.Login));
        ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));

        return ci;
    }
}
