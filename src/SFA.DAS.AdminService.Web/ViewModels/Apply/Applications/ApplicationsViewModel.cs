using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class ApplicationsViewModel
    {
        public PaginatedList<ApplicationSummaryItem> Applications { get; set; }

        public int ApplicationsPerPage { get; set; }

        public string SortColumn { get; set; }

        public string SortDirection { get; set; }

        public int PageIndex { get; set; }

        public string ReviewStatus { get; set; }

        public string Fragment { get; set; }

        public string Title { get; set; }

        public string ChangePageAction { get; set; }

        public string SortColumnAction { get; set; }

        public string ChangeApplicationsPerPageAction { get; set; }

        public List<SelectListItem> ApplicationsPerPageList { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "10", Text = "10" },
            new SelectListItem { Value = "50", Text = "50" },
            new SelectListItem { Value = "100", Text = "100"  },
            new SelectListItem { Value = "500", Text = "500"  }
        };

        public string DateSortColumnNameForReviewStatus()
        {
            string sortColumnName = string.Empty;

            if (ReviewStatus == ApplicationReviewStatus.New || ReviewStatus == ApplicationReviewStatus.InProgress)
            {
                sortColumnName = nameof(ApplicationSummaryItem.SubmittedDate);
            }
            else if (ReviewStatus == ApplicationReviewStatus.HasFeedback)
            {
                sortColumnName = nameof(ApplicationSummaryItem.FeedbackAddedDate);
            }
            else if (ReviewStatus == ApplicationReviewStatus.Approved)
            {
                sortColumnName = nameof(ApplicationSummaryItem.ClosedDate);
            }

            return sortColumnName;
        }

        public string DateHeaderColumnNameForReviewStatus()
        {
            string applicationColumnName = string.Empty;

            if (ReviewStatus == ApplicationReviewStatus.New)
            {
                applicationColumnName = "Date submitted";
            }
            else if (ReviewStatus == ApplicationReviewStatus.InProgress || ReviewStatus == ApplicationReviewStatus.HasFeedback)
            {
                applicationColumnName = "Date";
            }
            else if (ReviewStatus == ApplicationReviewStatus.Approved)
            {
                applicationColumnName = "Date approved";
            }

            return applicationColumnName;
        }
    }
}
