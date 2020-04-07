using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;

namespace SFA.DAS.AdminService.Web.Tests.Validators
{
    [TestFixture]
    public class GatewayValidatorTests
    {
        private RoatpGatewayPageViewModel _viewModel;

        private IRoatpGatewayPageViewModelValidator _validator;
        [SetUp]
        public void Setup()
        {
            _validator = new RoatpGatewayPageViewModelValidator();
        }

        [TestCase(SectionReviewStatus.Pass,"","","",false)]
        [TestCase(SectionReviewStatus.Pass, "sewrrwe", "", "", false)]
        [TestCase(SectionReviewStatus.InProgress, "", "", "", false)]
        [TestCase(SectionReviewStatus.InProgress, "", "asdasssfdsf", "", false)]
        [TestCase(SectionReviewStatus.Fail, "", "", "fail message goes here", false)]
        [TestCase(SectionReviewStatus.Fail, "", "", "", true )]
        [TestCase(null, "", "", "", true)]
        public void test_cases_for_no_status_and_no_fail_text_to_check_messages_as_expected(string status, string passMessage, string inProgressMessage, string failMessage, bool hasErrorMessage)
        {
            _viewModel = new RoatpGatewayPageViewModel
            {
                Status = status,
                OptionPassText = passMessage,
                OptionFailText = failMessage,
                OptionInProgressText = inProgressMessage
            };

            var result = _validator.Validate(_viewModel).Result;

            Assert.AreEqual(hasErrorMessage,result.Errors.Any());
        }

        [TestCase(150, false)]
        [TestCase(151,true)]
        public void test_cases_where_input_is_too_long(int wordCount, bool hasErrorMessage)
        {
            var words = string.Empty;
            for (var i=0;  i < wordCount; i++)
            {
                words = $"{words}{i} ";
            }

            _viewModel = new RoatpGatewayPageViewModel();
            _viewModel.Status = SectionReviewStatus.Pass;
            _viewModel.OptionPassText = words;


            var result = _validator.Validate(_viewModel).Result;

            Assert.AreEqual(hasErrorMessage, result.Errors.Any());

        }
    }
}
