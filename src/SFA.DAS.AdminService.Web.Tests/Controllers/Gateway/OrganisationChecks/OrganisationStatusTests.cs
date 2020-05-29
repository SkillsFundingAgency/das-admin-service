using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway.OrganisationChecks
{
    [TestFixture]
    public class OrganisationStatusTests : RoatpGatewayControllerTestBase<RoatpGatewayOrganisationChecksController>
    {
        private RoatpGatewayOrganisationChecksController _controller;
        private Mock<IGatewayOrganisationChecksOrchestrator> _orchestrator;

        [SetUp]
        public void Setup()
        {
            CoreSetup();

            _orchestrator = new Mock<IGatewayOrganisationChecksOrchestrator>();
            _controller = new RoatpGatewayOrganisationChecksController(ApplyApiClient.Object, ContextAccessor.Object, GatewayValidator.Object, _orchestrator.Object, Logger.Object);
        }

        [Test]
        public void check_organisation_status_request_is_called()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-10";

            _orchestrator.Setup(x => x.GetOrganisationStatusViewModel(new GetOrganisationStatusRequest(applicationId, Username)))
                .ReturnsAsync(new OrganisationStatusViewModel())
                .Verifiable("view model not returned");

            var _result = _controller.GetOrganisationStatusPage(applicationId, pageId).Result;
            _orchestrator.Verify(x => x.GetOrganisationStatusViewModel(It.IsAny<GetOrganisationStatusRequest>()), Times.Once());
        }

        [Test]
        public void post_organisation_status_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-30";

            var viewModel = new OrganisationStatusViewModel()
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            viewModel.SourcesCheckedOn = DateTime.Now;
            var command = new SubmitGatewayPageAnswerCommand(viewModel);

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, viewModel.Status, Username, It.IsAny<string>()));

            var result = _controller.EvaluateOrganisationStatus(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _orchestrator.Verify(x => x.GetOrganisationStatusViewModel(It.IsAny<GetOrganisationStatusRequest>()), Times.Never());
        }

        [Test]
        public void post_organisation_status_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-20";

            var viewModel = new OrganisationStatusViewModel()
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()

            };

            viewModel.ApplicationId = applicationId;
            viewModel.PageId = viewModel.PageId;
            viewModel.SourcesCheckedOn = DateTime.Now;
            var command = new SubmitGatewayPageAnswerCommand(viewModel);

            GatewayValidator.Setup(v => v.Validate(command))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                }
                );

            _orchestrator.Setup(x => x.GetOrganisationStatusViewModel(It.IsAny<GetOrganisationStatusRequest>()))
                .ReturnsAsync(viewModel)
                .Verifiable("view model not returned");

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, viewModel.Status, Username, It.IsAny<string>()));

            var result = _controller.EvaluateOrganisationStatus(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _orchestrator.Verify(x => x.GetTradingNameViewModel(It.IsAny<GetTradingNameRequest>()), Times.Never());
        }
    }
}