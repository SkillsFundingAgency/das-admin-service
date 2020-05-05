using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway.PeopleInControl
{

        [TestFixture]
        public class PeopleInControlTests : RoatpGatewayControllerTestBase<RoatpGatewayPeopleInControlController>
        {
            private RoatpGatewayPeopleInControlController _controller;
            private Mock<IPeopleInControlOrchestrator> _orchestrator;
            private Mock<ILogger<RoatpGatewayPeopleInControlController>> _logger;
            private readonly Guid _applicationId = Guid.NewGuid();

            public GetPeopleInControlRequest Request;
            public PeopleInControlPageViewModel ViewModel;

            [SetUp]
            public void Setup()
            {
                CoreSetup();
                _logger = new Mock<ILogger<RoatpGatewayPeopleInControlController>>();
                _orchestrator = new Mock<IPeopleInControlOrchestrator>();
                ViewModel = new PeopleInControlPageViewModel { ApplicationId = _applicationId };

                Request = new GetPeopleInControlRequest(_applicationId, Username);
                _orchestrator.Setup(x =>
                        x.GetPeopleInControlViewModel(It.IsAny<GetPeopleInControlRequest>()))
                    .ReturnsAsync(ViewModel)
                    .Verifiable("view model not returned");

                _controller = new RoatpGatewayPeopleInControlController(ContextAccessor.Object, ApplyApiClient.Object, _logger.Object, GatewayValidator.Object,  _orchestrator.Object);
            }

            [Test]
            public void check_people_in_control_request_is_sent_and_viewmodel_returned()
            {
                var result = (ViewResult)_controller.GetGatewayPeopleInControlPage(_applicationId, GatewayPageIds.PeopleInControl).Result;
                var resultModel = (PeopleInControlPageViewModel)result.Model;
                Assert.AreEqual(_applicationId, resultModel.ApplicationId);
            }

            [Test]
            public void post_people_in_control_happy_path()
            {
                var command = new SubmitGatewayPageAnswerCommand
                {
                    Status = SectionReviewStatus.Pass,
                    ApplicationId = ViewModel.ApplicationId,
                    PageId = ViewModel.PageId
                };

                var result = (RedirectToActionResult)_controller.EvaluatePeopleInControlPage(command).Result;

                    GatewayValidator.Verify(x => x.Validate(command), Times.Once);
                    Assert.AreEqual("ViewApplication", result.ActionName);
                    Assert.AreEqual("RoatpGateway", result.ControllerName);
            }

            [Test]
            public void post_people_in_control_path_with_errors()
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

                _orchestrator.Setup(x => x.GetPeopleInControlViewModel(Request))
                    .ReturnsAsync(vm)
                    .Verifiable("view model not returned");

                var result = (ViewResult)_controller.EvaluatePeopleInControlPage(command).Result;
                var resultModel = (PeopleInControlPageViewModel)result.Model;

                GatewayValidator.Verify(x => x.Validate(command), Times.Once);
                Assert.AreEqual(1, resultModel.ErrorMessages.Count);
        }
    }
    
}
