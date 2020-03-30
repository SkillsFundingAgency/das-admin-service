using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewaySectionsNotRequiredService : IGatewaySectionsNotRequiredService
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly ILogger<GatewaySectionsNotRequiredService> _logger;

        public GatewaySectionsNotRequiredService(IRoatpApplicationApiClient applyApiClient, ILogger<GatewaySectionsNotRequiredService> logger)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
        }

        public async Task SetupNotRequiredLinks(Guid applicationId, string userName, RoatpGatewayApplicationViewModel viewModel, int providerRoute)
        {
            await SetupNotRequiredLinkForTradingName(applicationId, userName, viewModel);

            await SetupNotRequiredLinkForWebsiteAddress(applicationId, userName, viewModel);

            await SetupNotRequiredLinkForOfficeForStudents(applicationId, userName, viewModel, providerRoute);

            await SetupNotRequiredLinkForInitialTeacherTraining(applicationId, userName, viewModel, providerRoute);

            await SetupNotRequireLinkForOfsted(applicationId, userName, viewModel, providerRoute);

            await SetupNotRequiredLinkForSubcontractorDeclaration(applicationId, userName, viewModel, providerRoute);

            await SetupNotRequiredLinksForPeopleInControlCriminalComplianceChecks(applicationId, userName, viewModel);
        } 

        private async Task SetupNotRequiredLinkForTradingName(Guid applicationId, string userName, RoatpGatewayApplicationViewModel viewModel)
        {
            var tradingName = await _applyApiClient.GetTradingName(applicationId);

            if (string.IsNullOrEmpty(tradingName))
            {
                var page = GetSectionByPageId(viewModel, GatewayPageIds.TradingName);

                if (page != null)
                    page.Status = SectionReviewStatus.NotRequired;

                _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{applicationId}' - PageId '{GatewayPageIds.TradingName}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{userName}' - PageData = 'null'");
                await _applyApiClient.SubmitGatewayPageAnswer(applicationId, GatewayPageIds.TradingName, SectionReviewStatus.NotRequired, userName, null);
            }
        }

        private async Task SetupNotRequiredLinkForWebsiteAddress(Guid applicationId, string userName, RoatpGatewayApplicationViewModel viewModel)
        {
            var websiteAddressUkrlp = await _applyApiClient.GetWebsiteAddressSourcedFromUkrlp(applicationId);
            var websiteAddressApply =
                await _applyApiClient.GetWebsiteAddressManuallyEntered(applicationId);
            var websiteAddressStatus = string.IsNullOrWhiteSpace(websiteAddressUkrlp) && string.IsNullOrWhiteSpace(websiteAddressApply) ? SectionReviewStatus.NotRequired : string.Empty; ;
            if (websiteAddressStatus.Equals(SectionReviewStatus.NotRequired))
            {
                var page = GetSectionByPageId(viewModel, GatewayPageIds.WebsiteAddress);

                if (page != null)
                    page.Status = SectionReviewStatus.NotRequired;
                _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{applicationId}' - PageId '{GatewayPageIds.WebsiteAddress}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{userName}' - PageData = 'null'");
                await _applyApiClient.SubmitGatewayPageAnswer(applicationId, GatewayPageIds.WebsiteAddress, SectionReviewStatus.NotRequired, userName, null);
            }
        }

        private async Task SetupNotRequiredLinkForOfficeForStudents(Guid applicationId, string userName, RoatpGatewayApplicationViewModel viewModel, int providerRoute)
        {
            var officeForStudentStatus = SectionReviewStatus.NotRequired;

            if (providerRoute.Equals(ProviderTypes.Main) || providerRoute.Equals(ProviderTypes.Employer))
            {
                var officeForStudent = await _applyApiClient.GetOfficeForStudents(applicationId);
                if (officeForStudent != null && officeForStudent.Equals("Yes", StringComparison.InvariantCultureIgnoreCase)) officeForStudentStatus = string.Empty;
            }
            if (officeForStudentStatus.Equals(SectionReviewStatus.NotRequired))
            {
                var page = GetSectionByPageId(viewModel, GatewayPageIds.OfficeForStudents);

                if (page != null)
                    page.Status = SectionReviewStatus.NotRequired;
                _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{applicationId}' - PageId '{GatewayPageIds.OfficeForStudents}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{userName}' - PageData = 'null'");
                await _applyApiClient.SubmitGatewayPageAnswer(applicationId, GatewayPageIds.OfficeForStudents, SectionReviewStatus.NotRequired, userName, null);
            }
        }
        private async Task SetupNotRequiredLinkForInitialTeacherTraining(Guid applicationId, string userName, RoatpGatewayApplicationViewModel viewModel, int providerRoute)
        {
            var initialTeacherTrainingStatus = SectionReviewStatus.NotRequired;

            if (providerRoute.Equals(ProviderTypes.Main) || providerRoute.Equals(ProviderTypes.Employer))
            {
                var initialTeacherTraining = await _applyApiClient.GetInitialTeacherTraining(applicationId);
                if (initialTeacherTraining != null && initialTeacherTraining.Equals("Yes", StringComparison.InvariantCultureIgnoreCase)) initialTeacherTrainingStatus = string.Empty;
            }

            if (initialTeacherTrainingStatus.Equals(SectionReviewStatus.NotRequired))
            {
                var page = GetSectionByPageId(viewModel, GatewayPageIds.InitialTeacherTraining);

                page.Status = SectionReviewStatus.NotRequired;

                _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{applicationId}' - PageId '{GatewayPageIds.InitialTeacherTraining}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{userName}' - PageData = 'null'");
                await _applyApiClient.SubmitGatewayPageAnswer(applicationId, GatewayPageIds.InitialTeacherTraining, SectionReviewStatus.NotRequired, userName, null);
            }
        }

        private async Task SetupNotRequireLinkForOfsted(Guid applicationId, string userName, RoatpGatewayApplicationViewModel viewModel, int providerRoute)
        {
            var OfstedStatus = providerRoute.Equals(ProviderTypes.Supporting) ? SectionReviewStatus.NotRequired : string.Empty;
            if (OfstedStatus.Equals(SectionReviewStatus.NotRequired))
            {
                var page = GetSectionByPageId(viewModel, GatewayPageIds.Ofsted);

                if (page != null)
                    page.Status = SectionReviewStatus.NotRequired;

                _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{applicationId}' - PageId '{GatewayPageIds.Ofsted}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{userName}' - PageData = 'null'");
                await _applyApiClient.SubmitGatewayPageAnswer(applicationId, GatewayPageIds.Ofsted, SectionReviewStatus.NotRequired, userName, null);
            }
        }

        private async Task SetupNotRequiredLinkForSubcontractorDeclaration(Guid applicationId, string userName, RoatpGatewayApplicationViewModel viewModel, int providerRoute)
        {
            var SubcontractorDeclarationStatus = providerRoute.Equals(ProviderTypes.Supporting) ? string.Empty : SectionReviewStatus.NotRequired;
            if (SubcontractorDeclarationStatus.Equals(SectionReviewStatus.NotRequired))
            {
                var page = GetSectionByPageId(viewModel, GatewayPageIds.SubcontractorDeclaration);

                if (page != null)
                    page.Status = SectionReviewStatus.NotRequired;

                _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{applicationId}' - PageId '{GatewayPageIds.SubcontractorDeclaration}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{userName}' - PageData = 'null'");
                await _applyApiClient.SubmitGatewayPageAnswer(applicationId, GatewayPageIds.SubcontractorDeclaration, SectionReviewStatus.NotRequired, userName, null);
            }
        }

        private async Task SetupNotRequiredLinksForPeopleInControlCriminalComplianceChecks(Guid applicationId, string userName, RoatpGatewayApplicationViewModel viewModel)
        {
            var ukrlpDetails = await _applyApiClient.GetUkrlpDetails(applicationId);

            if (ukrlpDetails.VerificationDetails.FirstOrDefault(x => x.VerificationAuthority == VerificationAuthorities.SoleTraderPartnershipAuthority) != null)
            {
                var criminalComplianceWhosInControlSection = viewModel.Sequences.FirstOrDefault(x => x.SequenceNumber
                                                                                                == GatewaySequences.PeopleInControlCriminalComplianceChecks);
                if (criminalComplianceWhosInControlSection != null)
                {
                    foreach (var page in criminalComplianceWhosInControlSection.Sections)
                    {
                        page.Status = SectionReviewStatus.NotRequired;
                        _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{applicationId}' - PageId '{page.PageId}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{userName}' - PageData = 'null'");
                        await _applyApiClient.SubmitGatewayPageAnswer(applicationId, page.PageId, SectionReviewStatus.NotRequired, userName, null);
                    }
                }
            }
        }


        private GatewaySection GetSectionByPageId(RoatpGatewayApplicationViewModel viewModel, string gatewayPageId)
        {
            return viewModel?.Sequences?.SelectMany(seq => seq.Sections)
                    .Where(sec => sec.PageId == gatewayPageId)?.FirstOrDefault();
        }
    }
}
