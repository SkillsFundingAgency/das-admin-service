using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public class RoatpApplicationOversightDownloadItem
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }

        public string Ukprn { get; set; }
        public string ProviderRoute { get; set; }
        public string OrganisationName { get; set; }
        public string ApplicationReferenceNumber { get; set; }
        public DateTime ApplicationSubmittedDate { get; set; }
    }
}
