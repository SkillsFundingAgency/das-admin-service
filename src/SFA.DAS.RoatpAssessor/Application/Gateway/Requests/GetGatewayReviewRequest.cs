using MediatR;
using System;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetGatewayReviewRequest : IRequest<GetGatewayReviewResponse>
    {
        public GetGatewayReviewRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }

        public Guid ApplicationId { get; }
    }
}
