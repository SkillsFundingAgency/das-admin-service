using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Infrastructure.Apply;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.OrganisationChecks.Orchestrator
{
    [TestFixture]
    public class WebsiteAddressTests
    {
        private GatewayOrganisationChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOrganisationChecksOrchestrator>> _logger;

        private static string ukprn = "12345678";
        private static string UKRLPLegalName = "John's workshop";
        private static string UserName = "GatewayUser";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _logger = new Mock<ILogger<GatewayOrganisationChecksOrchestrator>>();
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, Mock.Of<IRoatpOrganisationSummaryApiClient>(), _logger.Object);
        }

        [TestCase("http://www.OrganisationWebSite.co.uk", "http://www.UkrlpApiWebsite.co.uk")]
        [TestCase(null, "http://www.UkrlpApiWebsite.co.uk")]
        [TestCase("http://www.OrganisationWebSite.co.uk", null)]
        [TestCase(null, null)]
        public void check_orchestrator_builds_with_website_address(string organisationWebsite, string ukrlpApiWebsite)
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

            _applyApiClient.Setup(x => x.GetOrganisationWebsiteAddress(applicationId)).ReturnsAsync(organisationWebsite);
            
            var ukrlpDetails = new ProviderDetails
            {
                ContactDetails = new List<ProviderContact>()
                {
                    new ProviderContact 
                    { 
                        ContactWebsiteAddress = ukrlpApiWebsite, 
                        ContactType = RoatpGatewayConstants.ProviderContactDetailsTypeLegalIdentifier
                    }
                }
            };
            
            _applyApiClient.Setup(x => x.GetUkrlpDetails(applicationId)).ReturnsAsync(ukrlpDetails);

            var request = new GetWebsiteRequest(applicationId, UserName);

            var response = _orchestrator.GetWebsiteViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(UKRLPLegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
            Assert.AreEqual(organisationWebsite, viewModel.SubmittedWebsite);
            Assert.AreEqual(ukrlpApiWebsite, viewModel.UkrlpWebsite);
        }
    }
}
