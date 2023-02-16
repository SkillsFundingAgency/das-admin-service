using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Api.Types.Enums;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests
{
    public class CertificateAmendControllerTests
    {
        [SetUp]
        public void Arrange()
        {
        }

        [Test]
        [MoqAutoData]
        public async Task WhenPostAmendReason_WithModelError_RedirectsToGetAmendReasonAction(
            CertificateAmendReasonViewModel vm)
        {
            // Arrange
            var fixture = new CertificateAmendControllerTestsFixture()
                .WithModelError();

            // Act
            var result = await fixture.AmendReason(vm);

            // Assert
            fixture.VerifyRedirectToAction(result, "AmendReason", null, new { StdCode = vm.Learner.StandardCode, vm.Learner.Uln });
        }

        [Test]
        [MoqAutoData]
        public async Task WhenPostAmendReason_WithoutModelError_RedirectsToGetCheckAction(
            CertificateAmendReasonViewModel vm)
        {
            // Arrange
            vm.Reasons = new List<string> { AmendReasons.ApprenticeAddress.ToString() };
            var fixture = new CertificateAmendControllerTestsFixture();

            // Act
            var result = await fixture.AmendReason(vm);

            // Assert
            fixture.VerifyRedirectToAction(result, "Check", null, new { vm.Learner.CertificateId });
        }

        [Test]
        [MoqAutoData]
        public async Task WhenPostAmendReason_WithoutModelError_CallsApiToUpdateCertificate(
            CertificateAmendReasonViewModel vm)
        {
            // Arrange
            vm.Reasons = new List<string> { AmendReasons.ApprenticeAddress.ToString() };
            var fixture = new CertificateAmendControllerTestsFixture();

            // Act
            await fixture.AmendReason(vm);

            // Assert
            fixture.VerifyUpdateCertificateWithAmendReasonCalled(vm.Learner.CertificateReference);
        }

        [Test]
        [MoqAutoData]
        public async Task WhenPostReprintReason_WithModelError_RedirectsToGetReprintReasonAction(
            CertificateReprintReasonViewModel vm)
        {
            // Arrange
            var fixture = new CertificateAmendControllerTestsFixture()
                .WithModelError();

            // Act
            var result = await fixture.ReprintReason(vm);

            // Assert
            fixture.VerifyRedirectToAction(result, "ReprintReason", null, new { StdCode = vm.Learner.StandardCode, vm.Learner.Uln });
        }

        [Test]
        [MoqAutoData]
        public async Task WhenPostReprintReason_WithoutModelError_RedirectsToGetCheckAction(
            CertificateReprintReasonViewModel vm)
        {
            // Arrange
            vm.Reasons = new List<string> { ReprintReasons.ApprenticeAddress.ToString() };
            var fixture = new CertificateAmendControllerTestsFixture();

            // Act
            var result = await fixture.ReprintReason(vm);

            // Assert
            fixture.VerifyRedirectToAction(result, "Check", null, new { vm.Learner.CertificateId });
        }

        [Test]
        [MoqAutoData]
        public async Task WhenPostReprintReason_WithoutModelError_CallsApiToUpdateCertificate(
            CertificateReprintReasonViewModel vm)
        {
            // Arrange
            vm.Reasons = new List<string> { ReprintReasons.ApprenticeAddress.ToString() };
            var fixture = new CertificateAmendControllerTestsFixture();

            // Act
            await fixture.ReprintReason(vm);

            // Assert
            fixture.VerifyUpdateCertificateWithReprintReasonCalled(vm.Learner.CertificateReference);
        }

        [Test]
        [MoqAutoData]
        public void WhenCheckIsInvoked_AndHasModelError_RedirectsToGetCheckAction(
            CertificateCheckViewModel vm,
            StandardOptions options)
        {
            // Arrange
            vm.StandardUId = null;
            vm.Option = null;

            var fixture = new CertificateAmendControllerTestsFixture()
                .WithModelError();

            // Act
            var result = fixture.Check(vm);

            // Assert
            fixture.VerifyRedirectToAction(result, "Check", null, new 
            { 
                CertificateId = vm.Id, 
                vm.SearchString, 
                vm.Page 
            });
        }

        [Test]
        [MoqAutoData]
        public void WhenCheckIsInvoked_AndStatusIsSubmitted_RedirectsToConfirmAmend(
            CertificateCheckViewModel vm)
        {
            // Arrange
            vm.Status = CertificateStatus.Submitted;
            var fixture = new CertificateAmendControllerTestsFixture();

            // Act
            var result = fixture.Check(vm);

            // Assert
            fixture.VerifyRedirectToAction(result, "ConfirmAmend", null, new
            {
                certificateId = vm.Id,
                redirectToCheck = vm.RedirectToCheck,
                Uln = vm.Uln,
                StdCode = vm.StandardCode,
                Page = vm.Page,
                SearchString = vm.SearchString
            });
        }

        [Test]
        [MoqAutoData]
        public void WhenCheckIsInvoked_AndStatusIsPrinted_RedirectsToConfirmReprint(
            CertificateCheckViewModel vm)
        {
            // Arrange
            vm.Status = CertificateStatus.Printed;
            var fixture = new CertificateAmendControllerTestsFixture();

            // Act
            var result = fixture.Check(vm);

            // Assert
            fixture.VerifyRedirectToAction(result, "ConfirmReprint", null, new
            {
                certificateId = vm.Id,
                redirectToCheck = vm.RedirectToCheck,
                Uln = vm.Uln,
                StdCode = vm.StandardCode,
                Page = vm.Page,
                SearchString = vm.SearchString
            });
        }

        public class CertificateAmendControllerTestsFixture
        {
            private Fixture _fixture;
            private Certificate _certificate;
            private Organisation _organisation;
            private List<StandardVersion> _standardVersions;

            private CertificateAmendController _sut;

            private Mock<IApiClient> _apiClient;
            private Mock<ILogger<CertificateAmendController>> _logger;
            
            public CertificateAmendControllerTestsFixture()
            {
                _fixture = new Fixture();
                _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
                
                _apiClient = new Mock<IApiClient>();
                _logger = new Mock<ILogger<CertificateAmendController>>();

                _sut = new CertificateAmendController(_logger.Object, SetupMockHttpAccessor().Object, _apiClient.Object)
                {
                    TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
                };
            }

            public CertificateAmendControllerTestsFixture WithLearner(LearnerDetailResult learnerDetailResult)
            {
                _apiClient.Setup(p => p.GetLearner(learnerDetailResult.StandardCode, learnerDetailResult.Uln, It.IsAny<bool>())).ReturnsAsync(learnerDetailResult);

                return this;
            }

            public CertificateAmendControllerTestsFixture WithModelError()
            {
                _sut.ModelState.AddModelError("key", "errorMessage");
                return this;
            }

            public IActionResult Check(CertificateCheckViewModel viewModel)
            {
                return _sut.Check(viewModel);
            }

            public async Task<IActionResult> AmendReason(CertificateAmendReasonViewModel viewModel)
            {
                return await _sut.AmendReason(viewModel);
            }

            public async Task<IActionResult> ReprintReason(CertificateReprintReasonViewModel viewModel)
            {
                return await _sut.ReprintReason(viewModel);
            }

            public void VerifyUpdateCertificateWithAmendReasonCalled(string certificateRefernce)
            {
                _apiClient.Verify(p => p.UpdateCertificateWithAmendReason(It.Is<UpdateCertificateWithAmendReasonCommand>(cmd => cmd.CertificateReference == certificateRefernce)), Times.Once);
            }

            public void VerifyUpdateCertificateWithReprintReasonCalled(string certificateRefernce)
            {
                _apiClient.Verify(p => p.UpdateCertificateWithReprintReason(It.Is<UpdateCertificateWithReprintReasonCommand>(cmd => cmd.CertificateReference == certificateRefernce)), Times.Once);
            }

            public void VerifyRedirectToAction(IActionResult result, string action, string controller, dynamic routeValues = null)
            {
                var redirectResult = result as RedirectToActionResult;
                redirectResult.ActionName.Should().Be(action);
                redirectResult.ControllerName.Should().Be(controller);

                if (routeValues != null)
                {
                    ExpandoObject eoRouteValues = JsonSerializer.Deserialize<ExpandoObject>(JsonSerializer.Serialize(routeValues));
                    foreach (var kvp in eoRouteValues)
                    {
                        redirectResult.RouteValues.TryGetValue(kvp.Key, out object value);
                        value.ToString().Should().Be(kvp.Value.ToString());
                    }
                }
                else
                {
                    redirectResult.RouteValues.Should().BeNull();
                }
            }

            public void VerifyModelError(IActionResult result, string key, string errorMessage)
            {
                var assertResult = result as ViewResult;
                assertResult.ViewData.ModelState.IsValid.Should().BeFalse();
                assertResult.ViewData.ModelState.ErrorCount.Should().Be(1);
                assertResult.ViewData.ModelState.Keys.Should().Contain(key);
                var errorEnumerator = assertResult.ViewData.ModelState.Values.GetEnumerator();
                errorEnumerator.MoveNext();
                errorEnumerator.Current.Errors[0].ErrorMessage.Should().Be(errorMessage);
            }

            private Mock<IHttpContextAccessor> SetupMockHttpAccessor()
            {
                var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "username")
                }));

                var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
                mockHttpContextAccessor.Setup(p => p.HttpContext).Returns(new DefaultHttpContext { User = user });

                return mockHttpContextAccessor;
            }
        }
    }
}
