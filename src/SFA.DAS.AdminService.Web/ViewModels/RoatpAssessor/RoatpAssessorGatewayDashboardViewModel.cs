using SFA.DAS.RoatpAssessor.Application.Models;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor
{
    public class RoatpAssessorGatewayDashboardViewModel
    {
        public IEnumerable<ReviewSummary> PendingReviews { get; set; }
        public IEnumerable<ReviewSummary> InProgressReviews { get; set; }
        public IEnumerable<ReviewSummary> CompletedReviews { get; set; }
    }
}
