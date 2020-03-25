using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public static class GatewayPageIds
    {
        public const string LegalName = "LegalName";
        public const string TradingName = "TradingName";
        public const string OrganisationStatus = "OrganisationStatus";
        public const string Address  = "Address";
        public const string IcoNumber = "IcoNumber";
        public const string WebsiteAddress = "WebsiteAddress";
        public const string OrganisationRisk = "OrganisationRisk";

        public const string PeopleInControl = "PeopleInControl";
        public const string PeopleInControlRisk = "PeopleInControlRisk";

        public const string Roatp = "Roatp";
        public const string Repao = "Repao";

        public const string OfficeForStudents = "OfficeForStudents";
        public const string InitialTeacherTraining = "InitialTeacherTraining";
        public const string Ofsted = "Ofsted";
        public const string SubcontractorDeclaration = "SubcontractorDeclaration";

        public static class CriminalComplianceOrganisationChecks
        {
            public const string CompositionCreditors = "CompositionWithCreditors";
            public const string FailedToRepayFunds = "PayBack";
            public const string ContractTermination = "ContractTerm";
            public const string ContractWithdrawnEarly = "Withdrawn";
            public const string Roto = "Roto";
            public const string FundingRemoved = "FundingRemoved";
            public const string RemovedProfessionalRegister = "RemovedProfessionalRegister";
            public const string IttAccreditation = "IttAccreditation";
            public const string RemovedCharityRegister = "RemovedCharityRegister";
            public const string Safeguarding = "Safeguarding";
            public const string Whistleblowing = "Whistleblowing";
            public const string Insolvency = "Insolvency";
        }

    }
}
