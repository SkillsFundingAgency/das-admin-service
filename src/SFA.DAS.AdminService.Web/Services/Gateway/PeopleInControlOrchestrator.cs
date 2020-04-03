using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure.Apply;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class PeopleInControlOrchestrator: IPeopleInControlOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IRoatpOrganisationSummaryApiClient _organisationSummaryApiClient;
        private readonly ILogger<PeopleInControlOrchestrator> _logger;

        public PeopleInControlOrchestrator(IRoatpApplicationApiClient applyApiClient, IRoatpOrganisationSummaryApiClient organisationSummaryApiClient, ILogger<PeopleInControlOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
            _organisationSummaryApiClient = organisationSummaryApiClient;
        }

        public async Task<PeopleInControlPageViewModel> GetPeopleInControlViewModel(GetPeopleInControlRequest request)
        {
             _logger.LogInformation($"Retrieving people in control details for application {request.ApplicationId}");

             var model = new PeopleInControlPageViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.PeopleInControl, request.UserName,
                RoatpGatewayConstants.Captions.PeopleInControlChecks,
                RoatpGatewayConstants.Headings.PeopleInControl,
                NoSelectionErrorMessages.PeopleInControlCheck);

            _logger.LogInformation($"Retrieving people in control - type of organisation for application {request.ApplicationId}");
            model.TypeOfOrganisation = await _organisationSummaryApiClient.GetTypeOfOrganisation(request.ApplicationId);

            _logger.LogInformation($"Retrieving people in control - [{RoatpGatewayConstants.PeopleInControl.Heading.CompanyDirectors}] for application {request.ApplicationId}");
            model.CompanyDirectorsData = new PeopleInControlData
            {
                Caption = RoatpGatewayConstants.PeopleInControl.Heading.CompanyDirectors,
                ExternalSourceHeading = RoatpGatewayConstants.PeopleInControl.Caption.CompaniesHouse,
                FromExternalSource = await _organisationSummaryApiClient.GetDirectorsFromCompaniesHouse(request.ApplicationId),
                FromSubmittedApplication = await _organisationSummaryApiClient.GetDirectorsFromSubmitted(request.ApplicationId)
            };

            _logger.LogInformation($"Retrieving people in control - [{RoatpGatewayConstants.PeopleInControl.Heading.PeopleWithSignificantControl}] for application {request.ApplicationId}");
            model.PscData = new PeopleInControlData
            {
                Caption = RoatpGatewayConstants.PeopleInControl.Heading.PeopleWithSignificantControl,
                ExternalSourceHeading = RoatpGatewayConstants.PeopleInControl.Caption.CompaniesHouse,
                FromExternalSource = await _organisationSummaryApiClient.GetPscsFromCompaniesHouse(request.ApplicationId),
                FromSubmittedApplication = await _organisationSummaryApiClient.GetPscsFromSubmitted(request.ApplicationId)
            };

            _logger.LogInformation($"Retrieving people in control - [{RoatpGatewayConstants.PeopleInControl.Heading.Trustees}] for application {request.ApplicationId}");
            model.TrusteeData = new PeopleInControlData
            {
                Caption = RoatpGatewayConstants.PeopleInControl.Heading.Trustees,
                ExternalSourceHeading = RoatpGatewayConstants.PeopleInControl.Caption.CharityCommission,
                FromExternalSource = await _organisationSummaryApiClient.GetTrusteesFromCharityCommission(request.ApplicationId),
                FromSubmittedApplication = await _organisationSummaryApiClient.GetTrusteesFromSubmitted(request.ApplicationId)
            };

            _logger.LogInformation($"Retrieving people in control - [{RoatpGatewayConstants.PeopleInControl.Heading.WhosInControl}] for application {request.ApplicationId}");
            model.WhosInControlData = new PeopleInControlData
            {
                Caption = RoatpGatewayConstants.PeopleInControl.Heading.WhosInControl,
                ExternalSourceHeading = null,
                FromExternalSource = null,
                FromSubmittedApplication =
                    await _organisationSummaryApiClient.GetWhosInControlFromSubmitted(request.ApplicationId)
            };

            return model;
        }

        public async Task<PeopleInControlHighRiskPageViewModel> GetPeopleInControlHighRiskViewModel(GetPeopleInControlRequest request)
        {
            var model = new PeopleInControlHighRiskPageViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, GatewayPageIds.PeopleInControlRisk, request.UserName,
                RoatpGatewayConstants.Captions.PeopleInControlChecks,
                RoatpGatewayConstants.Headings.PeopleInControlHighRisk,
                NoSelectionErrorMessages.PeopleInControlHighRiskCheck);

            _logger.LogInformation($"Retrieving people in control high risk - type of organisation for application {request.ApplicationId}");
            model.TypeOfOrganisation = await _organisationSummaryApiClient.GetTypeOfOrganisation(request.ApplicationId);

            _logger.LogInformation($"Retrieving people in control high risk - [{RoatpGatewayConstants.PeopleInControl.Heading.CompanyDirectors}] for application {request.ApplicationId}");
            model.CompanyDirectorsData = new PeopleInControlHighRiskData
            {
                Heading = RoatpGatewayConstants.PeopleInControl.Heading.CompanyDirectors,
                PeopleInControl = await _organisationSummaryApiClient.GetDirectorsFromSubmitted(request.ApplicationId)
            };

            _logger.LogInformation($"Retrieving people in control high risk - [{RoatpGatewayConstants.PeopleInControl.Heading.PeopleWithSignificantControl}] for application {request.ApplicationId}");
            model.PscData = new PeopleInControlHighRiskData
            {
                Heading = RoatpGatewayConstants.PeopleInControl.Heading.PeopleWithSignificantControl,
                PeopleInControl = await _organisationSummaryApiClient.GetPscsFromSubmitted(request.ApplicationId)
            };

            _logger.LogInformation($"Retrieving people in control high risk - [{RoatpGatewayConstants.PeopleInControl.Heading.Trustees}] for application {request.ApplicationId}");
            model.TrusteeData = new PeopleInControlHighRiskData
            {
                Heading = RoatpGatewayConstants.PeopleInControl.Heading.Trustees,
                PeopleInControl = await _organisationSummaryApiClient.GetTrusteesFromSubmitted(request.ApplicationId)
            };

            _logger.LogInformation($"Retrieving people in control high risk - [{RoatpGatewayConstants.PeopleInControl.Heading.WhosInControl}] for application {request.ApplicationId}");
            model.WhosInControlData = new PeopleInControlHighRiskData
            {
                Heading = RoatpGatewayConstants.PeopleInControl.Heading.WhosInControl,
                PeopleInControl = await _organisationSummaryApiClient.GetWhosInControlFromSubmitted(request.ApplicationId)
            };

            return model;
        }
    }
}
