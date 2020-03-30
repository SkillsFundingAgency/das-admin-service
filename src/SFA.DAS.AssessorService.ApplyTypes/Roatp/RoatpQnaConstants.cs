namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public static class RoatpQnaConstants
    {
        public const string OrganisationTypeEducationalInstitute = "An educational institute";
        public const string OrganisationTypePublicBody = "A public body";

        public static class RoatpSequences
        {
            public static int Preamble = 0;
            public static int YourOrganisation = 1;
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
                public static int DescribeYourOrganisation = 4;
                public static class PageIds
                {
                    public static string OrganisationTypeMainSupporting = "140";
                    public static string OrganisationTypeEmployer = "150";
                    public static string PublicBodyTypeMainSupporting = "170";
                    public static string PublicBodyTypeEmployer = "171";
                    public static string EducationalInstituteTypeMainSupporting = "160";
                    public static string EducationalInstituteTypeEmployer = "161";
                }

                public static class QuestionIds
                {
                    public static string OrganisationTypeMainSupporting = "YO-140";
                    public static string OrganisationTypeEmployer = "YO-150";
                    public static string PublicBodyTypeMainSupporting = "YO-170";
                    public static string PublicBodyTypeEmployer = "YO-171";
                    public static string EducationalInstituteTypeMainSupporting = "YO-160";
                    public static string EducationalInstituteTypeEmployer = "YO-161";
                }
            }
        }

        public static class QnaQuestionTags
        {
            public const string Ukprn = "UKPRN";
            public const string UKRLPLegalName = "UKRLPLegalName";
            public const string UKRLPVerificationCompanyNumber = "UKRLPVerificationCompanyNumber";
            public const string UKRLPVerificationCharityRegNumber = "UKRLPVerificationCharityRegNumber";
        }
    }
}
