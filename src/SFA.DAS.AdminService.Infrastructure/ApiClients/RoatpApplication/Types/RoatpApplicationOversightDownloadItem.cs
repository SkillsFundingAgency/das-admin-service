﻿using System;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types
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
        public string ProviderRouteNameOnRegister { get; set; }
        public string OrganisationType { get; set; }
        public int? OrganisationStatusId { get; set; }
        public string LegalAddress { get; set; }
        public string GatewayOutcome { get; set; }
        public string AssessorOutcome { get; set; }
        public string FHCOutcome { get; set; }
        public string OverallOutcome { get; set; }
        public string CompanyNumber { get; set; }
    }
}
