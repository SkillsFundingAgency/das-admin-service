namespace SFA.DAS.AdminService.Web.Domain
{
    using Microsoft.Extensions.Logging;
    using System.Linq;
    using System.Security.Claims;

    public static class UserExtensions
    {
        static public ILogger<ClaimsPrincipal> Logger { set; get; }

        public static string UserDisplayName(this ClaimsPrincipal user)
        {
            var givenNameClaim = user.GivenName();
            var surnameClaim = user.Surname();

            return $"{givenNameClaim} {surnameClaim}";
        }

        public static string GivenName(this ClaimsPrincipal user)
        {
            var identity = user.Identities.FirstOrDefault();
            var givenNameClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
            var givenName = identity?.Claims.FirstOrDefault(x => x.Type == givenNameClaim);

            if (givenName == null)
            {
                Logger.LogError($"Unable to get value of claim {givenNameClaim}");
            }

            return givenName?.Value ?? "Unknown";
        }

        public static string Surname(this ClaimsPrincipal user)
        {
            var identity = user.Identities.FirstOrDefault();
            var surnameClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
            var surname = identity?.Claims.FirstOrDefault(x => x.Type == surnameClaim);
            if (surname == null)
            {
                Logger.LogError($"Unable to get value of claim {surnameClaim}");
            }

            return surname?.Value ?? "User";
        }
    }
}