using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor
{
    public class RoatpAssessorReviewViewModel : RoatpAssessorPageViewModel
    {
        public AssessorReviewNo ReviewNo { get; }

        public RoatpAssessorReviewViewModel(Guid applicationId, AssessorReviewNo reviewNo, int sequenceNo, int sectionNo, string pageId, Section section, Page page)
            : base(applicationId, sequenceNo, sectionNo, pageId, section, page)
        {
            ReviewNo = reviewNo;
        }
    }
}
