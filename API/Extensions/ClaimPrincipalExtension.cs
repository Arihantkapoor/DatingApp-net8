using System.Security.Claims;

namespace API.Extensions;
public static class ClaimPrincipalExtension
{
    public static string GetUsername(this ClaimsPrincipal user){
        var username = user.FindFirstValue(ClaimTypes.NameIdentifier) 
        ?? throw new Exception("Username can not be found in token");
        return username;
    }
}