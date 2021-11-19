using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Tests.Controllers;
using SFA.DAS.AdminService.Web.ViewModels;
using Moq;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using SFA.DAS.AssessorService.Application.Api.Client;
using System;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using AutoFixture;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests
{
    public class CertificateAmendControllerTests
    {

        private Mock<ILogger<CertificateAmendController>> _logger;
        private MockHttpMessageHandler _mockHttp;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger<CertificateAmendController>>();
            _mockHttp = new MockHttpMessageHandler();
        }


        [Test]
        [MoqAutoData]
        public async Task WhenConfirmAndSubmitIsInvoked_AndStandardUIdNotInModel_ButHasOptions_ReturnsOptionError(
            CertificateCheckViewModel vm,
            StandardOptions options)
        {
            //Setup
            vm.StandardUId = null;
            // Setup Options Call before HttpClientMock is instantiated from configuration.
            // Setup StandardCode call as StandardUID is empty
            _mockHttp.When($"http://localhost:59022/api/v1/standard-version/standard-options/{vm.StandardCode}")
                .Respond("application/json", JsonConvert.SerializeObject(options));
            //Set option to not set
            vm.Option = null;

            var sut = SetupController(vm);

            // Act
            var result = await sut.ConfirmAndSubmit(vm);

            // Assert
            var assertResult = result as ViewResult;
            assertResult.ViewData.ModelState.IsValid.Should().BeFalse();
            assertResult.ViewData.ModelState.ErrorCount.Should().Be(1);
            assertResult.ViewData.ModelState.Keys.Should().Contain("Option");
            var errorEnumerator = assertResult.ViewData.ModelState.Values.GetEnumerator();
            errorEnumerator.MoveNext();
            errorEnumerator.Current.Errors[0].ErrorMessage.Should().Be("Add an option");
        }

        [Test]
        [MoqAutoData]
        public async Task WhenConfirmAndSubmitIsInvoked_AndStandardUIdIsInModel_ButHasOptions_ReturnsRedirect(
            CertificateCheckViewModel vm,
            StandardOptions options)
        {
            //Setup
            // Setup Options Call before HttpClientMock is instantiated from configuration.
            // Setup StandardCode call as StandardUID is empty
            _mockHttp.When($"http://localhost:59022/api/v1/standard-version/standard-options/{vm.StandardUId}")
                .Respond("application/json", JsonConvert.SerializeObject(options));
            
            var sut = SetupController(vm);

            // Act
            var result = await sut.ConfirmAndSubmit(vm);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            redirectResult.ControllerName.Should().Be("Comment");
            redirectResult.ActionName.Should().Be("Index");
            
        }

        private CertificateAmendController SetupController(CertificateCheckViewModel vm)
        {
            return new CertificateAmendController(_logger.Object, SetupMockHttpAccessor().Object, SetupApiClient(vm))
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        private ApiClient SetupApiClient(CertificateCheckViewModel vm)
        {
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var organisation = fixture.Create<Organisation>();
            var certificate = fixture.Create<Certificate>();
            certificate.Id = vm.Id;
            certificate.StandardCode = vm.StandardCode;
            certificate.StandardUId = vm.StandardUId;
            var certificateData = fixture.Create<CertificateData>();
            certificateData.CourseOption = vm.Option;
            certificate.CertificateData = JsonConvert.SerializeObject(certificateData);
            var standardVersions = fixture.CreateMany<StandardVersion>();

            _mockHttp.When($"http://localhost:59022/api/v1/certificates/{certificate.Id}")
                    .Respond("application/json", JsonConvert.SerializeObject(certificate));

            _mockHttp.When($"http://localhost:59022/api/v1/organisations/organisation/{certificate.OrganisationId}")
                    .Respond("application/json", JsonConvert.SerializeObject(organisation));

            _mockHttp.When($"http://localhost:59022/api/v1/standard-version/standards/versions/{vm.StandardCode}")
                .Respond("application/json", JsonConvert.SerializeObject(standardVersions));

            var client = _mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost:59022/");
            var tokenServiceMock = new Mock<ITokenService>();
            return new ApiClient(client, tokenServiceMock.Object);
        }

        private Mock<IHttpContextAccessor> SetupMockHttpAccessor()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "unittestupn")
            }));

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext { User = user };
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            return mockHttpContextAccessor;
        }
    }
}
