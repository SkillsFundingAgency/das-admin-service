using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.RoatpAssessor.Configuration
{
    public static class QuestionsConfig
    {
        public static class OrganisationInformation
        {
            public static QuestionConfig Website => new QuestionConfig(Guid.Parse("49542005-7047-454d-a01f-08d769b4a319"), "40", "YO-41");
        }
    }

    public class QuestionConfig
    {
        public Guid SectionId { get; }
        public string PageId { get; }
        public string QuestionId { get; }

        public QuestionConfig(Guid sectionId, string pageId, string questionId)
        {
            SectionId = sectionId;
            PageId = pageId;
            QuestionId = questionId;
        }
    }
}
