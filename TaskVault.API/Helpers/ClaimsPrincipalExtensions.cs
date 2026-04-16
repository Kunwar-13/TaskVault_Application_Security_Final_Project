using System;
using System.Security.Claims;

namespace TaskVault.API.Helpers;

public static class ClaimsPrincipalExtensions
{
   
    public static int GetUserId(this ClaimsPrincipal user)
    {
     
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                 ?? user.FindFirst("sub")
                 ?? throw new UnauthorizedAccessException("User ID claim not found.");

        return int.Parse(claim.Value);
    
    }

}
