using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway
{
    [TestFixture]
    public class RoatpOrganisationCriminalComplianceChecksControllerTests
    {
        private RoatpOrganisationCriminalComplianceChecksController _controller;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IHttpContextAccessor> _contextAccessor;
        private Mock<IRoatpGatewayPageViewModelValidator> _gatewayValidator;
        private Mock<IGatewayCriminalComplianceChecksOrchestrator> _orchestrator;
        private Mock<ILogger<RoatpOrganisationCriminalComplianceChecksController>> _logger;

        private string username => "Gateway User";
        private string givenName => "Gateway";
        private string surname => "User";
        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _contextAccessor = new Mock<IHttpContextAccessor>();
            _gatewayValidator = new Mock<IRoatpGatewayPageViewModelValidator>();
            _orchestrator = new Mock<IGatewayCriminalComplianceChecksOrchestrator>();
            _logger = new Mock<ILogger<RoatpOrganisationCriminalComplianceChecksController>>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", username),
                new Claim(ClaimTypes.GivenName, givenName),
                new Claim(ClaimTypes.Surname, surname)
            }));

            var context = new DefaultHttpContext { User = user };
            
            _contextAccessor.Setup(_ => _.HttpContext).Returns(context);
            _controller = new RoatpOrganisationCriminalComplianceChecksController(_applyApiClient.Object, _contextAccessor.Object, _gatewayValidator.Object, _orchestrator.Object, _logger.Object);
        }

        [Test]
        public void Check_Composition_Creditors_Request_Is_Sent()
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()))
                .ReturnsAsync(new OrganisationCriminalCompliancePageViewModel())
                .Verifiable("view model not returned");

            var result = _controller.GetOrganisationCompositionCreditorsPage(applicationId).GetAwaiter().GetResult();
            _orchestrator.Verify(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()), Times.Once());
        }

        [Test]
        public void Composition_with_creditors_check_posted()
        {
            var model = new OrganisationCriminalCompliancePageViewModel
            {
                ApplicationId = Guid.NewGuid(),
                ApplyLegalName = "legal name",
                ComplianceCheckQuestionId = "CC-1",
                ComplianceCheckAnswer = "No",
                OptionPassText = null,
                Status = "Pass",
                PageId = "5-10",
                QuestionText = "Question text",
                Ukprn = "10001234"
            };

            _applyApiClient.Setup(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var validationResponse = new ValidationResponse 
            {
                Errors = new List<ValidationErrorDetail>()
            };
            _gatewayValidator.Setup(x => x.Validate(It.IsAny<RoatpGatewayPageViewModel>())).ReturnsAsync(validationResponse);

            var result = _controller.EvaluateOrganisationCompositionCreditorsPage(model).GetAwaiter().GetResult();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("ViewApplication");
        }

        [Test]
        public void Composition_with_creditors_check_has_validation_error()
        {
            var model = new OrganisationCriminalCompliancePageViewModel
            {
                ApplicationId = Guid.NewGuid(),
                ApplyLegalName = "legal name",
                ComplianceCheckQuestionId = "CC-1",
                ComplianceCheckAnswer = "No",
                OptionFailText = null,
                Status = "Fail",
                PageId = "5-10",
                QuestionText = "Question text",
                Ukprn = "10001234"
            };

            _applyApiClient.Setup(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
                {
                    new ValidationErrorDetail
                    {
                        ErrorMessage = "Comments are mandatory",
                        Field = "OptionFailText"
                    }
                }
            };
            _gatewayValidator.Setup(x => x.Validate(It.IsAny<RoatpGatewayPageViewModel>())).ReturnsAsync(validationResponse);

            var result = _controller.EvaluateOrganisationCompositionCreditorsPage(model).GetAwaiter().GetResult();

            var viewResult = result as ViewResult;
            var viewModel = viewResult.Model as OrganisationCriminalCompliancePageViewModel;
            viewModel.Should().NotBeNull();
            viewModel.ErrorMessages.Count.Should().BeGreaterThan(0);
        }
    }
}
