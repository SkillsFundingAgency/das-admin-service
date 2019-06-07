using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Private;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_ukprn_page : CertificatePostBase
    {
        private RedirectToActionResult _result;        

        [SetUp]
        public void WhenValidModelContainsNoErrors()
        {
            var certificatePrivateProviderUkprnController =
                new CertificatePrivateProviderUkprnController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockApiClient                    
                    );

            var vm = new CertificateUkprnViewModel
            {
                Id = Certificate.Id,
                Ukprn = "10000008",                            
                IsPrivatelyFunded = true,
                ReasonForChange = "Required reason for change"
            };                      

            var result = certificatePrivateProviderUkprnController.Ukprn(vm).GetAwaiter().GetResult();

            _result = result as RedirectToActionResult;
        }

        [Test]
        public void ThenShouldReturnRedirectToController()
        {
            _result.ControllerName.Should().Be("CertificateAmend");
        }

        [Test]
        public void ThenShouldReturnRedirectToAction()
        {
            _result.ActionName.Should().Be("Check");
        }
    }
}

