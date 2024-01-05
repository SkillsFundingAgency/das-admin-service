using System;
using MediatR;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types
{
    public class UpdateOrganisationCharityNumberRequest : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
        public string CharityNumber { get; set; }
        public string UpdatedBy { get; set; }
    }
}
