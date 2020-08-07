using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Infrastructure.Apply;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.OrganisationChecks.Orchestrator
{
    [TestFixture]
    public class OrganisationRiskTests
    {
        private GatewayOrganisationChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOrganisationChecksOrchestrator>> _logger;
        private Mock<IRoatpOrganisationSummaryApiClient> _organisationSummaryApiClient;
        private static string ukprn = "12345678";
        private static string UKRLPLegalName = "John LTD.";
        private static string UserName = "GatewayUser";
        private static string CompanyNumber = "12345678";
        private static string CharityNumber = "87654321";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _logger = new Mock<ILogger<GatewayOrganisationChecksOrchestrator>>();
            _organisationSummaryApiClient = new Mock<IRoatpOrganisationSummaryApiClient>();
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, _organisationSummaryApiClient.Object, _logger.Object);
        }

        [TestCase("Company and charity", "John Training and Consultancy")]
        [TestCase(null, "John Training and Consultancy")]
        [TestCase("Company and charity", null)]
        [TestCase(null, null)]
        public void Check_organisation_risk_details_are_returned(string organisationType, string tradingName)
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.OrganisationRisk;

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                CheckedOn = DateTime.Now, 
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(applicationId, pageId, UserName)).ReturnsAsync(commonDetails);

            _organisationSummaryApiClient.Setup(x => x.GetTypeOfOrganisation(applicationId)).ReturnsAsync(organisationType);
            _organisationSummaryApiClient.Setup(x => x.GetCharityNumber(applicationId)).ReturnsAsync(CharityNumber);
            _organisationSummaryApiClient.Setup(x => x.GetCompanyNumber(applicationId)).ReturnsAsync(CompanyNumber);
            _applyApiClient.Setup(x => x.GetTradingName(applicationId)).ReturnsAsync(tradingName);
            
            var request = new GetOrganisationRiskRequest(applicationId, UserName);

            var response = _orchestrator.GetOrganisationRiskViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(UKRLPLegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
            Assert.AreEqual(organisationType, viewModel.OrganisationType);
            Assert.AreEqual(tradingName, viewModel.TradingName);
            Assert.AreEqual(CompanyNumber, viewModel.CompanyNumber);
            Assert.AreEqual(CharityNumber, viewModel.CharityNumber);

        }
    }
}
