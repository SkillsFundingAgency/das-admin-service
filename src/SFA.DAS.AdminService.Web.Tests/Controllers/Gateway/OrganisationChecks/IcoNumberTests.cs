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
        public void check_ico_number_request_is_sent()
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetIcoNumberViewModel(new GetIcoNumberRequest(applicationId, Username)))
                .ReturnsAsync(new IcoNumberViewModel())
                .Verifiable("view model not returned");

            var _result = _controller.GetIcoNumberPage(applicationId).Result;
            _orchestrator.Verify(x => x.GetIcoNumberViewModel(It.IsAny<GetIcoNumberRequest>()), Times.Once());
        }

        [Test]
        public void post_ico_number_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.IcoNumber;

            var vm = new IcoNumberViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            ApplyApiClient.Setup(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, Username, It.IsAny<string>()));

            var result = _controller.EvaluateIcoNumberPage(vm).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _orchestrator.Verify(x => x.GetIcoNumberViewModel(It.IsAny<GetIcoNumberRequest>()), Times.Never());
        }

        [Test]
        public void post_ico_number_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.IcoNumber;

            var vm = new IcoNumberViewModel
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()

            };

            GatewayValidator.Setup(v => v.Validate(It.IsAny<IcoNumberViewModel>()))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                }
                );

            vm.ApplicationId = applicationId;
            vm.PageId = vm.PageId;
            vm.SourcesCheckedOn = DateTime.Now;

            _orchestrator.Setup(x => x.GetIcoNumberViewModel(It.IsAny<GetIcoNumberRequest>()))
                .ReturnsAsync(vm)
                .Verifiable("view model not returned");

            ApplyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, Username, It.IsAny<string>()));

            var result = _controller.EvaluateIcoNumberPage(vm).Result;

            ApplyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _orchestrator.Verify(x => x.GetIcoNumberViewModel(It.IsAny<GetIcoNumberRequest>()), Times.Never());
        }

    }
}
