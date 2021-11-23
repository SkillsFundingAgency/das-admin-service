using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    //TODO: Remove after Roatp FHA migration (APR-1823)
    public class RoatpApplicationSummaryItem
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string ApplicationRoute { get; set; }
        public string OrganisationName { get; set; }
        public string Ukprn { get; set; }
        public string ApplicationReferenceNumber { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string ApplicationStatus { get; set; }
        public string GatewayReviewStatus { get; set; }
        public string AssessorReviewStatus { get; set; }
        public string FinancialReviewStatus { get; set; }

        public DateTime? ClarificationRequestedDate { get; set; }
        public DateTime? OutcomeMadeDate { get; set; }
        public string OutcomeMadeBy { get; set; }

        public string ApplicationRouteShortText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ApplicationRoute))
                {
                    return string.Empty;
                }
                var index = ApplicationRoute.IndexOf(' ');
                if (index < 0)
                {
                    return ApplicationRoute;
                }
                return ApplicationRoute.Substring(0, index + 1);
            }
        }
    }
}
