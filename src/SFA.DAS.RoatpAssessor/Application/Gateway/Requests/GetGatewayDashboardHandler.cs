using MediatR;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.RoatpAssessor.Application.Extensions;
using SFA.DAS.RoatpAssessor.Application.Models;
using SFA.DAS.RoatpAssessor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetGatewayDashboardHandler : IRequestHandler<GetGatewayDashboardRequest, GetGatewayDashboardResponse>
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;

        public GetGatewayDashboardHandler(IApplyApiClient applyApiClient, IQnaApiClient qnaApiClient)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
        }

        public async Task<GetGatewayDashboardResponse> Handle(GetGatewayDashboardRequest request, CancellationToken cancellationToken)
        {
            var reviews = await  _applyApiClient.GetActiveApplicationReviewsAsync();

            var newGatewayReviews = reviews.Where(r => r.GatewayReviewCanBeStarted);
            var InProgressGatewayReviews = reviews.Where(r => r.GatewayReviewIsInProgress);
            var completedGatewayReviews = reviews.Where(r => r.GatewayReviewIsCompleted);

            var newReviewSummariesTask = GetGatewayReviewSummariesAsync(newGatewayReviews);
            var inProgressReviewSummariesTask = GetGatewayReviewSummariesAsync(InProgressGatewayReviews);
            var completedReviewSummariesTask = GetGatewayReviewSummariesAsync(completedGatewayReviews);

            await Task.WhenAll(newReviewSummariesTask, inProgressReviewSummariesTask, completedReviewSummariesTask);

            var response = new GetGatewayDashboardResponse
            {
                PendingReviews = newReviewSummariesTask.Result,
                InProgressReviews = inProgressReviewSummariesTask.Result,
                CompletedReviews = completedReviewSummariesTask.Result
            };

            return response;
        }

        private async Task<IEnumerable<ReviewSummary>> GetGatewayReviewSummariesAsync(IEnumerable<Domain.Entities.ApplicationReview> gatewayReviews)
        {
            var reviewSummariesTasks = new List<Task<ReviewSummary>>();

            foreach (var review in gatewayReviews)
            {
                reviewSummariesTasks.Add(GetReviewSummaryAsync(review.ApplicationId, review.ApplicationRef, review.ApplicationSubmittedAt));
            }

            var reviewSummaries = await Task.WhenAll(reviewSummariesTasks);

            return reviewSummaries;
        }

        private async Task<ReviewSummary> GetReviewSummaryAsync(Guid applicationId, string applicationRef, DateTime submittedAt)
        {
            var applicationData = new RoatpAssessorApplicationData( await _qnaApiClient.GetApplicationData(applicationId));

            return new ReviewSummary
            {
                ApplicationId = applicationId,
                ApplicationRef = applicationRef,
                SubmittedAt = submittedAt,
                Organisation = applicationData.UKRLP_LegalName,
                ProviderType = applicationData.Apply_ProviderRoute.ToString(),
                Ukprn = applicationData.UKPRN
            };
        }
    }
}
