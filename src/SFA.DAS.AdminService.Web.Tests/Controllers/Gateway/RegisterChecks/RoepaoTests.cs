using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway.RegisterChecks
{
    [TestFixture]
    public class RoepaoTests
    {
        private RoatpGatewayRegisterChecksController _controller;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IHttpContextAccessor> _contextAccessor;
        private Mock<IRoatpGatewayPageValidator> _gatewayValidator;
        private Mock<IGatewayRegisterChecksOrchestrator> _orchestrator;
        private Mock<ILogger<RoatpGatewayRegisterChecksController>> _logger;

        private string username = "john smith";
        private string givenName = "john";
        private string surname = "smith";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _contextAccessor = new Mock<IHttpContextAccessor>();
            _gatewayValidator = new Mock<IRoatpGatewayPageValidator>();
            _logger = new Mock<ILogger<RoatpGatewayRegisterChecksController>>();
            _orchestrator = new Mock<IGatewayRegisterChecksOrchestrator>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
             {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", username),
                new Claim(ClaimTypes.GivenName, givenName),
                new Claim(ClaimTypes.Surname, surname)
             }));

            _contextAccessor.Setup(_ => _.HttpContext).Returns(new DefaultHttpContext { User = user });

            _gatewayValidator.Setup(v => v.Validate(It.IsAny<SubmitGatewayPageAnswerCommand>()))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>()
                });

            _controller = new RoatpGatewayRegisterChecksController(_applyApiClient.Object, _contextAccessor.Object, _gatewayValidator.Object, _orchestrator.Object, _logger.Object);
        }

        [Test]
        public async Task check_roepao_request_is_sent()
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetRoepaoViewModel(new GetRoepaoRequest(applicationId, username)))
                .ReturnsAsync(new RoepaoPageViewModel())
                .Verifiable("view model not returned");

            var _result = await _controller.GetGatewayRoepaoPage(applicationId, GatewayPageIds.Roepao);
            _orchestrator.Verify(x => x.GetRoepaoViewModel(It.Is<GetRoepaoRequest>(y => y.ApplicationId == applicationId && y.UserName == username)), Times.Once());
        }

        [Test]
        public async Task saving_roepao_happy_path_saves_evaluation_result()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.Roepao;

            var vm = new RoepaoPageViewModel
            {
                ApplicationId = applicationId,
                PageId = pageId,
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                OptionPassText = "Some pass text"
            };

            var command = new SubmitGatewayPageAnswerCommand(vm);

            _gatewayValidator.Setup(v => v.Validate(command)).ReturnsAsync(new ValidationResponse { Errors = new List<ValidationErrorDetail>() });

            var result = await _controller.EvaluateRoepaoPage(command);

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, username, vm.OptionPassText), Times.Once);
        }

        [Test]
        public async Task saving_roepao_without_required_fields_does_not_save()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.Roepao;

            var vm = new RoepaoPageViewModel
            {
                ApplicationId = applicationId,
                PageId = pageId,
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>(),
                OptionFailText = null
            };

            var command = new SubmitGatewayPageAnswerCommand(vm);

            _gatewayValidator.Setup(v => v.Validate(command))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                });

            _orchestrator.Setup(x => x.GetRoepaoViewModel(It.Is<GetRoepaoRequest>(y => y.ApplicationId == vm.ApplicationId
                                                                                && y.UserName == username))).ReturnsAsync(vm);

            var result = await _controller.EvaluateRoepaoPage(command);

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, username, vm.OptionPassText), Times.Never);
        }
    }
}
