using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway.PeopleInControl
{

    [TestFixture]
    public class PeopleInControlHighRiskTests
    {
        private RoatpGatewayPeopleInControlController _controller;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IHttpContextAccessor> _contextAccessor;
        private Mock<IRoatpGatewayPageViewModelValidator> _gatewayValidator;
        private Mock<IPeopleInControlOrchestrator> _orchestrator;
        private Mock<ILogger<RoatpGatewayPeopleInControlController>> _logger;

        private string username => "mark cain";
        private string givenName => "mark";
        private string surname => "cain";
        private Guid applicationId = Guid.NewGuid();
        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _contextAccessor = new Mock<IHttpContextAccessor>();
            _gatewayValidator = new Mock<IRoatpGatewayPageViewModelValidator>();
            _logger = new Mock<ILogger<RoatpGatewayPeopleInControlController>>();
            _orchestrator = new Mock<IPeopleInControlOrchestrator>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
             {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", username),
                new Claim(ClaimTypes.GivenName, givenName),
                new Claim(ClaimTypes.Surname, surname)
             }));

            var context = new DefaultHttpContext { User = user };
            _gatewayValidator.Setup(v => v.Validate(It.IsAny<PeopleInControlHighRiskPageViewModel>()))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>()
                }
                );
            _contextAccessor.Setup(_ => _.HttpContext).Returns(context);

            _orchestrator.Setup(x =>
                    x.GetPeopleInControlHighRiskViewModel(new GetPeopleInControlHighRiskRequest(applicationId, username)))
                .ReturnsAsync(new PeopleInControlHighRiskPageViewModel())
                .Verifiable("view model not returned");


            _controller = new RoatpGatewayPeopleInControlController(_contextAccessor.Object, _applyApiClient.Object, _logger.Object, _gatewayValidator.Object, _orchestrator.Object);
        }

        [Test]
        public void check_people_in_control_high_risk_request_is_sent()
        {
            var _result = _controller.GetGatewayPeopleInControlRiskPage(applicationId, GatewayPageIds.PeopleInControl).Result;
            _orchestrator.Verify(x => x.GetPeopleInControlHighRiskViewModel(It.IsAny<GetPeopleInControlHighRiskRequest>()), Times.Once());
        }

        [Test]
        public void post_people_in_control_high_risk_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.PeopleInControlRisk;

            var vm = new PeopleInControlHighRiskPageViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            vm.SourcesCheckedOn = DateTime.Now;

            _applyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, username, It.IsAny<string>()));

            var result = _controller.EvaluatePeopleInControlHighRiskPage(vm).Result;

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _orchestrator.Verify(x => x.GetPeopleInControlHighRiskViewModel(It.IsAny<GetPeopleInControlHighRiskRequest>()), Times.Once());
        }

        [Test]
        public void post_people_in_control_high_risk_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.PeopleInControlRisk;

            var viewModel = new PeopleInControlHighRiskPageViewModel
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()

            };

            _gatewayValidator.Setup(v => v.Validate(It.IsAny<PeopleInControlHighRiskPageViewModel>()))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                        new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                }
                );

            viewModel.ApplicationId = applicationId;
            viewModel.PageId = viewModel.PageId;
            viewModel.SourcesCheckedOn = DateTime.Now;

            _orchestrator.Setup(x => x.GetPeopleInControlHighRiskViewModel(It.IsAny<GetPeopleInControlHighRiskRequest>()))
                .ReturnsAsync(viewModel)
                .Verifiable("view model not returned");


            _applyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, viewModel.Status, username, It.IsAny<string>()));

            var result = _controller.EvaluatePeopleInControlHighRiskPage(viewModel).Result;

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _orchestrator.Verify(x => x.GetPeopleInControlHighRiskViewModel(It.IsAny<GetPeopleInControlHighRiskRequest>()), Times.Once());

        }
    }

}
