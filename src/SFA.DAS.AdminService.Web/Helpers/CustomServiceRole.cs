using SFA.DAS.DfESignIn.Auth.Interfaces;
using System.Security.Claims;

namespace SFA.DAS.AdminService.Web.Helpers
{
    public class CustomServiceRole : ICustomServiceRole
    {
        public string RoleClaimType => ClaimTypes.Role;
    }
}
