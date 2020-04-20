using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewayRegisterChecksOrchestrator : IGatewayRegisterChecksOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly ILogger<GatewayRegisterChecksOrchestrator> _logger;

        public GatewayRegisterChecksOrchestrator(IRoatpApplicationApiClient applyApiClient, IRoatpApiClient roatpApiClient,
                                                     ILogger<GatewayRegisterChecksOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _roatpApiClient = roatpApiClient;
            _logger = logger;
        }

        public async Task<RoatpPageViewModel> GetRoatpViewModel(GetRoatpRequest request)
        {
            var pageId = GatewayPageIds.Roatp;
            _logger.LogInformation($"Retrieving RoATP details for application {request.ApplicationId}");

            var model = new RoatpPageViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, pageId, request.UserName,
                                                    RoatpGatewayConstants.Captions.RegisterChecks,
                                                    RoatpGatewayConstants.Headings.Roatp,
                                                    NoSelectionErrorMessages.Roatp);

            model.ApplyProviderRoute = await _applyApiClient.GetProviderRouteName(model.ApplicationId);

            var roatpProviderDetails = await _roatpApiClient.GetOrganisationRegisterStatus(model.Ukprn);

            if (roatpProviderDetails != null)
            {
                model.RoatpUkprnOnRegister = roatpProviderDetails.UkprnOnRegister;
                model.RoatpStatusDate = roatpProviderDetails.StatusDate;
                model.RoatpProviderRoute = await GetProviderRoute(roatpProviderDetails.ProviderTypeId);
                model.RoatpStatus = await GetProviderStatus(roatpProviderDetails.StatusId, roatpProviderDetails.ProviderTypeId);
                model.RoatpRemovedReason = await GetRemovedReason(roatpProviderDetails.RemovedReasonId);
            }

            return model;
        }

        public async Task<RoepaoPageViewModel> GetRoepaoViewModel(GetRoepaoRequest request)
        {
            var pageId = GatewayPageIds.Roepao;
            _logger.LogInformation($"Retrieving RoEPAO details for application {request.ApplicationId}");

            var model = new RoepaoPageViewModel();
            await model.PopulatePageCommonDetails(_applyApiClient, request.ApplicationId, pageId, request.UserName,
                                                    RoatpGatewayConstants.Captions.RegisterChecks,
                                                    RoatpGatewayConstants.Headings.Roepao,
                                                    NoSelectionErrorMessages.Roepao);

            return model;
        }

        private async Task<string> GetProviderRoute(int? providerTypeId)
        {
            string route = null;

            if (providerTypeId.HasValue)
            {
                var roatpTypes = await _roatpApiClient.GetProviderTypes();

                route = roatpTypes?.FirstOrDefault(t => t.Id == providerTypeId.Value)?.Type;
            }

            return route;
        }

        private async Task<string> GetProviderStatus(int? providerStatusId, int? providerTypeId)
        {
            string status = null;

            if(providerStatusId.HasValue)
            {
                var roatpStatuses = await _roatpApiClient.GetOrganisationStatuses(providerTypeId);

                status = roatpStatuses?.FirstOrDefault(s => s.Id == providerStatusId.Value)?.Status;
            }

            return status;
        }

        private async Task<string> GetRemovedReason(int? providerRemovedReasonId)
        {
            string reason = null;

            if (providerRemovedReasonId.HasValue)
            {
                var roatpRemovedReasons = await _roatpApiClient.GetRemovedReasons();

                reason = roatpRemovedReasons?.FirstOrDefault(rm => rm.Id == providerRemovedReasonId.Value)?.Reason;
            }

            return reason;
        }
    }
}
