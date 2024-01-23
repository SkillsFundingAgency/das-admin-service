using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types
{
    public class UpdateOrganisationApplicationDeterminedDateRequest
    {

        public DateTime ApplicationDeterminedDate { get; set; }
        public Guid OrganisationId { get; set; }
        public string LegalName { get; set; }
        public string UpdatedBy { get; set; }
    }
}