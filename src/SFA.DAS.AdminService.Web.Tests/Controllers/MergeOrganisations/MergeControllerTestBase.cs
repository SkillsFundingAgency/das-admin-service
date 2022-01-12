using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.Merge;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class MergeControllerTestBase
    {
        protected Fixture _autoFixture;
        protected Mock<IApiClient> _mockApiClient;
        protected Mock<IMergeOrganisationSessionService> _mockMergeSessionService;

        protected MergeOrganisationsController MergeController;

        [SetUp]
        public void BaseArrange()
        {
            _autoFixture = new Fixture();

            _mockApiClient = new Mock<IApiClient>();
            _mockMergeSessionService = new Mock<IMergeOrganisationSessionService>();

            MergeController = new MergeOrganisationsController(_mockApiClient.Object, _mockMergeSessionService.Object);
        }
    }
}
