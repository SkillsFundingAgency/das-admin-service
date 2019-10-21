using MediatR;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using SFA.DAS.RoatpAssessor.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Commands
{
    class StartGatewayReviewHandler : IRequestHandler<StartGatewayReviewCommand>
    {
        private readonly IApplyApiClient _applyApiClient;

        public StartGatewayReviewHandler(IApplyApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;
        }

        public async Task<Unit> Handle(StartGatewayReviewCommand request, CancellationToken cancellationToken)
        {
            await _applyApiClient.UpdateApplicationReviewGatewayReviewAsync(request.ApplicationId, ApplicationReviewStatus.InProgress);
            
            return Unit.Value;
        }
    }
}
