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
    public class LegalNameTests
    {
        private GatewayOrganisationChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOrganisationChecksOrchestrator>> _logger;


        private static string PageId => "1-10";
        private GatewayPageAnswer _gatewayPageAnswer;
        private static string ukprn => "12344321";
        private static string UKRLPLegalName => "Mark's workshop";

        private static string CompanyNumber => "654321";
        private static string CharityNumber => "123456";

        private static string ProviderName => "Mark's other workshop";
        private static string CompanyName => "Companies House Name";

        private static string CharityName => "Charity commission Name";

        private static string UserName = "GatewayUser";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _logger = new Mock<ILogger<GatewayOrganisationChecksOrchestrator>>();
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, _logger.Object);
        }

        [Test]
        public void check_legal_name_orchestrator_builds_with_company_and_charity_details()
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
                ProviderName = ProviderName
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(It.IsAny<Guid>())).ReturnsAsync(ukrlpDetails);

            var companiesHouseDetails = new CompaniesHouseSummary
            {
                CompanyName = CompanyName
            };
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(It.IsAny<Guid>())).ReturnsAsync(companiesHouseDetails);

            var charityDetails = new CharityCommissionSummary
            {
                CharityName = CharityName
            };
            _applyApiClient.Setup(x => x.GetCharityCommissionDetails(It.IsAny<Guid>())).ReturnsAsync(charityDetails);

            var request = new GetLegalNameRequest(applicationId, UserName);

            var response = _orchestrator.GetLegalNameViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(UKRLPLegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(ProviderName, viewModel.UkrlpLegalName);
            Assert.AreEqual(CompanyName, viewModel.CompaniesHouseLegalName);
            Assert.AreEqual(CharityName, viewModel.CharityCommissionLegalName);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
        }

        [Test]
        public void check_legal_name_orchestrator_builds_with_company_details_only()
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
                ProviderName = ProviderName
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(It.IsAny<Guid>())).ReturnsAsync(ukrlpDetails);

            var companiesHouseDetails = new CompaniesHouseSummary
            {
                CompanyName = CompanyName
            };
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(It.IsAny<Guid>())).ReturnsAsync(companiesHouseDetails);

            CharityCommissionSummary charityDetails = null;
            _applyApiClient.Setup(x => x.GetCharityCommissionDetails(It.IsAny<Guid>())).ReturnsAsync(charityDetails);

            var request = new GetLegalNameRequest(applicationId, UserName);

            var response = _orchestrator.GetLegalNameViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(UKRLPLegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(ProviderName, viewModel.UkrlpLegalName);
            Assert.AreEqual(CompanyName, viewModel.CompaniesHouseLegalName);
            Assert.IsNull(viewModel.CharityCommissionLegalName);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
        }

        [Test]
        public void check_legal_name_orchestrator_builds_with_charity_details_only()
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
                ProviderName = ProviderName
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(It.IsAny<Guid>())).ReturnsAsync(ukrlpDetails);

            CompaniesHouseSummary companiesHouseDetails = null;
            _applyApiClient.Setup(x => x.GetCompaniesHouseDetails(It.IsAny<Guid>())).ReturnsAsync(companiesHouseDetails);

            var charityDetails = new CharityCommissionSummary
            {
                CharityName = CharityName
            };
            _applyApiClient.Setup(x => x.GetCharityCommissionDetails(It.IsAny<Guid>())).ReturnsAsync(charityDetails);

            var request = new GetLegalNameRequest(applicationId, UserName);

            var response = _orchestrator.GetLegalNameViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(UKRLPLegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(ProviderName, viewModel.UkrlpLegalName);
            Assert.IsNull(viewModel.CompaniesHouseLegalName);
            Assert.AreEqual(CharityName, viewModel.CharityCommissionLegalName);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
        }

    }
}
