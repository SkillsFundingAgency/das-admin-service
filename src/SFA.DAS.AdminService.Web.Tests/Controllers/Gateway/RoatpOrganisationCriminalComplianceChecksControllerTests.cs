using FluentAssertions;
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

        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved)]
        public void Criminal_compliance_check_returns_view(string gatewayPageId)
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()))
                .ReturnsAsync(new OrganisationCriminalCompliancePageViewModel())
                .Verifiable("view model not returned");

            var result = _controller.GetCriminalCompliancePage(applicationId, gatewayPageId).GetAwaiter().GetResult();
            _orchestrator.Verify(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()), Times.Once());

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var viewModel = viewResult.Model as OrganisationCriminalCompliancePageViewModel;
            viewModel.Should().NotBeNull();
            viewModel.PageTitle.Should().Be(CriminalCompliancePageConfiguration.Titles[gatewayPageId]);
        }

        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved)]
        public void Criminal_compliance_check_posted(string gatewayPageId)
        {
            var model = new OrganisationCriminalCompliancePageViewModel
            {
                ApplicationId = Guid.NewGuid(),
                ApplyLegalName = "legal name",
                ComplianceCheckQuestionId = "CC-1",
                ComplianceCheckAnswer = "No",
                OptionPassText = null,
                Status = "Pass",
                PageId = gatewayPageId,
                QuestionText = "Question text",
                Ukprn = "10001234"
            };

            _applyApiClient.Setup(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var validationResponse = new ValidationResponse 
            {
                Errors = new List<ValidationErrorDetail>()
            };
            _gatewayValidator.Setup(x => x.Validate(It.IsAny<RoatpGatewayPageViewModel>())).ReturnsAsync(validationResponse);

            var result = _controller.EvaluateCriminalCompliancePage(model).GetAwaiter().GetResult();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("ViewApplication");
        }

        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved)]
        public void Criminal_compliance_check_has_validation_error(string gatewayPageId)
        {
            var model = new OrganisationCriminalCompliancePageViewModel
            {
                ApplicationId = Guid.NewGuid(),
                ApplyLegalName = "legal name",
                ComplianceCheckQuestionId = "CC-1",
                ComplianceCheckAnswer = "No",
                OptionFailText = null,
                Status = "Fail",
                PageId = gatewayPageId,
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

            var result = _controller.EvaluateCriminalCompliancePage(model).GetAwaiter().GetResult();

            var viewResult = result as ViewResult;
            var viewModel = viewResult.Model as OrganisationCriminalCompliancePageViewModel;
            viewModel.Should().NotBeNull();
            viewModel.ErrorMessages.Count.Should().BeGreaterThan(0);
        }

        //[Test]
        //public void Failed_to_repay_funds_check_returns_view()
        //{
        //    var applicationId = Guid.NewGuid();

        //    _orchestrator.Setup(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()))
        //        .ReturnsAsync(new OrganisationCriminalCompliancePageViewModel())
        //        .Verifiable("view model not returned");

        //    var result = _controller.GetOrganisationFailedToRepayFundsPage(applicationId).GetAwaiter().GetResult();
        //    _orchestrator.Verify(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()), Times.Once());

        //    var viewResult = result as ViewResult;
        //    viewResult.Should().NotBeNull();
        //    var viewModel = viewResult.Model as OrganisationCriminalCompliancePageViewModel;
        //    viewModel.Should().NotBeNull();
        //    viewModel.PageTitle.Should().Be(CriminalCompliancePageTitles.OrganisationFailedToRepayFunds);
        //    viewModel.PostBackAction.Should().Be(CriminalCompliancePagePostActions.OrganisationFailedToRepayFunds);
        //}

        //[Test]
        //public void Failed_to_repay_funds_check_posted()
        //{
        //    var model = new OrganisationCriminalCompliancePageViewModel
        //    {
        //        ApplicationId = Guid.NewGuid(),
        //        ApplyLegalName = "legal name",
        //        ComplianceCheckQuestionId = "CC-1",
        //        ComplianceCheckAnswer = "No",
        //        OptionPassText = null,
        //        Status = "Pass",
        //        PageId = GatewayPageIds.CCOrganisationFailedToRepayFunds,
        //        QuestionText = "Question text",
        //        Ukprn = "10001234"
        //    };

        //    _applyApiClient.Setup(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        //    var validationResponse = new ValidationResponse
        //    {
        //        Errors = new List<ValidationErrorDetail>()
        //    };
        //    _gatewayValidator.Setup(x => x.Validate(It.IsAny<RoatpGatewayPageViewModel>())).ReturnsAsync(validationResponse);

        //    var result = _controller.EvaluateOrganisationFailedToRepayFundsPage(model).GetAwaiter().GetResult();

        //    var redirectResult = result as RedirectToActionResult;
        //    redirectResult.Should().NotBeNull();
        //    redirectResult.ActionName.Should().Be("ViewApplication");
        //}

        //[Test]
        //public void Failed_to_repay_funds_check_has_validation_error()
        //{
        //    var model = new OrganisationCriminalCompliancePageViewModel
        //    {
        //        ApplicationId = Guid.NewGuid(),
        //        ApplyLegalName = "legal name",
        //        ComplianceCheckQuestionId = "CC-1",
        //        ComplianceCheckAnswer = "No",
        //        OptionFailText = null,
        //        Status = "Fail",
        //        PageId = GatewayPageIds.CCOrganisationFailedToRepayFunds,
        //        QuestionText = "Question text",
        //        Ukprn = "10001234"
        //    };

        //    _applyApiClient.Setup(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        //    var validationResponse = new ValidationResponse
        //    {
        //        Errors = new List<ValidationErrorDetail>()
        //        {
        //            new ValidationErrorDetail
        //            {
        //                ErrorMessage = "Comments are mandatory",
        //                Field = "OptionFailText"
        //            }
        //        }
        //    };
        //    _gatewayValidator.Setup(x => x.Validate(It.IsAny<RoatpGatewayPageViewModel>())).ReturnsAsync(validationResponse);

        //    var result = _controller.EvaluateOrganisationFailedToRepayFundsPage(model).GetAwaiter().GetResult();

        //    var viewResult = result as ViewResult;
        //    var viewModel = viewResult.Model as OrganisationCriminalCompliancePageViewModel;
        //    viewModel.Should().NotBeNull();
        //    viewModel.ErrorMessages.Count.Should().BeGreaterThan(0);
        //}

        //[Test]
        //public void Contract_terminated_check_returns_view()
        //{
        //    var applicationId = Guid.NewGuid();

        //    _orchestrator.Setup(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()))
        //        .ReturnsAsync(new OrganisationCriminalCompliancePageViewModel())
        //        .Verifiable("view model not returned");

        //    var result = _controller.GetOrganisationContractTerminationPage(applicationId).GetAwaiter().GetResult();
        //    _orchestrator.Verify(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()), Times.Once());

        //    var viewResult = result as ViewResult;
        //    viewResult.Should().NotBeNull();
        //    var viewModel = viewResult.Model as OrganisationCriminalCompliancePageViewModel;
        //    viewModel.Should().NotBeNull();
        //    viewModel.PageTitle.Should().Be(CriminalCompliancePageTitles.OrganisationContractTerminatedByPublicBody);
        //    viewModel.PostBackAction.Should().Be(CriminalCompliancePagePostActions.OrganisationContractTerminatedByPublicBody);
        //}

        //[Test]
        //public void Contract_terminated_check_posted()
        //{
        //    var model = new OrganisationCriminalCompliancePageViewModel
        //    {
        //        ApplicationId = Guid.NewGuid(),
        //        ApplyLegalName = "legal name",
        //        ComplianceCheckQuestionId = "CC-1",
        //        ComplianceCheckAnswer = "No",
        //        OptionPassText = null,
        //        Status = "Pass",
        //        PageId = GatewayPageIds.CCOrganisationContractTermination,
        //        QuestionText = "Question text",
        //        Ukprn = "10001234"
        //    };

        //    _applyApiClient.Setup(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        //    var validationResponse = new ValidationResponse
        //    {
        //        Errors = new List<ValidationErrorDetail>()
        //    };
        //    _gatewayValidator.Setup(x => x.Validate(It.IsAny<RoatpGatewayPageViewModel>())).ReturnsAsync(validationResponse);

        //    var result = _controller.EvaluateOrganisationContractTerminationPage(model).GetAwaiter().GetResult();

        //    var redirectResult = result as RedirectToActionResult;
        //    redirectResult.Should().NotBeNull();
        //    redirectResult.ActionName.Should().Be("ViewApplication");
        //}

        //[Test]
        //public void Contract_terminated_check_has_validation_error()
        //{
        //    var model = new OrganisationCriminalCompliancePageViewModel
        //    {
        //        ApplicationId = Guid.NewGuid(),
        //        ApplyLegalName = "legal name",
        //        ComplianceCheckQuestionId = "CC-1",
        //        ComplianceCheckAnswer = "No",
        //        OptionFailText = null,
        //        Status = "Fail",
        //        PageId = GatewayPageIds.CCOrganisationContractTermination,
        //        QuestionText = "Question text",
        //        Ukprn = "10001234"
        //    };

        //    _applyApiClient.Setup(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        //    var validationResponse = new ValidationResponse
        //    {
        //        Errors = new List<ValidationErrorDetail>()
        //        {
        //            new ValidationErrorDetail
        //            {
        //                ErrorMessage = "Comments are mandatory",
        //                Field = "OptionFailText"
        //            }
        //        }
        //    };
        //    _gatewayValidator.Setup(x => x.Validate(It.IsAny<RoatpGatewayPageViewModel>())).ReturnsAsync(validationResponse);

        //    var result = _controller.EvaluateOrganisationContractTerminationPage(model).GetAwaiter().GetResult();

        //    var viewResult = result as ViewResult;
        //    var viewModel = viewResult.Model as OrganisationCriminalCompliancePageViewModel;
        //    viewModel.Should().NotBeNull();
        //    viewModel.ErrorMessages.Count.Should().BeGreaterThan(0);
        //}

        //[Test]
        //public void Contract_withdrawn_check_returns_view()
        //{
        //    var applicationId = Guid.NewGuid();

        //    _orchestrator.Setup(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()))
        //        .ReturnsAsync(new OrganisationCriminalCompliancePageViewModel())
        //        .Verifiable("view model not returned");

        //    var result = _controller.GetOrganisationContractWithdrawnPage(applicationId).GetAwaiter().GetResult();
        //    _orchestrator.Verify(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()), Times.Once());

        //    var viewResult = result as ViewResult;
        //    viewResult.Should().NotBeNull();
        //    var viewModel = viewResult.Model as OrganisationCriminalCompliancePageViewModel;
        //    viewModel.Should().NotBeNull();
        //    viewModel.PageTitle.Should().Be(CriminalCompliancePageTitles.OrganisationContractWithdrawnEarly);
        //    viewModel.PostBackAction.Should().Be(CriminalCompliancePagePostActions.OrganisationContractWithdrawnEarly);
        //}

        //[Test]
        //public void Contract_withdrawn_check_posted()
        //{
        //    var model = new OrganisationCriminalCompliancePageViewModel
        //    {
        //        ApplicationId = Guid.NewGuid(),
        //        ApplyLegalName = "legal name",
        //        ComplianceCheckQuestionId = "CC-1",
        //        ComplianceCheckAnswer = "No",
        //        OptionPassText = null,
        //        Status = "Pass",
        //        PageId = GatewayPageIds.CCOrganisationContractWithdrawnEarly,
        //        QuestionText = "Question text",
        //        Ukprn = "10001234"
        //    };

        //    _applyApiClient.Setup(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        //    var validationResponse = new ValidationResponse
        //    {
        //        Errors = new List<ValidationErrorDetail>()
        //    };
        //    _gatewayValidator.Setup(x => x.Validate(It.IsAny<RoatpGatewayPageViewModel>())).ReturnsAsync(validationResponse);

        //    var result = _controller.EvaluateOrganisationContractWithdrawnPage(model).GetAwaiter().GetResult();

        //    var redirectResult = result as RedirectToActionResult;
        //    redirectResult.Should().NotBeNull();
        //    redirectResult.ActionName.Should().Be("ViewApplication");
        //}

        //[Test]
        //public void Contract_withdrawn_check_has_validation_error()
        //{
        //    var model = new OrganisationCriminalCompliancePageViewModel
        //    {
        //        ApplicationId = Guid.NewGuid(),
        //        ApplyLegalName = "legal name",
        //        ComplianceCheckQuestionId = "CC-1",
        //        ComplianceCheckAnswer = "No",
        //        OptionFailText = null,
        //        Status = "Fail",
        //        PageId = GatewayPageIds.CCOrganisationContractWithdrawnEarly,
        //        QuestionText = "Question text",
        //        Ukprn = "10001234"
        //    };

        //    _applyApiClient.Setup(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        //    var validationResponse = new ValidationResponse
        //    {
        //        Errors = new List<ValidationErrorDetail>()
        //        {
        //            new ValidationErrorDetail
        //            {
        //                ErrorMessage = "Comments are mandatory",
        //                Field = "OptionFailText"
        //            }
        //        }
        //    };
        //    _gatewayValidator.Setup(x => x.Validate(It.IsAny<RoatpGatewayPageViewModel>())).ReturnsAsync(validationResponse);

        //    var result = _controller.EvaluateOrganisationContractWithdrawnPage(model).GetAwaiter().GetResult();

        //    var viewResult = result as ViewResult;
        //    var viewModel = viewResult.Model as OrganisationCriminalCompliancePageViewModel;
        //    viewModel.Should().NotBeNull();
        //    viewModel.ErrorMessages.Count.Should().BeGreaterThan(0);
        //}

        //[Test]
        //public void Removed_from_Roto_check_returns_view()
        //{
        //    var applicationId = Guid.NewGuid();

        //    _orchestrator.Setup(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()))
        //        .ReturnsAsync(new OrganisationCriminalCompliancePageViewModel())
        //        .Verifiable("view model not returned");

        //    var result = _controller.GetOrganisationRemovedRotoPage(applicationId).GetAwaiter().GetResult();
        //    _orchestrator.Verify(x => x.GetCriminalComplianceCheckViewModel(It.IsAny<GetCriminalComplianceCheckRequest>()), Times.Once());

        //    var viewResult = result as ViewResult;
        //    viewResult.Should().NotBeNull();
        //    var viewModel = viewResult.Model as OrganisationCriminalCompliancePageViewModel;
        //    viewModel.Should().NotBeNull();
        //    viewModel.PageTitle.Should().Be(CriminalCompliancePageTitles.OrganisationRemovedFromRoTO);
        //    viewModel.PostBackAction.Should().Be(CriminalCompliancePagePostActions.OrganisationRemovedFromRoTO);
        //}

        //[Test]
        //public void Removed_from_Roto_check_posted()
        //{
        //    var model = new OrganisationCriminalCompliancePageViewModel
        //    {
        //        ApplicationId = Guid.NewGuid(),
        //        ApplyLegalName = "legal name",
        //        ComplianceCheckQuestionId = "CC-1",
        //        ComplianceCheckAnswer = "No",
        //        OptionPassText = null,
        //        Status = "Pass",
        //        PageId = GatewayPageIds.CCOrganisationRemovedRoTO,
        //        QuestionText = "Question text",
        //        Ukprn = "10001234"
        //    };

        //    _applyApiClient.Setup(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        //    var validationResponse = new ValidationResponse
        //    {
        //        Errors = new List<ValidationErrorDetail>()
        //    };
        //    _gatewayValidator.Setup(x => x.Validate(It.IsAny<RoatpGatewayPageViewModel>())).ReturnsAsync(validationResponse);

        //    var result = _controller.EvaluateOrganisationRemovedRotoPage(model).GetAwaiter().GetResult();

        //    var redirectResult = result as RedirectToActionResult;
        //    redirectResult.Should().NotBeNull();
        //    redirectResult.ActionName.Should().Be("ViewApplication");
        //}

        //[Test]
        //public void Removed_from_Roto_check_has_validation_error()
        //{
        //    var model = new OrganisationCriminalCompliancePageViewModel
        //    {
        //        ApplicationId = Guid.NewGuid(),
        //        ApplyLegalName = "legal name",
        //        ComplianceCheckQuestionId = "CC-1",
        //        ComplianceCheckAnswer = "No",
        //        OptionFailText = null,
        //        Status = "Fail",
        //        PageId = GatewayPageIds.CCOrganisationRemovedRoTO,
        //        QuestionText = "Question text",
        //        Ukprn = "10001234"
        //    };

        //    _applyApiClient.Setup(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        //    var validationResponse = new ValidationResponse
        //    {
        //        Errors = new List<ValidationErrorDetail>()
        //        {
        //            new ValidationErrorDetail
        //            {
        //                ErrorMessage = "Comments are mandatory",
        //                Field = "OptionFailText"
        //            }
        //        }
        //    };
        //    _gatewayValidator.Setup(x => x.Validate(It.IsAny<RoatpGatewayPageViewModel>())).ReturnsAsync(validationResponse);

        //    var result = _controller.EvaluateOrganisationRemovedRotoPage(model).GetAwaiter().GetResult();

        //    var viewResult = result as ViewResult;
        //    var viewModel = viewResult.Model as OrganisationCriminalCompliancePageViewModel;
        //    viewModel.Should().NotBeNull();
        //    viewModel.ErrorMessages.Count.Should().BeGreaterThan(0);
        //}
    }
}
