using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
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
        private Mock<IQnaApiClient> _qnaApiClient;

        private static string PageId => "1-10";
        private GatewayPageAnswer _gatewayPageAnswer;
        private static string ukprn => "12344321";
        private static string UKRLPLegalName => "Mark's workshop";

        private static string CompanyNumber => "654321";
        private static string CharityNumber => "123456";

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
            _qnaApiClient = new Mock<IQnaApiClient>();
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, _qnaApiClient.Object, _logger.Object);
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
            _applyApiClient.Setup(x => x.GetPageCommonDetails(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            var ukrlpDetails = new ProviderDetails
            {
                ProviderName = ProviderName,
                ProviderStatus = ProviderStatus
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(It.IsAny<Guid>())).ReturnsAsync(ukrlpDetails);

            var companiesHouseDetails = new CompaniesHouseSummary
            {
                Status = CompanyStatus
            };
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(It.IsAny<Guid>())).ReturnsAsync(companiesHouseDetails);

            var charityDetails = new CharityCommissionSummary
            {
                Status = CharityStatus
            };
            _applyApiClient.Setup(x => x.GetCharityCommissionDetails(It.IsAny<Guid>())).ReturnsAsync(charityDetails);

            var request = new GetOrganisationStatusRequest(applicationId, UserName);

            var response = _orchestrator.GetOrganisationStatusViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(ProviderStatusWithCapitalisation, viewModel.UkrlpStatus);
            Assert.AreEqual(CompanyStatusWithCapitalisation, viewModel.CompaniesHouseStatus);
            Assert.AreEqual(CharityStatusWithCapitalisation, viewModel.CharityCommissionStatus);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
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
            _applyApiClient.Setup(x => x.GetPageCommonDetails(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            var ukrlpDetails = new ProviderDetails
            {
                ProviderName = ProviderName,
                ProviderStatus = ProviderStatus
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(It.IsAny<Guid>())).ReturnsAsync(ukrlpDetails);

            var companiesHouseDetails = new CompaniesHouseSummary
            {
                Status = CompanyStatus
            };
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(It.IsAny<Guid>())).ReturnsAsync(companiesHouseDetails);

            var charityDetails = new CharityCommissionSummary
            {
                Status = null
            };

            _applyApiClient.Setup(x => x.GetCharityCommissionDetails(It.IsAny<Guid>())).ReturnsAsync(charityDetails);

            var request = new GetOrganisationStatusRequest(applicationId, UserName);

            var response = _orchestrator.GetOrganisationStatusViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(ProviderStatusWithCapitalisation, viewModel.UkrlpStatus);
            Assert.AreEqual(CompanyStatusWithCapitalisation, viewModel.CompaniesHouseStatus);
            Assert.AreEqual(string.Empty,viewModel.CharityCommissionStatus);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
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
            _applyApiClient.Setup(x => x.GetPageCommonDetails(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            var ukrlpDetails = new ProviderDetails
            {
                ProviderName = ProviderName,
                ProviderStatus = ProviderStatus
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(It.IsAny<Guid>())).ReturnsAsync(ukrlpDetails);

            var companiesHouseDetails = new CompaniesHouseSummary
            {
                Status = null
            };
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(It.IsAny<Guid>())).ReturnsAsync(companiesHouseDetails);

            var charityDetails = new CharityCommissionSummary
            {
                Status = CharityStatus
            };

            _applyApiClient.Setup(x => x.GetCharityCommissionDetails(It.IsAny<Guid>())).ReturnsAsync(charityDetails);

            var request = new GetOrganisationStatusRequest(applicationId, UserName);

            var response = _orchestrator.GetOrganisationStatusViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(ProviderStatusWithCapitalisation, viewModel.UkrlpStatus);
            Assert.AreEqual(string.Empty, viewModel.CompaniesHouseStatus);
            Assert.AreEqual(CharityStatusWithCapitalisation, viewModel.CharityCommissionStatus);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
        }
    }
}
