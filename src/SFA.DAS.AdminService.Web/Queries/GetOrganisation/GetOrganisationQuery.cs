using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AdminService.Web.Queries.GetOrganisation
{
    public class GetOrganisationQuery : IRequest<Organisation>
    {
        public Guid Id { get; set; }

        public GetOrganisationQuery(Guid id)
        {
            Id = Id;
        }
    }
}
