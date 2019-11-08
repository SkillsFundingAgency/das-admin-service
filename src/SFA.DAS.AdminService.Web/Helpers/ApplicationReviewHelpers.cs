using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AdminService.Web.Controllers.Apply;

namespace SFA.DAS.AdminService.Web.Helpers
{
    public static class ApplicationReviewHelpers
    {
        public static string TranslateApplicationStatus(string sequenceStatus)
        {
            switch (sequenceStatus)
            {
                case ApplicationSectionStatus.Submitted:
                    return "Not started";
                case ApplicationSectionStatus.InProgress:
                    return "Evaluation started";
                case ApplicationSectionStatus.Evaluated:
                    return "Evaluated";
            }

            return "";
        }

        public static string TranslateFinancialStatus(string financeStatus, string grade)
        {
            switch (financeStatus)
            {
                case FinancialReviewStatus.New:
                    return "Not started";
                case FinancialReviewStatus.InProgress:
                    return "In Progress";
                case FinancialReviewStatus.Graded:
                case FinancialReviewStatus.Approved:
                    switch(grade)
                    {
                        case FinancialApplicationSelectedGrade.Outstanding:
                        case FinancialApplicationSelectedGrade.Good:
                        case FinancialApplicationSelectedGrade.Satisfactory:
                            return "Passed";
                        case FinancialApplicationSelectedGrade.Exempt:
                            return "Exempt";
                        case FinancialApplicationSelectedGrade.Inadequate:
                            return "Rejected";
                    }
                    break;
                case FinancialReviewStatus.Exempt:
                    return "Exempt";
            }

            return "";
        }

        public static string ApplicationBacklinkAction(string reviewStatus)
        {
            switch(reviewStatus)
            {
                case ApplicationReviewStatus.New:
                    return $"ChangePageNewApplications";
                case ApplicationReviewStatus.InProgress:
                    return $"ChangePageInProgressApplications";
                case ApplicationReviewStatus.HasFeedback:
                    return $"ChangePageFeedbackApplications";
                case ApplicationReviewStatus.Approved:
                    return $"ChangePageApprovedApplications";
            }

            return string.Empty;
        }

        public static string ApplicationBacklinkController(string applicationType)
        {
            return $"{applicationType}Application";
        }

        public static string ApplicationFragment(string reviewStatus)
        {
            switch (reviewStatus)
            {
                case ApplicationReviewStatus.New:
                    return "new";
                case ApplicationReviewStatus.InProgress:
                    return "in-progress";
                case ApplicationReviewStatus.HasFeedback:
                    return "feedback";
                case ApplicationReviewStatus.Approved:
                    return "approved";
            }

            return string.Empty;
        }
    }
}