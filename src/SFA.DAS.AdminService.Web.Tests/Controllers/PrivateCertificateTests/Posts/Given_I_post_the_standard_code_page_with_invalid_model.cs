using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.ExternalApis.Services;
using SFA.DAS.AdminService.Web.Controllers.Private;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_standard_code_page_with_invalid_model : CertificatePostBase
    {
        private ViewResult _result;

        [SetUp]
        public void WhenInvalidModelContainsOneError()
        {
            var distributedCacheMock = new Mock<IDistributedCache>();

            var certificatePrivateStandardCodeController =
                new CertificatePrivateStandardCodeController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    new CacheService(distributedCacheMock.Object),
                    MockApiClient,
                    MockStandardServiceClient.Object, MockOrganisationsApiClient);

            var vm = new CertificateStandardCodeListViewModel
            {
                Id = Certificate.Id,
                FullName = "James Corley",
                SelectedStandardCode = "93",
                IsPrivatelyFunded = true,
                ReasonForChange = "Required reason for change"
            };

            MockSession.Setup(q => q.Get("EndPointAsessorOrganisationId"))
                .Returns("EPA00001");

            // view model validation errors will not be trigged as they are attached via fluent
            // validation and these are not connected in tests however this test is actually testing
            // the correct view is returned so the actual error is irrelevant and can be forced
            certificatePrivateStandardCodeController.ModelState.AddModelError("", "Error");

            var result = certificatePrivateStandardCodeController.StandardCode(vm).GetAwaiter().GetResult();

            _result = result as ViewResult;
        }

        [Test]
        public void ThenShouldReturnInvalidModelWithOneError()
        {
            _result.ViewData.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public void ThenShouldReturnStandardCodeView()
        {
            _result.ViewName.Should().Be("~/Views/CertificateAmend/StandardCode.cshtml");
        }
    }
}

