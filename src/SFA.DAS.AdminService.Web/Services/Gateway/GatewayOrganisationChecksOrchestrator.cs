using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewayOrganisationChecksOrchestrator : IGatewayOrganisationChecksOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly ILogger<GatewayOrganisationChecksOrchestrator> _logger;

        public GatewayOrganisationChecksOrchestrator(IRoatpApplicationApiClient applyApiClient,
                                                     ILogger<GatewayOrganisationChecksOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
        }

        public async Task<LegalNamePageViewModel> GetLegalNameViewModel(GetLegalNameRequest request)
        {
            _logger.LogInformation($"Retrieving legal name details for application {request.ApplicationId}");

            var pageId = GatewayPageIds.LegalName;

            var model = new LegalNamePageViewModel { ApplicationId = request.ApplicationId, PageId = pageId };

            model.GatewayReviewStatus = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId,
                request.UserName, RoatpGatewayFields.GatewayReviewStatus);

            model.ApplyLegalName = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId,
                request.UserName, RoatpGatewayFields.OrganisationName);
            model.Ukprn =
                await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId, request.UserName,
                    RoatpGatewayFields.UKPRN);

            model.UkrlpLegalName =
                await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId, request.UserName,
                    RoatpGatewayFields.UkrlpLegalName);
            
            var applicationSubmittedOn =
                await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId, request.UserName,
                    RoatpGatewayFields.ApplicationSubmittedOn);

            if (applicationSubmittedOn != null && DateTime.TryParse(applicationSubmittedOn, out var submittedOn))
                model.ApplicationSubmittedOn = submittedOn;

            var sourcesCheckedOn =
                await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId, request.UserName,
                    RoatpGatewayFields.SourcesCheckedOn);

            if (applicationSubmittedOn != null && DateTime.TryParse(sourcesCheckedOn, out var checkedOn))
                model.SourcesCheckedOn = checkedOn;

            model.CompaniesHouseLegalName = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                pageId,
                request.UserName, RoatpGatewayFields.CompaniesHouseName);


            model.CharityCommissionLegalName = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                pageId,
                request.UserName, RoatpGatewayFields.CharityCommissionName);


            var currentStatus = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId,
                request.UserName, RoatpGatewayFields.Status);

            model.Status = currentStatus;

            if (string.IsNullOrEmpty(currentStatus)) return model;
            switch (currentStatus)
            {
                case SectionReviewStatus.Pass:
                    model.OptionPassText = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                        pageId,
                        request.UserName, "OptionPassText");
                    break;
                case SectionReviewStatus.Fail:
                    model.OptionFailText = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                        pageId,
                        request.UserName, "OptionFailText");
                    break;
                case SectionReviewStatus.InProgress:
                    model.OptionInProgressText = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                        pageId,
                        request.UserName, "OptionInProgressText");
                    break;
            }

            return model;
        }
    }
}
