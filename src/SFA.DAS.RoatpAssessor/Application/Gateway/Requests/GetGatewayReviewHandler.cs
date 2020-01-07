using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.RoatpAssessor.Services;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetGatewayReviewHandler : IRequestHandler<GetGatewayReviewRequest, GetGatewayReviewResponse>
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;

        public GetGatewayReviewHandler(IApplyApiClient applyApiClient, IQnaApiClient qnaApiClient)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
        }

        public async Task<GetGatewayReviewResponse> Handle(GetGatewayReviewRequest request, CancellationToken cancellationToken)
        {
            var review = await _applyApiClient.GetApplicationReviewAsync(request.ApplicationId);

            if (!review.GatewayReviewIsInProgress)
            {
                throw new Exception($"Review '{request.ApplicationId}' is not in correct state to do Gateway review");
            }

            return new GetGatewayReviewResponse
            {
                ApplicationId = review.ApplicationId
            };
        }
    }
}
