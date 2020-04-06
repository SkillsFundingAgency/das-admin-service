using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;

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
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Whistleblowing)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt)]
        public void Criminal_compliance_check_is_displayed(string gatewayPageId)
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
        }

        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Whistleblowing)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt)]
        public void Criminal_compliance_check_result_is_saved(string gatewayPageId)
        {
            var model = new OrganisationCriminalCompliancePageViewModel
            {
                ApplicationId = Guid.NewGuid(),
                ApplyLegalName = "legal name",
                ComplianceCheckQuestionId = "CC-1",
                ComplianceCheckAnswer = "No",
                OptionPassText = "check passed",
                Status = "Pass",
                PageId = gatewayPageId,
                QuestionText = "Question text",
                Ukprn = "10001234",
            };

            var validationResponse = new ValidationResponse 
            {
                Errors = new List<ValidationErrorDetail>()
            };
            _gatewayValidator.Setup(x => x.Validate(It.IsAny<RoatpGatewayPageViewModel>())).ReturnsAsync(validationResponse);

            var result = _controller.EvaluateCriminalCompliancePage(model).GetAwaiter().GetResult();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("ViewApplication");

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(model.ApplicationId, gatewayPageId, model.Status,
                                  username, model.OptionPassText), Times.Once);
        }

        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Whistleblowing)]
        [TestCase(GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees)]
        [TestCase(GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt)]
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

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(model.ApplicationId, gatewayPageId, model.Status,
                                 username, model.OptionPassText), Times.Never);
        }
    }
}
