using System.Security.Claims;

namespace SchedulerService.Domain.Extensions;

public static class ClaimsExtensions
{
    public static string GetIdFromClaims(this ClaimsPrincipal user)
    {
        return user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}