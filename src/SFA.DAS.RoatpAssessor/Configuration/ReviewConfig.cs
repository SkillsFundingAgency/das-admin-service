using System;

namespace SFA.DAS.RoatpAssessor.Configuration
{
    public static class ReviewConfig
    {
        public static class InitialChecks
        {
            public static class Ukprn
            {
                public const string LegalNameCheck = "LegalNameCheck";
                public const string StatusCheck = "StatusCheck";
                public const string AddressCheck = "AddressCheck";

                public static OutcomeConfig Outcome => new OutcomeConfig(Guid.Parse("44e6c5c7-89ad-447d-a01c-08d769b4a319"), "1", "UKPRN");
            }
        }

        public static class OrganisationInformation
        {
            public static class Website
            {
                public static OutcomeConfig Outcome => new OutcomeConfig(Guid.Parse("49542005-7047-454d-a01f-08d769b4a319"), "40", "YO-40");
            }
        }
    }

    public class OutcomeConfig
    {
        public Guid SectionId { get; }
        public string PageId { get; }
        public string QuestionId { get; }

        public OutcomeConfig(Guid sectionId, string pageId, string questionId)
        {
            SectionId = sectionId;
            PageId = pageId;
            QuestionId = questionId;
        }
    }
}
