using MediatR;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.RoatpAssessor.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetGatewayReviewTaskListHandler : IRequestHandler<GetGatewayReviewTaskListRequest, GetGatewayReviewTaskListResponse>
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;

        public GetGatewayReviewTaskListHandler(IApplyApiClient applyApiClient, IQnaApiClient qnaApiClient)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
        }

        public async Task<GetGatewayReviewTaskListResponse> Handle(GetGatewayReviewTaskListRequest request, CancellationToken cancellationToken)
        {
            var review = await _applyApiClient.GetApplicationReviewAsync(request.ApplicationId);

            if (!review.GatewayReviewIsInProgress)
            {
                throw new Exception($"Review '{request.ApplicationId}' is not in correct state to do Gateway review");
            }

            throw new NotImplementedException();
        }
    }
}
