using System;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    internal static class RoatpGatewayPageViewModelExtensions
    {
        internal static async Task PopulatePageCommonDetails(this RoatpGatewayPageViewModel viewModel, IRoatpApplicationApiClient applyApiClient, Guid applicationId, string pageId, string userName)
        {
            var commonDetails = await applyApiClient.GetPageCommonDetails(applicationId, pageId, userName);

            viewModel.ApplicationId = applicationId;
            viewModel.PageId = pageId;
            viewModel.ApplyLegalName = commonDetails.LegalName;
            viewModel.Ukprn = commonDetails.Ukprn;
            viewModel.Status = commonDetails.Status;
            viewModel.OptionPassText = commonDetails.OptionPassText;
            viewModel.OptionFailText = commonDetails.OptionFailText;
            viewModel.OptionInProgressText = commonDetails.OptionInProgressText;
            viewModel.SourcesCheckedOn = commonDetails.CheckedOn;
            viewModel.ApplicationSubmittedOn = commonDetails.ApplicationSubmittedOn;
            viewModel.GatewayReviewStatus = commonDetails.GatewayReviewStatus;
        }
    }
}
