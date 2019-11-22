using MediatR;
using SFA.DAS.RoatpAssessor.Configuration;
using SFA.DAS.RoatpAssessor.Domain.DTOs;
using System;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetQuestionReviewRequest : IRequest<QuestionReview>
    {
        public Guid ApplicationId { get; }
        public QuestionConfig QuestionConfig { get; }

        public GetQuestionReviewRequest(Guid applicationId, QuestionConfig questionConfig)
        {
            ApplicationId = applicationId;
            QuestionConfig = questionConfig;
        }
    }
}
