namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types
{
    using System;
    using MediatR;

    public class DuplicateUKPRNCheckRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public string UKPRN { get; set; }
    }
}
