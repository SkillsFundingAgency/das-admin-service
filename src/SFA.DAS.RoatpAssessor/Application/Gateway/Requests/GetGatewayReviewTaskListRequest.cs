using MediatR;
using System;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetGatewayReviewTaskListRequest : IRequest<GetGatewayReviewTaskListResponse>
    {
        public Guid ApplicationId { get; }

        public GetGatewayReviewTaskListRequest(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }
}
