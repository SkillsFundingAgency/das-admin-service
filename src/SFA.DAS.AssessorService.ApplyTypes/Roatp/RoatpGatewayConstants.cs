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
            public static string ExperienceAndAccreditation = "Experience and accreditation checks";
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

            public static string OfficeForStudents = "Office for Students (OfS) check";
            public static string InitialTeacherTraining = "Initial teacher training (ITT) check";
            public static string Ofsted = "Ofsted check";
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
        public static Dictionary<string, string> Errors = new Dictionary<string, string>
        {
            { GatewayPageIds.LegalName, "Select the outcome for legal name check" },
            { GatewayPageIds.TradingName, "Select the outcome for trading name check" },
            { GatewayPageIds.OrganisationStatus, "Select the outcome for organisation status check" },
            { GatewayPageIds.Address ,"Select the outcome for address check" },
            { GatewayPageIds.PeopleInControl, "Select the outcome for people in control check" },
            { GatewayPageIds.PeopleInControlRisk, "Select the outcome for people in control high risk check" },
            { GatewayPageIds.IcoNumber, "Select the outcome for ICO registration number check" },
            { GatewayPageIds.WebsiteAddress, "Select the outcome for website address check" },
            { GatewayPageIds.OrganisationRisk, "Select the outcome for organisation high risk check" },
            { GatewayPageIds.OfficeForStudents, "Select the outcome for Office for Students (OfS) check" },
            { GatewayPageIds.InitialTeacherTraining, "Select the outcome for initial teacher training (ITT) check" },
            { GatewayPageIds.Ofsted, "Select the outcome for Ofsted check" },
            { GatewayPageIds.SubcontractorDeclaration, "Select the outcome for subcontractor declaration check" },
            { GatewayPageIds.Roatp, "Select the outcome for the RoATP check" },
            { GatewayPageIds.Roepao, "Select the outcome for the register of end-point assessment organisations check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors, "Select the outcome for composition with creditors check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, "Select the outcome for failed to pay back funds in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination, "Select the outcome for contract terminated early by a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, "Select the outcome for withdrawn from a contract with a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, "Select the outcome for Removed from Register of Training Organisations(RoTO) in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, "Select the outcome for funding removed from any education bodies in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, "Select the outcome for removed from any professional or trade registers in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation, "Select the outcome for Involuntary withdrawal from Initial Teacher Training accreditation in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, "Select the outcome for removed from any charity register check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding,"Select the outcome for investigated due to safeguarding issues in the last 3 months check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Whistleblowing, "Select the outcome for investigated due to whistleblowing issues in the last 3 months check" },
            { GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, "Select the outcome for subject to insolvency or winding up proceedings in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions, "Select the outcome for unspent criminal convictions check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds, "Select the outcome for failed to pay back funds in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities, "Select the outcome for investigated for fraud or irregularities in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation, "Select the outcome for ongoing investigations for fraud or irregularities check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated, "Select the outcome for contract terminated by a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract, "Select the outcome for withdrawn from a contract with a public body in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments, "Select the outcome for breached tax payments or social security contributions in the last 3 years check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees, "Select the outcome for Register of Removed Trustees check" },
            { GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt, "Select the outcome for people in control or any partner organisations been made bankrupt in the last 3 years check" }
        };

    }
}
