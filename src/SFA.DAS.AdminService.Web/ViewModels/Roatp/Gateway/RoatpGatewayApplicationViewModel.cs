using SFA.DAS.AdminService.Web.ViewModels.Roatp.Applications;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway
{
    public class RoatpGatewayApplicationViewModel : OrganisationDetailsViewModel
    {
        public Guid Id { get; }
        public Guid ApplicationId { get; set; }
        public Guid OrgId { get; }

        public string ApplicationStatus { get; }
        public string GatewayReviewStatus { get; set; }

        public bool IsGatewayApproved { get; set; }

        public List<GatewaySequence> Sequences { get; set; }
        public bool ReadyToConfirm { get; set; }



        public RoatpGatewayApplicationViewModel(AssessorService.ApplyTypes.Roatp.Apply.Apply application)
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
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
                Ukprn = application.ApplyData.ApplyDetails.UKPRN;
                OrganisationName = application.ApplyData.ApplyDetails.OrganisationName;
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
            }
        }
    }

    public class GatewaySequence
    {
        public int SequenceNumber { get; set; }
        public string SequenceTitle { get; set; }
        public List<GatewaySection> Sections { get; set; }
    }

    public class GatewaySection
    {
        public int SectionNumber { get; set; }
        public string PageId { get; set; }
        public string HiddenText { get; set; }
        public string LinkTitle { get; set; }
        public string Status { get; set; }       
    }
}
