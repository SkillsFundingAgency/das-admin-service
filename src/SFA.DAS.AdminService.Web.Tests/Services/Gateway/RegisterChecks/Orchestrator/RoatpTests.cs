using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrganisationRegisterStatus = SFA.DAS.AssessorService.Api.Types.Models.Roatp.OrganisationRegisterStatus;
using OrganisationStatus = SFA.DAS.AssessorService.Api.Types.Models.Roatp.OrganisationStatus;
using ProviderType = SFA.DAS.AssessorService.Api.Types.Models.Roatp.ProviderType;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.RegisterChecks.Orchestrator
{
    [TestFixture]
    public class RoatpTests
    {
        private GatewayRegisterChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpApiClient> _roatpApiClient;
        private Mock<ILogger<GatewayRegisterChecksOrchestrator>> _logger;

        private static string ukprn = "12345678";
        private static string UKRLPLegalName = "John's workshop";
        private static string UserName = "GatewayUser";

        private static int roatpProviderTypeId = 1;
        private static int roatpStatusId = 1;

        private readonly Guid _applicationId = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _roatpApiClient = new Mock<IRoatpApiClient>();
            _logger = new Mock<ILogger<GatewayRegisterChecksOrchestrator>>();
            _orchestrator = new GatewayRegisterChecksOrchestrator(_applyApiClient.Object, _roatpApiClient.Object, _logger.Object);

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                CheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            var providerType = new ProviderType
            {
                Id = roatpProviderTypeId,
                Type = $"{roatpProviderTypeId}"
            };
            _roatpApiClient.Setup(x => x.GetProviderTypes()).ReturnsAsync(new List<ProviderType> { providerType });

            var organisationStatus = new OrganisationStatus
            {
                Id = roatpStatusId,
                Status = $"{roatpStatusId}"
            };
            _roatpApiClient.Setup(x => x.GetOrganisationStatuses(It.IsAny<int?>())).ReturnsAsync(new List<OrganisationStatus> { organisationStatus });
        }

        [Test]
        public async Task check_orchestrator_builds_with_roatp_not_on_register_details()
        {
            var organisationRegisterStatus = new OrganisationRegisterStatus
            {
                UkprnOnRegister = false
            };
            _roatpApiClient.Setup(x => x.GetOrganisationRegisterStatus(ukprn.ToString())).ReturnsAsync(organisationRegisterStatus);

            var request = new GetRoatpRequest(_applicationId, UserName);

            var viewModel = await _orchestrator.GetRoatpViewModel(request);

            Assert.IsFalse(viewModel.RoatpUkprnOnRegister);
            Assert.IsNull(viewModel.RoatpStatus);
            Assert.IsNull(viewModel.RoatpStatusDate);
            Assert.IsNull(viewModel.RoatpProviderRoute);
        }

        [Test]
        public async Task check_orchestrator_builds_with_roatp_on_register_details()
        {
            var organisationRegisterStatus = new OrganisationRegisterStatus
            {
                UkprnOnRegister = true,
                StatusId = roatpStatusId,
                ProviderTypeId = roatpProviderTypeId,
                StatusDate = DateTime.Now.AddMonths(-1)
            };
            _roatpApiClient.Setup(x => x.GetOrganisationRegisterStatus(ukprn.ToString())).ReturnsAsync(organisationRegisterStatus);

            var request = new GetRoatpRequest(_applicationId, UserName);

            var viewModel = await _orchestrator.GetRoatpViewModel(request);

            Assert.IsTrue(viewModel.RoatpUkprnOnRegister);
            Assert.IsNotNull(viewModel.RoatpStatus);
            Assert.IsNotNull(viewModel.RoatpStatusDate);
            Assert.IsNotNull(viewModel.RoatpProviderRoute);
        }
    }
}
