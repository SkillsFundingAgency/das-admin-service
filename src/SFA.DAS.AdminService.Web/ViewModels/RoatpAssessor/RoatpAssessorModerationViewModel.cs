using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor
{
    public class RoatpAssessorModerationViewModel : RoatpAssessorPageViewModel
    {
        public ApplicationReviewStatus Review1Outcome { get; }
        public string Review1Comment { get; }

        public ApplicationReviewStatus Review2Outcome { get; }
        public string Review2Comment { get; }

        public RoatpAssessorModerationViewModel(Guid applicationId, int sequenceNo, int sectionNo, string pageId, Section section, Page page,
            ApplicationReviewStatus review1Outcome, string review1Comment, ApplicationReviewStatus review2Outcome, string review2Comment)
            : base(applicationId, sequenceNo, sectionNo, pageId, section, page)
        {
            Review1Outcome = review1Outcome;
            Review1Comment = review1Comment;
            Review2Outcome = review2Outcome;
            Review2Comment = review2Comment;
        }
    }
}
