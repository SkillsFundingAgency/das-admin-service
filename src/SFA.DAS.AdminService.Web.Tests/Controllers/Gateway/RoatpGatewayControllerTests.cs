using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway
{
    [TestFixture]
    public class RoatpGatewayControllerBaseTests
    {
        private RoatpGatewayControllerBase _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new RoatpGatewayControllerBase();
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
