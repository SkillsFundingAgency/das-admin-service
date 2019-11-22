using System.Security.Claims;

namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetId(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn").Value;
        }
    }
}
