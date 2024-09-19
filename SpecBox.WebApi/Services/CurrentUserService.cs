using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SpecBox.Domain.Model.Users;
using SpecBox.WebApi.Lib;

namespace SpecBox.WebApi.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    public Guid GetUserId()
    {
        var userIdClaim = httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new Exception("User GUID not found in claims.");
        }
        return userIdClaim.Value.FromBase32CrockfordGuid(true);
    }

    public async Task<User> GetUser(ApplicationDbContext db)
    {
        var userId = GetUserId();

        var user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new Exception("User was not found");
        }

        return user;
    }
}