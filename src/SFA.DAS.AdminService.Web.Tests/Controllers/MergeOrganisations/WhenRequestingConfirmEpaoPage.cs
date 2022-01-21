using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenRequestingConfirmEpaoPage : MergeControllerTestBase
    {
        private EpaOrganisation _epaOrganisation;

        [SetUp]
        public void Arrange()
        {
            BaseArrange();

            SetupGetEpaoOrganisation();
        }

        [TestCase("primary")]
        [TestCase("secondary")]
        public async Task Then_GetEpaOrganisationData(string type)
        {
            var epaoId = _autoFixture.Create<string>();

            await MergeController.ConfirmEpao(type, epaoId, null);

            _mockApiClient.Verify(c => c.GetEpaOrganisation(epaoId), Times.Once());
        }

        private void SetupGetEpaoOrganisation()
        {
            _epaOrganisation = _autoFixture.Create<EpaOrganisation>();

            _mockApiClient.Setup(c => c.GetEpaOrganisation(It.IsAny<string>()))
                .ReturnsAsync(_epaOrganisation);
        }
    }
}
