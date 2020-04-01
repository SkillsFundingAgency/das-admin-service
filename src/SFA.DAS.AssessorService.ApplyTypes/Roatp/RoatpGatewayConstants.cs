using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public static class RoatpGatewayConstants
    {
        public const string ProviderContactDetailsTypeLegalIdentifier = "L";

        public static class Captions
        {
            public static string OrganisationChecks = "Organisation checks";

            public static string ExperienceAndAccreditation = "Experience and accreditation";
            public static string OrganisationsCriminalAndComplianceChecks = "Organisation’s criminal and compliance checks";
        }

        public static class Headings
        {
            public static string LegalName = "Legal name check";
            public static string TradingName = "Trading name check";
            public static string OrganisationStatusCheck = "Organisation status check";
            public static string AddressCheck = "Address check";
            public static string IcoNumber = "Information Commissioner's Office (ICO) registration number check";
            public static string Website = "Website address check";
            public static string OrganisationRisk = "Organisation high risk check";

            public static string OfficeForStudents = "";
            public static string InitialTeacherTraining = "";
            public static string Ofsted = "";
            public static string SubcontractorDeclaration = "Subcontractor declaration check";

            //public static string CompositionCreditors = "CompositionWithCreditors";
            //public static string FailedToRepayFunds = "PayBack";
            //public static string ContractTermination = "ContractTerm";
            //public static string ContractWithdrawnEarly = "Withdrawn";
            //public static string Roto = "Roto";
            //public static string FundingRemoved = "FundingRemoved";
            //public static string RemovedProfessionalRegister = "RemovedProfessionalRegister";
            //public static string IttAccreditation = "IttAccreditation";
            //public static string RemovedCharityRegister = "RemovedCharityRegister";
            //public static string Safeguarding = "Safeguarding";
            //public static string Whistleblowing = "Whistleblowing";
            //public static string Insolvency = "Insolvency";
        }
    }

    public static class NoSelectionErrorMessages
    {
        public static string LegalName = "Select the outcome for legal name check";
        public static string TradingName = "Select the outcome for trading name check";
        public static string OrganisationStatusCheck = "Select the outcome for organisation status check";
        public static string AddressCheck = "Select the outcome for address check";
        public static string IcoNumber = "Select the outcome for ICO registration number check";
        public static string Website = "Select the outcome for website address check";
        public static string OrganisationRisk = "Select the outcome for organisation high risk check";

        public static string OfficeForStudents = "Select the outcome for Office for Students(OfS) check";
        public static string InitialTeacherTraining = "Select the outcome for initial teacher training(ITT) check";
        public static string Ofsted = "Select the outcome for Ofsted check";
        public static string SubcontractorDeclaration = "Select the outcome for subcontractor declaration check";

        //public static string CompositionCreditors = "Select the outcome for composition with creditors check";
        //public static string FailedToRepayFunds = "Select the outcome for failed to pay back funds in the last 3 years check";
        //public static string ContractTermination = "Select the outcome for contract terminated early by a public body in the last 3 years check";
        //public static string ContractWithdrawnEarly = "Select the outcome for withdrawn from a contract with a public body in the last 3 years check";
        //public static string Roto = "Select the outcome for Removed from Register of Training Organisations(RoTO) in the last 3 years check";
        //public static string FundingRemoved = "Select the outcome for funding removed from any education bodies in the last 3 years check";
        //public static string RemovedProfessionalRegister = "Select the outcome for removed from any professional or trade registers in the last 3 years check";
        //public static string IttAccreditation = "Select the outcome for Involuntary withdrawal from Initial Teacher Training accreditation in the last 3 years check";
        //public static string RemovedCharityRegister = "Select the outcome for removed from any charity register check";
        //public static string Safeguarding = "Select the outcome for investigated due to safeguarding issues in the last 3 months check";
        //public static string Whistleblowing = "Select the outcome for investigated due to whistleblowing issues in the last 3 months check";
        //public static string Insolvency = "Select the outcome for subject to insolvency or winding up proceedings in the last 3 years check";
    }
}
