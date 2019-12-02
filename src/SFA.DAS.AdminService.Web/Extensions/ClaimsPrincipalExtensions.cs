using System.Security.Claims;

namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetId(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn").Value;
        }

        public static string GetGivenName(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname").Value;
        }

        public static string GetSurname(this ClaimsPrincipal principal)
        {
            return ((ClaimsIdentity)principal.Identity).FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname").Value;
        }

        public static string GetGivenNameAndSurname(this ClaimsPrincipal principal)
        {
            return $"{principal.GetGivenName()} {principal.GetSurname()}";
        }
    }
}
