using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TasksApi.Controllers
{
    public class BaseApiController : ControllerBase
    {
        protected int UserID => int.Parse(FindClaim(ClaimTypes.NameIdentifier));

        private string FindClaim(string claimName)
        {
            ClaimsIdentity? claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            Claim? claim = claimsIdentity?.FindFirst(claimName);

            if (claim is null)
                return null;
            
            return claim.Value;
        }
    }
}
