using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor
{
    public class RoatpAssessorDashboardIndexViewModel
    {
        public IEnumerable<Guid> SubmittedApplications { get; set; }
        public IEnumerable<Guid> GatewayReviewsPending { get; set; }
        public IEnumerable<Guid> GatewayReviewsUnderReview { get; set; }
        public IEnumerable<Guid> PmoReviewsPending { get; set; }
        public IEnumerable<Guid> PmoReviewsUnderReview { get; set; }
        public IEnumerable<Guid> AssessorReview1Pending { get; set; }
        public IEnumerable<Guid> AssessorReview1UnderReview { get; set; }
        public IEnumerable<Guid> AssessorReview2Pending { get; set; }
        public IEnumerable<Guid> AssessorReview2UnderReview { get; set; }
        public IEnumerable<Guid> AssessorModerationPending { get; set; }
        public IEnumerable<Guid> AssessorModerationUnderReview { get; set; }
    }
}
