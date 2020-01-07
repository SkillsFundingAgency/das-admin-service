using MediatR;
using SFA.DAS.RoatpAssessor.Domain.Entities;
using System;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Commands
{
    public class UpdateGatewayReviewStatusCommand : IRequest
    {
        public Guid ApplicationId { get; }
        public ApplicationReviewStatus Status { get; }

        public UpdateGatewayReviewStatusCommand(Guid applicationId, ApplicationReviewStatus status)
        {
            ApplicationId = applicationId;
            Status = status;
        }
    }
}
