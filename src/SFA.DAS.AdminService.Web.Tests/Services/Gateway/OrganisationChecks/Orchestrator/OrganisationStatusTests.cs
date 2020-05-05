using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure.Apply;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.OrganisationChecks.Orchestrator
{
    [TestFixture]
    public class OrganisationStatusTests
    {
        private GatewayOrganisationChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOrganisationChecksOrchestrator>> _logger;

        private static string ukprn => "12344321";
        private static string UKRLPLegalName => "Mark's workshop";

        private static string ProviderName => "Mark's other workshop";
        private static string CompanyStatus => "active";
        private static string CompanyStatusWithCapitalisation => "Active";

        private static string CharityStatus => "closed";
        private static string CharityStatusWithCapitalisation => "Closed";
        private static string ProviderStatus = "registered";
        private static string ProviderStatusWithCapitalisation = "Registered";
        private static string UserName = "GatewayUser";              

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _logger = new Mock<ILogger<GatewayOrganisationChecksOrchestrator>>();
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, Mock.Of<IRoatpOrganisationSummaryApiClient>(), _logger.Object);
        }

        [Test]
        public void check_organisation_status_orchestrator_builds_with_company_and_charity_details()
        {
            var applicationId = Guid.NewGuid();

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                CheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(applicationId, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            var ukrlpDetails = new ProviderDetails
            {
                ProviderName = ProviderName,
                ProviderStatus = ProviderStatus,
                VerifiedByCompaniesHouse = true,
                VerifiedbyCharityCommission = true
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(applicationId)).ReturnsAsync(ukrlpDetails);

            var companiesHouseDetails = new CompaniesHouseSummary
            {
                Status = CompanyStatus
            };
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(applicationId)).ReturnsAsync(companiesHouseDetails);

            var charityDetails = new CharityCommissionSummary
            {
                Status = CharityStatus
            };
            _applyApiClient.Setup(x => x.GetCharityCommissionDetails(applicationId)).ReturnsAsync(charityDetails);

            var request = new GetOrganisationStatusRequest(applicationId, UserName);

            var response = _orchestrator.GetOrganisationStatusViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(ProviderStatusWithCapitalisation, viewModel.UkrlpStatus);
            Assert.AreEqual(CompanyStatusWithCapitalisation, viewModel.CompaniesHouseStatus);
            Assert.AreEqual(CharityStatusWithCapitalisation, viewModel.CharityCommissionStatus);
            Assert.AreEqual(ukprn, viewModel.Ukprn);

            _applyApiClient.Verify(x => x.GetUkrlpDetails(applicationId), Times.Once);
            _applyApiClient.Verify(x => x.GetCompaniesHouseDetails(applicationId), Times.Once);
            _applyApiClient.Verify(x => x.GetCharityCommissionDetails(applicationId), Times.Once);
        }

        [Test]
        public void check_organisation_status_orchestrator_builds_with_company_details_only()
        {
            var applicationId = Guid.NewGuid();

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                CheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(applicationId, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            var ukrlpDetails = new ProviderDetails
            {
                ProviderName = ProviderName,
                ProviderStatus = ProviderStatus,
                VerifiedByCompaniesHouse = true,
                VerifiedbyCharityCommission = false
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(applicationId)).ReturnsAsync(ukrlpDetails);

            var companiesHouseDetails = new CompaniesHouseSummary
            {
                Status = CompanyStatus
            };
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(applicationId)).ReturnsAsync(companiesHouseDetails);

            var charityDetails = new CharityCommissionSummary
            {
                Status = null
            };

            _applyApiClient.Setup(x => x.GetCharityCommissionDetails(applicationId)).ReturnsAsync(charityDetails);

            var request = new GetOrganisationStatusRequest(applicationId, UserName);

            var response = _orchestrator.GetOrganisationStatusViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(ProviderStatusWithCapitalisation, viewModel.UkrlpStatus);
            Assert.AreEqual(CompanyStatusWithCapitalisation, viewModel.CompaniesHouseStatus);
            Assert.IsNull(viewModel.CharityCommissionStatus);
            Assert.AreEqual(ukprn, viewModel.Ukprn);

            _applyApiClient.Verify(x => x.GetUkrlpDetails(applicationId), Times.Once);
            _applyApiClient.Verify(x => x.GetCompaniesHouseDetails(applicationId), Times.Once);
            _applyApiClient.Verify(x => x.GetCharityCommissionDetails(applicationId), Times.Never);
        }

        [Test]
        public void check_organisation_status_orchestrator_builds_with_charity_details_only()
        {
            var applicationId = Guid.NewGuid();

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                CheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(applicationId, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            var ukrlpDetails = new ProviderDetails
            {
                ProviderName = ProviderName,
                ProviderStatus = ProviderStatus,
                VerifiedByCompaniesHouse = false,
                VerifiedbyCharityCommission = true
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(applicationId)).ReturnsAsync(ukrlpDetails);

            var companiesHouseDetails = new CompaniesHouseSummary
            {
                Status = null
            };
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(applicationId)).ReturnsAsync(companiesHouseDetails);

            var charityDetails = new CharityCommissionSummary
            {
                Status = CharityStatus
            };

            _applyApiClient.Setup(x => x.GetCharityCommissionDetails(applicationId)).ReturnsAsync(charityDetails);

            var request = new GetOrganisationStatusRequest(applicationId, UserName);

            var response = _orchestrator.GetOrganisationStatusViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(ProviderStatusWithCapitalisation, viewModel.UkrlpStatus);
            Assert.IsNull(viewModel.CompaniesHouseStatus);
            Assert.AreEqual(CharityStatusWithCapitalisation, viewModel.CharityCommissionStatus);
            Assert.AreEqual(ukprn, viewModel.Ukprn);

            _applyApiClient.Verify(x => x.GetUkrlpDetails(applicationId), Times.Once);
            _applyApiClient.Verify(x => x.GetCompaniesHouseDetails(applicationId), Times.Never);
            _applyApiClient.Verify(x => x.GetCharityCommissionDetails(applicationId), Times.Once);
        }
    }
}
