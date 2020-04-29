using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.Models;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway.OrganisationChecks
{
    [TestFixture]
    public class IcoNumberTests : RoatpGatewayControllerTestBase<RoatpGatewayOrganisationChecksController>
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
        public async Task IcoNumber_details_are_returned()
        {
            var applicationId = Guid.NewGuid();
            var expectedViewModel = new IcoNumberViewModel();

            _orchestrator.Setup(x => x.GetIcoNumberViewModel(It.Is<GetIcoNumberRequest>(y => y.ApplicationId == applicationId && y.UserName == Username))).ReturnsAsync(expectedViewModel);

            var result = await _controller.GetIcoNumberPage(applicationId);
            var viewResult = result as ViewResult;
            Assert.AreSame(expectedViewModel, viewResult.Model);
        }

        [Test]
        public async Task IcoNumber_saves_evaluation_result()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.IcoNumber;

            var vm = new IcoNumberViewModel
            {
                ApplicationId = applicationId,
                PageId = pageId,
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                OptionPassText = "Some pass text"
            };

            var command = new SubmitGatewayPageAnswerCommand(vm);

            GatewayValidator.Setup(v => v.Validate(command)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            await _controller.EvaluateIcoNumberPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, Username, vm.OptionPassText));
        }

        [Test]
        public async Task IcoNumber_without_required_fields_does_not_save()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.IcoNumber;

            var vm = new IcoNumberViewModel()
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                ApplicationId = applicationId,
                PageId = pageId
            };

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

            _orchestrator.Setup(x => x.GetIcoNumberViewModel(It.Is<GetIcoNumberRequest>(y => y.ApplicationId == vm.ApplicationId
                                                                                && y.UserName == Username))).ReturnsAsync(vm);

            await _controller.EvaluateIcoNumberPage(command);

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, Username, null), Times.Never);
        }
    }
}