using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Handlers.Gateway;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewayCriminalComplianceChecksOrchestrator : IGatewayCriminalComplianceChecksOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly ICriminalComplianceChecksQuestionLookupService _questionLookupService;
        private readonly ILogger<GatewayCriminalComplianceChecksOrchestrator> _logger;

        public GatewayCriminalComplianceChecksOrchestrator(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient,
                                                           ICriminalComplianceChecksQuestionLookupService questionLookupService, 
                                                           ILogger<GatewayCriminalComplianceChecksOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _questionLookupService = questionLookupService;
            _logger = logger;
        }

        public async Task<OrganisationCriminalCompliancePageViewModel> GetCriminalComplianceCheckViewModel(GetCriminalComplianceCheckRequest request)
        {
            var model = new OrganisationCriminalCompliancePageViewModel { ApplicationId = request.ApplicationId, PageId = request.PageId };

            var applicationDetails = await _applyApiClient.GetApplication(model.ApplicationId);

            var gatewayReviewStatus = string.Empty;
            if (applicationDetails?.GatewayReviewStatus != null)
            {
                model.GatewayReviewStatus = applicationDetails.GatewayReviewStatus;
            }

            var ukrlpData = await _applyApiClient.GetUkrlpDetails(request.ApplicationId);
            model.ApplyLegalName = ukrlpData.ProviderName;
            model.Ukprn = ukrlpData.UKPRN;

            var pageDetails = _questionLookupService.GetPageDetailsForGatewayCheckPageId(request.PageId);
            model.PageTitle = pageDetails.Title;

            _logger.LogInformation($"Retrieving criminal compliance details for application {request.ApplicationId} page {request.PageId}");

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
                    foreach (var option in complianceChecksQuestion.Input.Options)
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

            return model;
        }
    }
}
