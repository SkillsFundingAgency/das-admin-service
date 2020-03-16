using MediatR;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Services.Gateway;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetCriminalComplianceCheckHandler : IRequestHandler<GetCriminalComplianceCheckRequest, OrganisationCriminalCompliancePageViewModel>
    {

        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly ICriminalComplianceChecksQuestionLookupService _questionLookupService;
        private readonly ILogger<GetCriminalComplianceCheckHandler> _logger;

        public GetCriminalComplianceCheckHandler(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient,
                                                 ICriminalComplianceChecksQuestionLookupService questionLookupService, ILogger<GetCriminalComplianceCheckHandler> logger)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _questionLookupService = questionLookupService;
            _logger = logger;
        }

        public async Task<OrganisationCriminalCompliancePageViewModel> Handle(GetCriminalComplianceCheckRequest request, CancellationToken cancellationToken)
        {
            var model = new OrganisationCriminalCompliancePageViewModel { ApplicationId = request.ApplicationId, PageId = request.PageId };

            var currentRecord = await _applyApiClient.GetGatewayPageAnswer(request.ApplicationId, request.PageId);
            var applicationDetails = await _applyApiClient.GetApplication(model.ApplicationId);

            var gatewayReviewStatus = string.Empty;
            if (applicationDetails?.GatewayReviewStatus != null)
            {
                gatewayReviewStatus = applicationDetails.GatewayReviewStatus;
            }

            if (currentRecord?.GatewayPageData != null)
            {
                model = JsonConvert.DeserializeObject<OrganisationCriminalCompliancePageViewModel>(currentRecord.GatewayPageData);
                model.Status = currentRecord.Status;
                model.GatewayReviewStatus = gatewayReviewStatus;
                return model;
            }

            model.GatewayReviewStatus = gatewayReviewStatus;

            var ukprn = await _qnaApiClient.GetQuestionTag(request.ApplicationId, RoatpQnaConstants.QnaQuestionTags.Ukprn);
            model.Ukprn = ukprn;

            model.ApplyLegalName = await _qnaApiClient.GetQuestionTag(request.ApplicationId, RoatpQnaConstants.QnaQuestionTags.UKRLPLegalName);

            var pageDetails = _questionLookupService.GetPageDetailsForGatewayCheckPageId(request.PageId);
            model.PageTitle = pageDetails.Title;

            var qnaPage = await _qnaApiClient.GetPageBySectionNo(request.ApplicationId, RoatpQnaConstants.RoatpSequences.CriminalComplianceChecks,
                                                                 RoatpQnaConstants.RoatpSections.CriminalComplianceChecks.OrganisationComplianceChecks,
                                                                 pageDetails.PageId);

            var complianceChecksQuestion = qnaPage.Questions.FirstOrDefault(x => x.QuestionId == pageDetails.QuestionId);

            if (complianceChecksQuestion != null)
            {
                model.QuestionText = complianceChecksQuestion.Label;
                var pageOfAnswers = qnaPage.PageOfAnswers.FirstOrDefault();
                if (pageOfAnswers != null)
                {
                    var answer = pageOfAnswers.Answers.FirstOrDefault(x => x.QuestionId == pageDetails.QuestionId);
                    model.ComplianceCheckQuestionId = answer.QuestionId;
                    model.ComplianceCheckAnswer = answer.Value;
                    foreach(var option in complianceChecksQuestion.Input.Options)
                    {
                        if (option.FurtherQuestions != null && option.FurtherQuestions.Any() && option.Value == answer.Value)
                        {
                            var furtherInformationAnswer = pageOfAnswers.Answers.FirstOrDefault(x => x.QuestionId == option.FurtherQuestions[0]?.QuestionId);
                            if (furtherInformationAnswer != null)
                            {
                                model.FurtherInformationQuestionId = furtherInformationAnswer.QuestionId;
                                model.FurtherInformationAnswer = furtherInformationAnswer.Value;
                                break;
                            }
                        }
                    }
                }
            }

            var pageData = JsonConvert.SerializeObject(model);
            _logger.LogInformation($"GetCriminalComplianceCheckHandler - SubmitGatewayPageAnswer -  ApplicationId '{model.ApplicationId}' - PageId '{model.PageId}' - Status '{model.Status}' - UserName '{request.UserName}' - PageData '{pageData}'");
            try
            {
                await _applyApiClient.SubmitGatewayPageAnswer(model.ApplicationId, model.PageId, model.Status, request.UserName, pageData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCriminalComplianceCheckHandler - SubmitGatewayPageAnswer -  Error: '" + ex.Message + "'");
            }

            return model;
        }



    }
}
