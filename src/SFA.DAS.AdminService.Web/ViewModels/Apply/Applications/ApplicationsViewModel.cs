using SFA.DAS.AdminService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class ApplicationsViewModel : PaginationViewModel<ApplicationSummaryItem>
    { 
        public string ReviewStatus { get; set; }

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

        public string StatusColumnValue(string applicationStatus, string applicationReviewStatus)
        {
            string statusColumnValue = applicationStatus;

            if(applicationReviewStatus == ApplicationReviewStatus.HasFeedback)
            {
                if(applicationStatus == ApplicationStatus.FeedbackAdded)
                {
                    statusColumnValue = "Feedback sent";
                }
                else if(applicationStatus == ApplicationStatus.Resubmitted)
                {
                    statusColumnValue = "Feedback received";
                }
            }

            return statusColumnValue;
        }
    }
}
