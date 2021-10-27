using MediatR;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Queries.GetOrganisation
{
    public class GetOrganisationHandler : IRequestHandler<GetOrganisationQuery, Organisation>
    {
        private IApplicationService _applicationService;

        public GetOrganisationHandler(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public async Task<Organisation> Handle(GetOrganisationQuery request, CancellationToken cancellationToken)
        {
            return await _applicationService.GetOrganisation(request.Id);
        }
    }
}
