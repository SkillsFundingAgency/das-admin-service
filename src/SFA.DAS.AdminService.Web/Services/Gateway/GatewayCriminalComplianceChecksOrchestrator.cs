﻿using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewayCriminalComplianceChecksOrchestrator : IGatewayCriminalComplianceChecksOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IRoatpGatewayCriminalComplianceChecksApiClient _criminalChecksApiClient;
        private readonly ILogger<GatewayCriminalComplianceChecksOrchestrator> _logger;

        public GatewayCriminalComplianceChecksOrchestrator(IRoatpApplicationApiClient applyApiClient, 
                                                           IRoatpGatewayCriminalComplianceChecksApiClient criminalChecksApiClient,
                                                           ILogger<GatewayCriminalComplianceChecksOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _criminalChecksApiClient = criminalChecksApiClient;
            _logger = logger;
        }

        public async Task<OrganisationCriminalCompliancePageViewModel> GetCriminalComplianceCheckViewModel(GetCriminalComplianceCheckRequest request)
        {
            _logger.LogInformation($"Retrieving criminal compliance details for application {request.ApplicationId} page {request.PageId}");

            var model = new OrganisationCriminalCompliancePageViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, request.PageId, request.UserName,
                                                    RoatpGatewayConstants.Captions.OrganisationsCriminalAndComplianceChecks,
                                                    CriminalCompliancePageConfiguration.Headings[request.PageId],
                                                    CriminalCompliancePageConfiguration.NoSelectionErrorMessages[request.PageId]);

            var criminalComplianceCheckDetails = await _criminalChecksApiClient.GetCriminalComplianceQuestionDetails(request.ApplicationId, request.PageId);
            model.QuestionText = criminalComplianceCheckDetails.QuestionText;
            model.ComplianceCheckQuestionId = criminalComplianceCheckDetails.QuestionId;
            model.ComplianceCheckAnswer = criminalComplianceCheckDetails.Answer;
            model.FurtherInformationQuestionId = criminalComplianceCheckDetails.FurtherQuestionId;
            model.FurtherInformationAnswer = criminalComplianceCheckDetails.FurtherAnswer;

            return model;
        }
    }
}
