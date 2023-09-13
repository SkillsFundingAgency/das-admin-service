using SFA.DAS.DfESignIn.Auth.Interfaces;
using System.Security.Claims;
using SFA.DAS.DfESignIn.Auth.Enums;

namespace SFA.DAS.AdminService.Web.Helpers
{
    public class CustomServiceRole : ICustomServiceRole
    {
        public string RoleClaimType => ClaimTypes.Role;
        public CustomServiceRoleValueType RoleValueType => CustomServiceRoleValueType.Code;
    }
}
