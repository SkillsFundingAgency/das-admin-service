using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoatpAssessor.Application.Services;
using SFA.DAS.RoatpAssessor.Services.ApplyApiClient.Models;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Commands
{
    public class CreateGatewayHandler : IRequestHandler<CreateGatewayCommand>
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly ITimeProvider _timeProvider;

        public CreateGatewayHandler(IApplyApiClient applyApiClient, ITimeProvider timeProvider)
        {
            _applyApiClient = applyApiClient;
            _timeProvider = timeProvider;
        }

        public async Task<Unit> Handle(CreateGatewayCommand command, CancellationToken cancellationToken)
        {
            //todo: ensure Application is in correct state and a gateway review doesn't already exist
            //also update Application status from 'Submitted' to 'In Progress'

            var now = _timeProvider.UtcNow;

            var model = new CreateGatewayModel
            {
                Id = Guid.NewGuid(),
                ApplicationId = command.ApplicationId,
                Status = "InProgress",
                ApplicationStatus = "UnderReview",
                CreatedAt = now,
                CreatedBy = command.UserId,
                AssignedAt = now,
                AssignedTo = command.UserId,
                AssignedToName = command.UserName
            };

            await _applyApiClient.CreateGatewayAsync(model);

            return Unit.Value;
        }
    }
}
