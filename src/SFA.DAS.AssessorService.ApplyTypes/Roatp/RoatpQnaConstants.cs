namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    //TODO: Remove after Roatp FHA migration (APR-1823)
    public static class RoatpQnaConstants
    {
        public const string OrganisationTypeEducationalInstitute = "An educational institute";
        public const string OrganisationTypePublicBody = "A public body";

        public static class RoatpSequences
        {
            public static int Preamble = 0;
            public static int YourOrganisation = 1;
            public static int FinancialEvidence = 2;
            public static int CriminalComplianceChecks = 3;
        }

        public static class RoatpSections
        {
            public static class Preamble
            {
                public static int SectionId = 1;
                public static string PageId = "1";

                public static class QuestionIds
                {
                    public static string UKPRN = "PRE-10";
                    public static string LegalName = "PRE-20";
                    public static string TradingName = "PRE-46";
                    public static string UkrlpVerificationCompany = "PRE-56";
                    public static string UkrlpVerificationCharity = "PRE-65";
                    public static string CompanyNumber = "PRE-51";
                    public static string CharityNumber = "PRE-61";
                }
            }

            public static class YourOrganisation
            {
                public static int OrganisationDetails = 2;
                public static int DescribeYourOrganisation = 4;

                public static class PageIds
                {
                    public static string ParentCompanyCheck = "20";
                    public static string ParentCompanyDetails = "21";
                    public static string IcoNumber= "30";
                    public static string Website = "40";
                    public static string TradingForMain = "50";
                    public static string TradingForEmployer = "51";
                    public static string TradingForSupporting = "60";
                    public static string OrganisationTypeMainSupporting = "140";
                    public static string OrganisationTypeEmployer = "150";
                    public static string EducationalInstituteType = "160";
                    public static string PublicBodyType = "170";
                    public static string SchoolType = "180";
                    public static string OrganisationRegisteredESFA = "200";
                    public static string OrganisationFundedESFA = "210";
                }
            }

            public static class FinancialEvidence
            {
                public static int YourOrganisationsFinancialEvidence = 2;
                public static int YourUkUltimateParentCompanysFinancialEvidence = 3;
            }
        }

        public static class QnaQuestionTags
        {
            public const string Ukprn = "UKPRN";
            public const string UKRLPLegalName = "UKRLPLegalName";
            public const string UKRLPVerificationCompanyNumber = "UKRLPVerificationCompanyNumber";
            public const string UKRLPVerificationCharityRegNumber = "UKRLPVerificationCharityRegNumber";

            public const string HasParentCompany = "HasParentCompany";
        }
    }
}