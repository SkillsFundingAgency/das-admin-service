using MediatR;
using SFA.DAS.RoatpAssessor.Configuration;
using SFA.DAS.RoatpAssessor.Domain.DTOs;
using System;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetQuestionReviewRequest : IRequest<QuestionReview>
    {
        public Guid ApplicationId { get; }
        public OutcomeConfig OutcomeConfig { get; }

        public GetQuestionReviewRequest(Guid applicationId, OutcomeConfig outcomeConfig)
        {
            ApplicationId = applicationId;
            OutcomeConfig = outcomeConfig;
        }
    }
}
