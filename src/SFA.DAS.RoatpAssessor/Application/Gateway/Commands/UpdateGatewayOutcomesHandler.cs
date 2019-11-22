using MediatR;
using SFA.DAS.RoatpAssessor.Application.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Commands
{
    class UpdateGatewayOutcomesHandler : IRequestHandler<UpdateGatewayOutcomesCommand>
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly ITimeProvider _timeProvider;

        public UpdateGatewayOutcomesHandler(IApplyApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;
        }

        public async Task<Unit> Handle(UpdateGatewayOutcomesCommand command, CancellationToken cancellationToken)
        {
            await _applyApiClient.UpdateGatewayOutcomesAsync(command);

            return Unit.Value;
        }
    }
}
