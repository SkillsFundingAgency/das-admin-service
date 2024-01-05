using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests
{
    public class WhenUserPostsToFamilyNamePageWithValidModel : CertificateAmendQueryBase
    {
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            var controller = new CertificateNamesController(MockedLogger.Object, MockHttpContextAccessor.Object, CertificateApiClient, LearnerDetailApiClient, OrganisationsApiClient, ScheduleApiClient, StandardVersionApiClient);
            var validModel = new CertificateFamilyNameViewModel { Id = Certificate.Id, FamilyName = "Testing", ReasonForChange = "Change Name" };
            _result = controller.FamilyName(validModel).GetAwaiter().GetResult();
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
