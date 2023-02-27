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
        public const string RoatpGatewayAssessorTeam = "GAC";
        public const string RoatpFinancialAssessorTeam = "FHC"; // RoATP FHA
        public const string RoatpApplicationOversightTeam = "AOV";
        public const string RoatpAssessorTeam = "AAC";
        public const string EpaoReportsOnlyTeam = "EPX";
        public const string TribalTeam = "TAD";

        public static bool HasValidRole(this ClaimsPrincipal user)
        {
            return user.IsInRole(CertificationTeam)
                   || user.IsInRole(OperationsTeam)
                   || user.IsInRole(AssessmentDeliveryTeam)
                   || user.IsInRole(ProviderRiskAssuranceTeam)
                   || user.IsInRole(RegisterViewOnlyTeam)
                   || user.IsInRole(RoatpGatewayTeam)
                   || user.IsInRole(RoatpGatewayAssessorTeam)
                   || user.IsInRole(RoatpFinancialAssessorTeam)
                   || user.IsInRole(RoatpAssessorTeam)
                   || user.IsInRole(RoatpApplicationOversightTeam)
                   || user.IsInRole(EpaoReportsOnlyTeam)
                   || user.IsInRole(TribalTeam);
        }
    }
}
