using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway
{
    [TestFixture]
    public class RoatpGatewayControllerTests
    {

        private RoatpGatewayController _controller;
        private  Mock<IRoatpApplicationApiClient> _applyApiClient;
        private  Mock<IHttpContextAccessor> _contextAccessor;
        private  Mock<IRoatpGatewayPageViewModelValidator> _gatewayValidator;
        private  Mock<IMediator> _mediator;
        private Mock<ILogger<RoatpGatewayController>> _logger;


        [SetUp]
        public void Setup()
        {
           _applyApiClient = new Mock<IRoatpApplicationApiClient>();
           _contextAccessor = new Mock<IHttpContextAccessor>();
           _gatewayValidator = new Mock<IRoatpGatewayPageViewModelValidator>();
           _mediator = new Mock<IMediator>();
           _logger = new Mock<ILogger<RoatpGatewayController>>();


            _controller = new RoatpGatewayController(_applyApiClient.Object,_contextAccessor.Object,_gatewayValidator.Object,_mediator.Object, _logger.Object);
        }


        [TestCase(SectionReviewStatus.Pass, "pass"  ,"fail","in progress","pass","","")]
        [TestCase(SectionReviewStatus.Fail, "pass", "fail", "in progress", "", "fail", "")]
        [TestCase(SectionReviewStatus.InProgress, "pass", "fail", "in progress", "", "", "in progress")]
        [TestCase(null, "pass", "fail", "in progress", "pass", "fail", "in progress")]
        public void check_gateway_options_settings(string status, string optionPassText,string optionFailText,string optionInProgressText, string expectedOptionPassText, string expectedOptionFailText, string expectedOptionInProgressText)
        {
            var vm = new RoatpGatewayPageViewModel
            {
                Status = status,
                OptionPassText = optionPassText,
                OptionFailText = optionFailText,
                OptionInProgressText = optionInProgressText
            };

            _controller.SetupGatewayPageOptionTexts(vm);
            Assert.AreEqual(expectedOptionPassText,vm.OptionPassText);
            Assert.AreEqual(expectedOptionFailText, vm.OptionFailText);
            Assert.AreEqual(expectedOptionInProgressText, vm.OptionInProgressText);
        }
    }
}
