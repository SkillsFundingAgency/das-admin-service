using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway.PeopleInControl
{

    [TestFixture]
    public class PeopleInControlHighRiskTests : RoatpGatewayControllerTestBase<RoatpGatewayPeopleInControlController>
    {
        private RoatpGatewayPeopleInControlController _controller;
        private Mock<IPeopleInControlOrchestrator> _orchestrator;
        private Mock<ILogger<RoatpGatewayPeopleInControlController>> _logger;

        private readonly Guid _applicationId = Guid.NewGuid();

        public GetPeopleInControlHighRiskRequest Request;
        public PeopleInControlHighRiskPageViewModel ViewModel;

        [SetUp]
        public void Setup()
        {

            CoreSetup();
            _logger = new Mock<ILogger<RoatpGatewayPeopleInControlController>>();
            _orchestrator = new Mock<IPeopleInControlOrchestrator>();
            ViewModel = new PeopleInControlHighRiskPageViewModel{ApplicationId = _applicationId};

        Request = new GetPeopleInControlHighRiskRequest(_applicationId, Username);
            _orchestrator.Setup(x =>
                    x.GetPeopleInControlHighRiskViewModel(It.IsAny<GetPeopleInControlHighRiskRequest>()))
                .ReturnsAsync(ViewModel)
                .Verifiable("view model not returned");

            _controller = new RoatpGatewayPeopleInControlController(ContextAccessor.Object, ApplyApiClient.Object, _logger.Object, GatewayValidator.Object, _orchestrator.Object);
        }

        [Test]
        public void check_people_in_control_high_risk_request_is_sent_and_viewmodel_returned()
        {
            var result = (ViewResult)_controller.GetGatewayPeopleInControlRiskPage(_applicationId, GatewayPageIds.PeopleInControl).Result;
            var resultModel = (PeopleInControlHighRiskPageViewModel)result.Model;
            Assert.AreEqual(_applicationId, resultModel.ApplicationId);
        }

        [Test]
        public void post_people_in_control_high_risk_happy_path()
        {
            var command = new SubmitGatewayPageAnswerCommand
            {
                Status = SectionReviewStatus.Pass,
                ApplicationId = ViewModel.ApplicationId,
                PageId = ViewModel.PageId
            };

            var result = (RedirectToActionResult)_controller.EvaluatePeopleInControlHighRiskPage(command).Result;

            GatewayValidator.Verify(x => x.Validate(command), Times.Once);
            Assert.AreEqual("ViewApplication", result.ActionName);
            Assert.AreEqual("RoatpGateway", result.ControllerName);
        }

        [Test]
        public void post_people_in_control_high_risk_path_with_errors()
        {
            var vm = ViewModel;

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

            vm.PageId = GatewayPageIds.PeopleInControlRisk;
            vm.SourcesCheckedOn = DateTime.Now;

            _orchestrator.Setup(x => x.GetPeopleInControlHighRiskViewModel(Request))
                .ReturnsAsync(vm)
                .Verifiable("view model not returned");

            var result = (ViewResult)_controller.EvaluatePeopleInControlHighRiskPage(command).Result;

            var resultModel = (PeopleInControlHighRiskPageViewModel)result.Model;

            GatewayValidator.Verify(x => x.Validate(command), Times.Once);
            Assert.AreEqual(1, resultModel.ErrorMessages.Count);
        }
    }
}
