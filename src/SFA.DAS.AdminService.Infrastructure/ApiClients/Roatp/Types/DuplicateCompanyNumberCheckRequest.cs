namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types
{
    using System;
    using MediatR;

    public class DuplicateCompanyNumberCheckRequest : IRequest
    {
        public Guid OrganisationId { get; set; }
        public string CompanyNumber { get; set; }
    }
}
