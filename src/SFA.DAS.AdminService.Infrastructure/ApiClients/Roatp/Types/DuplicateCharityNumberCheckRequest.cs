namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types
{
    using System;
    using MediatR;

    public class DuplicateCharityNumberCheckRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public string CharityNumber { get; set; }
    }
}
