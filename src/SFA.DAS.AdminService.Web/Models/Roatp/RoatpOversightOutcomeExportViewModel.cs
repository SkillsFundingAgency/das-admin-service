using System;

namespace SFA.DAS.AdminService.Web.Models.Roatp
{
    public class RoatpOversightOutcomeExportViewModel
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }

        public DateTime ApplicationSubmittedDate { get; set; }
        public string ApplicationReferenceNumber { get; set; }
        public string Ukprn { get; set; }
        public string OrganisationName { get; set; }
        public string ProviderRoute { get; set; }
        public bool IsOnRegister { get; set; }
        public string ProviderRouteNameOnRegister { get; set; }
        public string CompanyNumber { get; set; }
        public string OrganisationType { get; set; }
        public string Address { get; set; }
        public string GatewayOutcome { get; set; }
        public string AssessorOutcome { get; set; }
        public string FHCOutcome { get; set; }
        public string OverallOutcome { get; set; }

    }
}
