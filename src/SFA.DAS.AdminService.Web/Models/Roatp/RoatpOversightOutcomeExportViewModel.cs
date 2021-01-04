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
        

    }
}
