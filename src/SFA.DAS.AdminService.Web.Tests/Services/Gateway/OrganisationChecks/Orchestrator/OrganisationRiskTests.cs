using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;
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

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.OrganisationChecks.Orchestrator
{
    [TestFixture]
    public class OrganisationRiskTests
    {
        private GatewayOrganisationChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOrganisationChecksOrchestrator>> _logger;

        private static string ukprn = "10026709";
        private static string UKRLPLegalName = "EVA WOMENS AID LTD.";
        private static string UserName = "GatewayUser";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _logger = new Mock<ILogger<GatewayOrganisationChecksOrchestrator>>();
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, _logger.Object);
        }

        [TestCase("Company and charity", "Eva Training and Consultancy")]
        [TestCase(null, "Eva Training and Consultancy")]
        [TestCase("Company and charity", null)]
        [TestCase(null, null)]
        public void Check_orchestrator_builds_with_organisation_risk(string organisationType, string tradingName)
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

            _applyApiClient.Setup(x => x.GetTypeOfOrganisation(applicationId)).ReturnsAsync(organisationType);
            _applyApiClient.Setup(x => x.GetTradingName(applicationId)).ReturnsAsync(tradingName);
            
            var request = new GetOrganisationRiskRequest(applicationId, UserName);

            var response = _orchestrator.GetOrganisationRiskViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(UKRLPLegalName, viewModel.UkrlpLegalName);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
            Assert.AreEqual(organisationType, viewModel.OrganisationType);
            Assert.AreEqual(tradingName, viewModel.TradingName);
        }
    }
}
