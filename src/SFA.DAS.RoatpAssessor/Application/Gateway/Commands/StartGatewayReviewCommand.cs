using MediatR;
using System;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Commands
{
    public class StartGatewayReviewCommand : IRequest
    {
        public Guid ApplicationId { get; }

        public StartGatewayReviewCommand(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}
