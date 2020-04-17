using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Validators
{
    [TestFixture]
    public class RoatpGatewayApplicationViewModelValidatorTests
    {
        private RoatpGatewayApplicationViewModel _viewModel;

        private IRoatpGatewayApplicationViewModelValidator _validator;
        [SetUp]
        public void Setup()
        {
            _validator = new RoatpGatewayApplicationViewModelValidator();
        }

        [TestCase(GatewayReviewStatus.AskForClarification, "Clarification Message", "Declined Message", "Approved Message", false)]
        [TestCase(GatewayReviewStatus.AskForClarification, null, "Declined Message", "Approved Message", true)]
        [TestCase(GatewayReviewStatus.Decline, "Clarification Message", "Declined Message", "Approved Message", false)]
        [TestCase(GatewayReviewStatus.Decline, "Clarification Message", null, "Approved Message", true)]
        [TestCase(GatewayReviewStatus.Pass, "Clarification Message", "Declined Message", "Approved Message", false)]
        [TestCase(GatewayReviewStatus.Pass, null, "Declined Message", "Approved Message", false)]
        [TestCase(null, null, null, null, true)]
        public void Test_cases_for_no_status_and_no_fail_text_to_check_messages_as_expected(string gatewayReviewStatus, string clarificationMessage, string declinedMessage, string approvedMessage, bool hasErrorMessage)
        {
            _viewModel = new RoatpGatewayApplicationViewModel
            {
                GatewayReviewStatus = gatewayReviewStatus,
                OptionAskClarificationText = clarificationMessage,
                OptionDeclinedText = declinedMessage,
                OptionApprovedText = approvedMessage
            };

            var result = _validator.Validate(_viewModel).Result;

            Assert.AreEqual(hasErrorMessage, result.Errors.Any());
        }

        [TestCase(GatewayReviewStatus.AskForClarification, 500, false)]
        [TestCase(GatewayReviewStatus.AskForClarification, 501, true)]
        [TestCase(GatewayReviewStatus.Decline, 500, false)]
        [TestCase(GatewayReviewStatus.Decline, 501, true)]
        [TestCase(GatewayReviewStatus.Pass, 500, false)]
        [TestCase(GatewayReviewStatus.Pass, 501, true)]
        public void Test_cases_where_input_is_too_long(string gatewayReviewStatus, int wordCount, bool hasErrorMessage)
        {
            var words = string.Empty;
            for (var i = 0; i < wordCount; i++)
            {
                words = $"{words}{i} ";
            }

            _viewModel = new RoatpGatewayApplicationViewModel();
            _viewModel.GatewayReviewStatus = gatewayReviewStatus;
            _viewModel.OptionAskClarificationText = words;
            _viewModel.OptionDeclinedText = words;
            _viewModel.OptionApprovedText = words;

            var result = _validator.Validate(_viewModel).Result;

            Assert.AreEqual(hasErrorMessage, result.Errors.Any());
        }
    }
}
