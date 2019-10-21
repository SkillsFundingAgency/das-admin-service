using MediatR;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.RoatpAssessor.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Commands
{
    public class UpdateGatewayReviewStatusHandler : IRequestHandler<UpdateGatewayReviewStatusCommand>
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;

        public UpdateGatewayReviewStatusHandler(IApplyApiClient applyApiClient, IQnaApiClient qnaApiClient)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
        }

        public async Task<Unit> Handle(UpdateGatewayReviewStatusCommand request, CancellationToken cancellationToken)
        {
            var review = await _applyApiClient.GetApplicationReviewAsync(request.ApplicationId);

            if (!review.GatewayReviewIsInProgress)
            {
                throw new Exception($"Review '{request.ApplicationId}' is not in correct state to do Gateway review");
            }

            await _applyApiClient.UpdateApplicationReviewGatewayReviewAsync(review.ApplicationId, request.Status);

            return Unit.Value;
        }
    }
}
