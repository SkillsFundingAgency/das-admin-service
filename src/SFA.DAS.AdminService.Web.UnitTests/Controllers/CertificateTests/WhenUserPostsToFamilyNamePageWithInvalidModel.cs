using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests
{
    public class WhenUserPostsToFamilyNamePageWithInvalidModel : CertificateAmendQueryBase
    {     
        private IActionResult _result;
        
        [SetUp]
        public void Arrange()
        {
            var controller = new CertificateNamesController(MockedLogger.Object, MockHttpContextAccessor.Object, CertificateApiClient, LearnerDetailApiClient, OrganisationsApiClient, ScheduleApiClient, StandardVersionApiClient);
            var invalidModel = new CertificateFamilyNameViewModel { Id = Certificate.Id, ReasonForChange = "Reason Ok but no name" };
            controller.ModelState.AddModelError("FamilyName", "Family name cannot be null");
            _result = controller.FamilyName(invalidModel).GetAwaiter().GetResult();

        }

        [Test]
        public void ThenShouldReturnInvalidModelWithOneError()
        {
            var result = _result as ViewResult;
            result.ViewData.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public void ThenShouldReturnFamilyNameView()
        {
            var result = _result as ViewResult;
            result.ViewName.Should().Be("~/Views/CertificateAmend/FamilyName.cshtml");
        }
    }   
}
