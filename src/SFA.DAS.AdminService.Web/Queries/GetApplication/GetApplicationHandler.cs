using AutoMapper;
using MediatR;
using SFA.DAS.AdminService.Web.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Queries.GetApplication
{
    public class GetApplicationHandler : IRequestHandler<GetApplicationQuery, Models.Apply.Application>
    {
        private readonly IApplicationApiClient _applicationApiClient;

        public GetApplicationHandler(IApplicationApiClient applicationApiClient)
        {
            _applicationApiClient = applicationApiClient;
        }

        public async Task<Models.Apply.Application> Handle(GetApplicationQuery request, CancellationToken cancellationToken)
        {
            var application = await _applicationApiClient.GetApplication(request.Id);

            return Mapper.Map<Models.Apply.Application>(application);
        }
    }
}
