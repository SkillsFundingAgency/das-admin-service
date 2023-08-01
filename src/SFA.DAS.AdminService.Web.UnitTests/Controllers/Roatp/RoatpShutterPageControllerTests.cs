using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Roatp
{

    [TestFixture]
    public class RoatpShutterPageControllerTests
    {
        private RoatpShutterPageController _controller;
        private Mock<IWebConfiguration> _configuration;

        private string _gatewayUrl;

        [SetUp]
        public void Setup()
        {
            _configuration = new Mock<IWebConfiguration>();
            _gatewayUrl = "http://gateway";

            _configuration.Setup(c => c.RoatpGatewayBaseUrl).Returns(_gatewayUrl);


            _controller = new RoatpShutterPageController(_configuration.Object);
        }

        [Test]
        public async Task confirm_viewModel_is_as_expected()
        {
            var result = await _controller.ExternalApisUnavailable() as ViewResult;

            var viewModel = result.Model as ExternalApisUnavailableViewModel;

            Assert.That(_gatewayUrl, Is.EqualTo(viewModel.RoatpGatewayBaseUrl));
            
        }
    }
}
