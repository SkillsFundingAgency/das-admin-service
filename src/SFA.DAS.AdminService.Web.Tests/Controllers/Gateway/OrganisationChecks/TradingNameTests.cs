using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway.OrganisationChecks
{
    [TestFixture]
    public class TradingNameTests : RoatpGatewayControllerTestBase<RoatpGatewayOrganisationChecksController>
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
        public void check_trading_name_request_is_called()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-10";

            _orchestrator.Setup(x => x.GetTradingNameViewModel(new GetTradingNameRequest(applicationId, Username)))
                .ReturnsAsync(new TradingNamePageViewModel())
                .Verifiable("view model not returned");

            var _result = _controller.GetGatewayTradingNamePage(applicationId, pageId).Result;
            _orchestrator.Verify(x => x.GetTradingNameViewModel(It.IsAny<GetTradingNameRequest>()), Times.Once());
        }

        [Test]
        public void post_trading_name_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-20";

            var vm = new TradingNamePageViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            vm.SourcesCheckedOn = DateTime.Now;
            var command = new SubmitGatewayPageAnswerCommand(vm);

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, Username, It.IsAny<string>()));

            var result = _controller.EvaluateTradingNamePage(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _orchestrator.Verify(x => x.GetTradingNameViewModel(It.IsAny<GetTradingNameRequest>()), Times.Never());
        }

        [Test]
        public void post_trading_name_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-20";

            var vm = new TradingNamePageViewModel
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()

            };

            vm.ApplicationId = applicationId;
            vm.PageId = vm.PageId;
            vm.SourcesCheckedOn = DateTime.Now;
            var command = new SubmitGatewayPageAnswerCommand(vm);

            GatewayValidator.Setup(v => v.Validate(command))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                }
                );

            _orchestrator.Setup(x => x.GetTradingNameViewModel(It.Is<GetTradingNameRequest>(y => y.ApplicationId == vm.ApplicationId
                                                                                && y.UserName == Username))).ReturnsAsync(vm);

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, Username, It.IsAny<string>()));

            var result = _controller.EvaluateTradingNamePage(command).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        }
    }
}