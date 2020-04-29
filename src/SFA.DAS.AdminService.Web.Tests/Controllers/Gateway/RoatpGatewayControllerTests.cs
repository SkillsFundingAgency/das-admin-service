using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway
{
    [TestFixture]
    public class RoatpGatewayControllerBaseTests
    {
        private RoatpGatewayControllerBase<RoatpGatewayControllerBaseTests> _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new RoatpGatewayControllerBase<RoatpGatewayControllerBaseTests>(Mock.Of<IHttpContextAccessor>(), Mock.Of<IRoatpApplicationApiClient>(), Mock.Of<ILogger<RoatpGatewayControllerBaseTests>>(), Mock.Of<IRoatpGatewayPageValidator>());
        }

        [TestCase(SectionReviewStatus.Pass, "pass", "fail","in progress","pass","","")]
        [TestCase(SectionReviewStatus.Fail, "pass", "fail", "in progress", "", "fail", "")]
        [TestCase(SectionReviewStatus.InProgress, "pass", "fail", "in progress", "", "", "in progress")]
        [TestCase(null, "pass", "fail", "in progress", "pass", "fail", "in progress")]
        public void check_gateway_options_settings(string status, string optionPassText,string optionFailText,string optionInProgressText, string expectedOptionPassText, string expectedOptionFailText, string expectedOptionInProgressText)
        {
            var command = new SubmitGatewayPageAnswerCommand 
            {
                Status = status,
                OptionPassText = optionPassText,
                OptionFailText = optionFailText,
                OptionInProgressText = optionInProgressText
            };

            _controller.SetupGatewayPageOptionTexts(command);
            Assert.AreEqual(expectedOptionPassText, command.OptionPassText);
            Assert.AreEqual(expectedOptionFailText, command.OptionFailText);
            Assert.AreEqual(expectedOptionInProgressText, command.OptionInProgressText);
        }
    }
}
