using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.Apply;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewayPeopleInControlOrchestrator: IGatewayPeopleInControlOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IRoatpOrganisationSummaryApiClient _organisationSummaryApiClient;
        private readonly ILogger<GatewayPeopleInControlOrchestrator> _logger;

        public GatewayPeopleInControlOrchestrator(IRoatpApplicationApiClient applyApiClient, IRoatpOrganisationSummaryApiClient organisationSummaryApiClient, ILogger<GatewayPeopleInControlOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
            _organisationSummaryApiClient = organisationSummaryApiClient;
        }

        public async Task<PeopleInControlPageViewModel> GetPeopleInControlViewModel(GetPeopleInControlRequest request)
        {
             _logger.LogInformation($"Retrieving people in control details for application {request.ApplicationId}");

            var pageId = GatewayPageIds.PeopleInControl;

            var commonDetails =
                await _applyApiClient.GetPageCommonDetails(request.ApplicationId, pageId, request.UserName);

            var model = new PeopleInControlPageViewModel
            {
                ApplicationId = request.ApplicationId,
                PageId = pageId,
                ApplyLegalName = commonDetails.LegalName,
                Ukprn = commonDetails.Ukprn,
                Status = commonDetails.Status,
                OptionPassText = commonDetails.OptionPassText,
                OptionFailText = commonDetails.OptionFailText,
                OptionInProgressText = commonDetails.OptionInProgressText,
                SourcesCheckedOn = commonDetails.CheckedOn,
                ApplicationSubmittedOn = commonDetails.ApplicationSubmittedOn,
                GatewayReviewStatus = commonDetails.GatewayReviewStatus
            };

            _logger.LogInformation($"Retrieving people in control - type of organisation for application {request.ApplicationId}");
            model.TypeOfOrganisation = await _organisationSummaryApiClient.GetTypeOfOrganisation(request.ApplicationId);


            _logger.LogInformation($"Retrieving people in control - company directors for application {request.ApplicationId}");
            model.CompanyDirectorsData = new PeopleInControlData
            {
                Caption = "Company directors",
                ExternalSourceHeading = "Companies House data",
                FromExternalSource = await _organisationSummaryApiClient.GetDirectorsFromCompaniesHouse(request.ApplicationId),
                FromSubmittedApplication = await _organisationSummaryApiClient.GetDirectorsFromSubmitted(request.ApplicationId)
            };

            _logger.LogInformation($"Retrieving people in control - persons of significant control for application {request.ApplicationId}");
            model.PscData = new PeopleInControlData
            {
                Caption = "People with significant control (PSC’s)",
                ExternalSourceHeading = "Companies House data",
                FromExternalSource = await _organisationSummaryApiClient.GetPscsFromCompaniesHouse(request.ApplicationId),
                FromSubmittedApplication = await _organisationSummaryApiClient.GetPscsFromSubmitted(request.ApplicationId)
            };

            _logger.LogInformation($"Retrieving people in control - trustees for application {request.ApplicationId}");
            model.TrusteeData = new PeopleInControlData
            {
                Caption = "Trustees",
                ExternalSourceHeading = "Charity Commission data",
                FromExternalSource = await _organisationSummaryApiClient.GetTrusteesFromCharityCommission(request.ApplicationId),
                FromSubmittedApplication = await _organisationSummaryApiClient.GetTrusteesFromSubmitted(request.ApplicationId)
            };

            _logger.LogInformation($"Retrieving people in control - who's in control for application {request.ApplicationId}");
            model.WhosInControlData = new PeopleInControlData
            {
                Caption = "Who's in control",
                ExternalSourceHeading = null,
                FromExternalSource = null,
                FromSubmittedApplication =
                    await _organisationSummaryApiClient.GetWhosInControlFromSubmitted(request.ApplicationId)
            };

            return model;
        }
    }
}
