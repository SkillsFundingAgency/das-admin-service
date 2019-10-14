
using SFA.DAS.QnA.Api.Types.Page;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class PageViewModel
    {
        public Page Page { get; }

        public string Title { get; }

        public Guid ApplicationId { get; }

        public int SequenceNo { get; }

        public int SectionNo { get; }

        public string PageId { get; }

        public string FeedbackMessage { get; set; }

        public PageViewModel(Guid applicationId, int sequenceNo, int sectionNo, string pageId, Page page)
        {
            if (page != null)
            {
                Page = page;
                Title = page.Title;
                ApplicationId = applicationId;
                SequenceNo = sequenceNo;
                SectionNo = sectionNo;
                PageId = page.PageId;
            }
            else
            {
                ApplicationId = applicationId;
                SequenceNo = sequenceNo;
                SectionNo = sectionNo;
                PageId = pageId;
            }
        }
    }
}
