using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests
{
    public class WhenUserPostsToGivenNamesPageWithValidModel : CertificateAmendQueryBase
    {
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            var controller = new CertificateNamesController(MockedLogger.Object, MockHttpContextAccessor.Object, ApiClient);
            var validModel = new CertificateGivenNamesViewModel { Id = Certificate.Id, GivenNames = "Testing", ReasonForChange = "Change Name" };
            _result = controller.GivenNames(validModel).GetAwaiter().GetResult();
        }

        [Test]
        public void ThenShouldReturnCheckView()
        {
            var result = _result as RedirectToActionResult;
            result.ActionName.Should().Be("Check");
            result.ControllerName.Should().Be("CertificateAmend");
        }
    }
}
