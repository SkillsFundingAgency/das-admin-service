using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public static class RoatpGatewayConstants
    {
        public const string ProviderContactDetailsTypeLegalIdentifier = "L";

        public static class Captions
        {
            public static string OrganisationChecks = "Organisation checks";

            public static string PeopleInControlChecks = "People in control checks";
         
            public static string ExperienceAndAccreditation = "Experience and accreditation";
            public static string OrganisationsCriminalAndComplianceChecks = "Organisation’s criminal and compliance checks";

            public static string RegisterChecks = "Register checks";
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
            public static string PeopleInControl = "People in control check";
            public static string PeopleInControlHighRisk = "People in control high risk check";

            public static string OfficeForStudents = "";
            public static string InitialTeacherTraining = "";
            public static string Ofsted = "";
            public static string SubcontractorDeclaration = "Subcontractor declaration check";

            public static string Roatp = "RoATP check";
            public static string Roepao = "Register of end-point assessment organisations (EPAO) check";
        }

        public static class PeopleInControl
        {
            public static class Heading
            {
                public static string CompanyDirectors = "Company directors";
                public static string PeopleWithSignificantControl = "People with significant control (PSC's)";
                public static string Trustees = "Trustees";
                public static string WhosInControl = "Who's in control";
            }

            public static class Caption
            {
                public static string CompaniesHouse = "Companies House data";
                public static string CharityCommission = "Charity Commission data";
            }
        }
    }

    public static class NoSelectionErrorMessages
    {
        public static string LegalName = "Select the outcome for legal name check";
        public static string TradingName = "Select the outcome for trading name check";
        public static string OrganisationStatusCheck = "Select the outcome for organisation status check";
        public static string AddressCheck = "Select the outcome for address check";
        public static string PeopleInControlCheck = "Select the outcome for people in control check";
        public static string PeopleInControlHighRiskCheck = "Select the outcome for people in control high risk check";

        public static string IcoNumber = "Select the outcome for ICO registration number check";
        public static string Website = "Select the outcome for website address check";
        public static string OrganisationRisk = "Select the outcome for organisation high risk check";

        public static string OfficeForStudents = "Select the outcome for Office for Students(OfS) check";
        public static string InitialTeacherTraining = "Select the outcome for initial teacher training(ITT) check";
        public static string Ofsted = "Select the outcome for Ofsted check";
        public static string SubcontractorDeclaration = "Select the outcome for subcontractor declaration check";

        public static string Roatp = "Select the outcome for the RoATP check";
        public static string Roepao = "Select the outcome for the register of end-point assessment organisations check";
    }
}
