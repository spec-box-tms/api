using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpecBox.Domain;
using SpecBox.Domain.Model.Users;
using SpecBox.WebApi.Lib;
using SpecBox.WebApi.Model.Auth;

namespace SpecBox.WebApi.Services;

public class AuthService
{
    private readonly ApplicationDbContext db;
    private readonly SymmetricSecurityKey privateKey;

    public AuthService(ApplicationDbContext db, IConfiguration configuration)
    {
        this.db = db;
        var privateKey = configuration["PrivateKey"] ?? "MySuperSecretPrivateKeyWithLengthMoreThan128bits";
        this.privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));
    }

    public async Task<AccessTokenResponse> Exchange(string refreshToken)
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

        await RemoveExpiredTokens();

        var existingToken = await db.RefreshTokens.Include(t => t.User)
            .Where(t => t.Token == refreshToken).ToArrayAsync();

        if (existingToken.Length == 0)
        {
            throw new Exception("Use of expired refresh token");
        }

        foreach (var token in existingToken)
        {
            if (!token.UsedAt.HasValue)
                token.UsedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();

        return await CreateTokens(existingToken[0].User);
    }

    public async Task<AccessTokenResponse> CreateTokens(User user)
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

        return new AccessTokenResponse
        {
            AccessToken = accessTokenStr,
            RefreshToken = refreshTokenStr
        };
    }

    private async Task RemoveExpiredTokens()
    {
        db.RefreshTokens.RemoveRange(
            db.RefreshTokens.Where(t =>
                t.ExpireAt < DateTime.UtcNow ||
                t.UsedAt < DateTime.UtcNow.AddSeconds(-60)
            ));
        await db.SaveChangesAsync();
    }
    private ClaimsIdentity GenerateClaims(User user)
    {
        var ci = new ClaimsIdentity();

        ci.AddClaim(new Claim(ClaimTypes.Name, user.Login));
        ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToBase32Crockford(true)));

        return ci;
    }
}
