using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway
{
    [TestFixture]
    public class RoatpGatewayControllerTests : RoatpGatewayControllerTestBase<RoatpGatewayController>
    {
        private RoatpGatewayController _controller;
        private Mock<IGatewayOverviewOrchestrator> _orchestrator;
        private Mock<IRoatpGatewayApplicationViewModelValidator> _validator;

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayOverviewOrchestrator>();
            _validator = new Mock<IRoatpGatewayApplicationViewModelValidator>();
            _controller = new RoatpGatewayController(ApplyApiClient.Object, ContextAccessor.Object, _orchestrator.Object, _validator.Object,  Logger.Object);
        }

        [Test]
        public async Task ConfirmOutcome_model_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new RoatpGatewayApplicationViewModel { ReadyToConfirm  = true };

            ApplyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new RoatpApplicationResponse { ApplicationId = applicationId });
            _orchestrator.Setup(x => x.GetConfirmOverviewViewModel(It.Is<GetApplicationOverviewRequest>(y => y.ApplicationId == applicationId && y.UserName == Username))).ReturnsAsync(expectedViewModel);

            var result = await _controller.ConfirmOutcome(applicationId);
            var viewResult = result as ViewResult;
            Assert.AreSame(expectedViewModel, viewResult.Model);
        }

        [Test]
        public async Task ConfirmOutcome_evaluation_positive_result()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpGatewayApplicationViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.Approved,
                OptionApprovedText = "Some approved text",
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            _validator.Setup(v => v.Validate(viewModel)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            await _controller.EvaluateConfirmOutcome(viewModel);

            _orchestrator.Verify(x => x.GetConfirmOverviewViewModel(new GetApplicationOverviewRequest(viewModel.ApplicationId, Username)), Times.Never);
        }

        [Test]
        public async Task ConfirmOutcome_evaluation_result_is_on_error()
        {
            var applicationId = Guid.NewGuid();

            var viewModel = new RoatpGatewayApplicationViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.InProgress,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            _validator.Setup(v => v.Validate(viewModel))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "GatewayReviewStatus", ErrorMessage = "Select what you want to do"}
                        }
                });

            var expectedViewModelWithErrors = new RoatpGatewayApplicationViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = GatewayReviewStatus.New,
                ErrorMessages = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "GatewayReviewStatus", ErrorMessage = "Select what you want to do"}
                        }
            };

            _orchestrator.Setup(x => x.GetConfirmOverviewViewModel(It.Is<GetApplicationOverviewRequest>(y => y.ApplicationId == applicationId && y.UserName == Username))).ReturnsAsync(expectedViewModelWithErrors);

            var result = await _controller.EvaluateConfirmOutcome(viewModel);
            var viewResult = result as ViewResult;
            Assert.AreSame(expectedViewModelWithErrors, viewResult.Model);
        }
    }
}
