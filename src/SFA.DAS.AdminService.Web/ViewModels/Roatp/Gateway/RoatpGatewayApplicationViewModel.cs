using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class RoatpGatewayApplicationViewModel
    {
        public Guid Id { get; }
        public Guid ApplicationId { get; set; }
        public Guid OrgId { get; }

        public string ApplicationReference { get; }
        public int ApplicationRoute { get; }

        public string ApplicationStatus { get; }
        public string GatewayReviewStatus { get; }

        public string Ukprn { get; }
        public string OrganisationName { get; }
        
        public DateTime? SubmittedDate { get; }  

        public bool IsGatewayApproved { get; set; }

        public RoatpGatewayApplicationViewModel() { }

        public RoatpGatewayApplicationViewModel(AssessorService.ApplyTypes.Roatp.Apply application)
        {
            Id = application.Id;
            ApplicationId = application.ApplicationId;
            OrgId = application.OrganisationId;

            ApplicationStatus = application.ApplicationStatus;
            GatewayReviewStatus = application.GatewayReviewStatus;

            if (application.GatewayReviewStatus == SFA.DAS.AssessorService.ApplyTypes.Roatp.GatewayReviewStatus.Approved)
            {
                IsGatewayApproved = true;
            }
            else if (application.GatewayReviewStatus == SFA.DAS.AssessorService.ApplyTypes.Roatp.GatewayReviewStatus.Declined)
            {
                IsGatewayApproved = false;
            }

            if (application.ApplyData?.ApplyDetails != null)
            {
                ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRoute;
                Ukprn = application.ApplyData.ApplyDetails.UKPRN;
                OrganisationName = application.ApplyData.ApplyDetails.OrganisationName;
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
            }


        }
    }
}
