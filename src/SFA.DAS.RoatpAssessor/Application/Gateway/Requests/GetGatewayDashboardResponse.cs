using SFA.DAS.RoatpAssessor.Application.Models;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetGatewayDashboardResponse
    {
        public IEnumerable<ReviewSummary> PendingReviews { get; set; }
        public IEnumerable<ReviewSummary> InProgressReviews { get; set; }
        public IEnumerable<ReviewSummary> CompletedReviews { get; set; }
    }
}
