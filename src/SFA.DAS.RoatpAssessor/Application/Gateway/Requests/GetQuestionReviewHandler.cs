using MediatR;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.RoatpAssessor.Configuration;
using SFA.DAS.RoatpAssessor.Domain.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Application.Gateway.Requests
{
    public class GetQuestionReviewHandler : IRequestHandler<GetQuestionReviewRequest, QuestionReview>
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;

        public GetQuestionReviewHandler(IApplyApiClient applyApiClient, IQnaApiClient qnaApiClient)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
        }

        public async Task<QuestionReview> Handle(GetQuestionReviewRequest request, CancellationToken cancellationToken)
        {
            var pageTask = _qnaApiClient.GetPage(request.ApplicationId, request.OutcomeConfig.SectionId, request.OutcomeConfig.PageId);
            var gatewayReviewTask = _applyApiClient.GetGatewayReviewAsync(request.ApplicationId);
            var applicationTask = _applyApiClient.GetApplicationAsync(request.ApplicationId);

            await Task.WhenAll(pageTask, gatewayReviewTask, applicationTask);

            var gatewayReview = gatewayReviewTask.Result;
            var application = applicationTask.Result;

            var outcome = GetQuestionOutcome(gatewayReview, request.OutcomeConfig);
            var answers = GetQuestionAnswersFromPage(pageTask.Result, request.OutcomeConfig.QuestionId);

            var questionReview = new QuestionReview
            {
                OrganisationName = application.OrganisationName,
                Ukprn = application.Ukprn,
                Answers = answers,
                Outcome = outcome
            };

            return questionReview;
        }

        private Outcome GetQuestionOutcome(Domain.DTOs.Gateway gateway, OutcomeConfig questionConfig)
        {
            return gateway.Outcomes?.SingleOrDefault(o =>
                o.SectionId == questionConfig.SectionId &&
                o.PageId == questionConfig.PageId &&
                o.QuestionId == questionConfig.QuestionId);
        }

        private List<List<Answer>> GetQuestionAnswersFromPage(Page page, string questionId)
        {
            var allQuestionAnswers = new List<List<Answer>>();

            foreach (var pageOfAnswers in page.PageOfAnswers)
            {
                var questionAnswers = new List<Answer>();
                allQuestionAnswers.Add(questionAnswers);

                foreach (var answer in pageOfAnswers.Answers.Where(a =>
                 a.QuestionId == questionId))
                {
                    questionAnswers.Add(answer);
                }
            }

            return allQuestionAnswers;
        }
    }
}
