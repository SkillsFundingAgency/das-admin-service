using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;

namespace SFA.DAS.AdminService.Web.Helpers.Roatp
{
    public static class ApplicationReviewHelpers
    {
        public static string ApplicationBacklinkAction()
        {
            return nameof(RoatpApplicationController.OpenApplications);
        }
    }
}