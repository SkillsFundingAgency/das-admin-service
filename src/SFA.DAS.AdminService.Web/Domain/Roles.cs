﻿using System.Security.Claims;

namespace SFA.DAS.AdminService.Web.Domain
{
    public static class Roles
    {
        public const string RoleClaimType = "http://service/service";

        public const string CertificationTeam = "EPC";
        public const string OperationsTeam = "EPO";
        public const string AssessmentDeliveryTeam = "EPA"; // AAD
        public const string ProviderRiskAssuranceTeam = "EPR"; // FHA
        public const string RegisterViewOnlyTeam = "EPV";
        public const string RoatpGatewayTeam = "APR";
        public const string RoatpAssessorGateway = "GAC";
        public const string EpaoReportsOnlyTeam = "EPX";

        public static bool HasValidRole(this ClaimsPrincipal user)
        {
            return user.IsInRole(CertificationTeam)
                   || user.IsInRole(OperationsTeam)
                   || user.IsInRole(AssessmentDeliveryTeam)
                   || user.IsInRole(ProviderRiskAssuranceTeam)
                   || user.IsInRole(RegisterViewOnlyTeam)
                   || user.IsInRole(RoatpGatewayTeam)
                   || user.IsInRole(RoatpAssessorGateway)
                   || user.IsInRole(EpaoReportsOnlyTeam);
        }

        public static bool HasRoatpRolesOnly(this ClaimsPrincipal user)
        {
            if ((user.IsInRole(RoatpGatewayTeam) || user.IsInRole(RoatpAssessorGateway))
                && !user.IsInRole(CertificationTeam)
                && !user.IsInRole(OperationsTeam)
                && !user.IsInRole(AssessmentDeliveryTeam)
                && !user.IsInRole(ProviderRiskAssuranceTeam)
                && !user.IsInRole(RegisterViewOnlyTeam)
                && !user.IsInRole(EpaoReportsOnlyTeam))
            {
                return true;
            }

            return false;
        }
    }
}
