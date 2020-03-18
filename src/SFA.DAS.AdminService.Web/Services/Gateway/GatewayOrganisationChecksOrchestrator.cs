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

            var headerDetails =
                await _applyApiClient.GetGatewayPageHeaderDetails(request.ApplicationId, pageId, request.UserName);

            var model = new LegalNamePageViewModel { ApplicationId = request.ApplicationId, PageId = pageId };

            model.ApplyLegalName = headerDetails.LegalName;
            model.Ukprn = headerDetails.Ukprn;
            model.Status = headerDetails.Status;

            model.GatewayReviewStatus = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId,
                request.UserName, GatewayFields.GatewayReviewStatus);

            model.UkrlpLegalName =
                await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId, pageId, request.UserName,
                    GatewayFields.UkrlpLegalName);

            var applicationSubmittedOn = headerDetails.ApplicationSubmittedOn;

            if (applicationSubmittedOn != null && DateTime.TryParse(applicationSubmittedOn, out var submittedOn))
                model.ApplicationSubmittedOn = submittedOn;

            var sourcesCheckedOn = headerDetails.CheckedOn;

            if (applicationSubmittedOn != null && DateTime.TryParse(sourcesCheckedOn, out var checkedOn))
                model.SourcesCheckedOn = checkedOn;

            model.CompaniesHouseLegalName = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                pageId,
                request.UserName, GatewayFields.CompaniesHouseName);

            model.CharityCommissionLegalName = await _applyApiClient.GetGatewayPageAnswerValue(request.ApplicationId,
                pageId,
                request.UserName, GatewayFields.CharityCommissionName);

            switch (model.Status)
            {
                case null:
                    break;
                case SectionReviewStatus.Pass:
                    model.OptionPassText = headerDetails.OptionPassText;
                    break;
                case SectionReviewStatus.Fail:
                    model.OptionFailText = headerDetails.OptionFailText;
                    break;
                case SectionReviewStatus.InProgress:
                    model.OptionInProgressText = headerDetails.OptionInProgressText;
                    break;
               
            }

            return model;
        }
    }
}
