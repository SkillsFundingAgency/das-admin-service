using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Private;
using SFA.DAS.AdminService.Web.Tests.Helpers;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_learning_startdate_page : CertificatePostBase
    {
        private RedirectToActionResult _result;      

        [SetUp]
        public void WhenTheModelIsValid()
        {
            var mockStringLocaliserBuildernew = new MockStringLocaliserBuilder();
            
            var validator = 
                new CertificateLearnerStartDateViewModelValidator();

            var certificatePrivateLearnerStartDateController =
                new CertificatePrivateLearnerStartDateController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockApiClientFactory.Object,
                    validator                    
                    );

            var vm = new CertificateLearnerStartDateViewModel
            {
                Id = Certificate.Id,
                FullName = "James Corley",
                Day = "12",
                Month = "12",
                Year = "2017",
                IsPrivatelyFunded = true,
                ReasonForChange = "Required reason for change"
            };
                    

            var result = certificatePrivateLearnerStartDateController.LearnerStartDate(vm).GetAwaiter().GetResult();

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

