using System.Security.Claims;
using SpecBox.WebApi.Lib;

namespace SpecBox.WebApi.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    public Guid? GetUserId()
    {
        var userIdClaim = httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return null;
        }
        return userIdClaim.Value.FromBase32CrockfordGuid();
    }

}