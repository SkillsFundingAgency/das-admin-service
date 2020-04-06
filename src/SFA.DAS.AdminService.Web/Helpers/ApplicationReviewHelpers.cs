using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;

namespace SFA.DAS.AdminService.Web.Helpers
{
    public static class ApplicationReviewHelpers
    {
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

        public static string ApplicationBacklinkAction()
        {
            return nameof(RoatpApplicationController.OpenApplications);
        }
    }
}