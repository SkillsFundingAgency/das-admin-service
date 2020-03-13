using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Handlers.Gateway;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway
{
    [TestFixture]
    public class RoatpGatewayControllerLegalNameTests
    {

        private RoatpGatewayController _controller;
        private  Mock<IRoatpApplicationApiClient> _applyApiClient;
        private  Mock<IHttpContextAccessor> _contextAccessor;
        private  Mock<IRoatpGatewayPageViewModelValidator> _gatewayValidator;
        private  Mock<IMediator> _mediator;
        private Mock<ILogger<RoatpGatewayController>> _logger;

        private string username => "mark cain";
        private string givenName => "mark";
        private string surname => "cain";
        [SetUp]
        public void Setup()
        {
           _applyApiClient = new Mock<IRoatpApplicationApiClient>();
           _contextAccessor = new Mock<IHttpContextAccessor>();
           _gatewayValidator = new Mock<IRoatpGatewayPageViewModelValidator>();
           _mediator = new Mock<IMediator>();
           _logger = new Mock<ILogger<RoatpGatewayController>>();
            //_contextAccessor.Setup(x=>x.HttpContext.)

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", username),
                new Claim(ClaimTypes.GivenName, givenName),
                new Claim(ClaimTypes.Surname, surname)
            }));

            var context = new DefaultHttpContext{ User = user };
            _gatewayValidator.Setup(v => v.Validate(It.IsAny<LegalNamePageViewModel>()))
                .ReturnsAsync(new ValidationResponse
                    {
                        Errors = new List<ValidationErrorDetail>()
                    }
                );
            _contextAccessor.Setup(_ => _.HttpContext).Returns(context);
            _controller = new RoatpGatewayController(_applyApiClient.Object,_contextAccessor.Object,_gatewayValidator.Object,_mediator.Object,_logger.Object);
        }

        [Test]
        public  void check_legal_name_request_is_sent()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-10";

            _mediator.Setup(x => x.Send(new GetLegalNameRequest(applicationId, username), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LegalNamePageViewModel())
                .Verifiable("view model not returned");

            var _result =  _controller.GetGatewayLegalNamePage(applicationId, pageId).Result;
            _mediator.Verify(x => x.Send(It.IsAny<GetLegalNameRequest>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public void post_legal_name_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-10";

            var vm = new LegalNamePageViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            vm.SourcesCheckedOn = DateTime.Now;

            var pageData = JsonConvert.SerializeObject(vm);

            _applyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, username, It.IsAny<string>()));

            var result = _controller.EvaluateLegalNamePage(vm).Result;

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mediator.Verify(x => x.Send(It.IsAny<GetLegalNameRequest>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public void post_legal_name_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-20";

            var vm = new LegalNamePageViewModel
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()

            };

            _gatewayValidator.Setup(v => v.Validate(It.IsAny<LegalNamePageViewModel>()))
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

            _mediator.Setup(x => x.Send(It.IsAny<GetLegalNameRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(vm)
                .Verifiable("view model not returned");

            var pageData = JsonConvert.SerializeObject(vm);

            _applyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, username, It.IsAny<string>()));

            var result = _controller.EvaluateLegalNamePage(vm).Result;

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mediator.Verify(x => x.Send(It.IsAny<GetLegalNameRequest>(), It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}
