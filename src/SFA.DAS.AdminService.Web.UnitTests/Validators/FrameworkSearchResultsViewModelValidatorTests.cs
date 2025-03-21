using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using System;

namespace SFA.DAS.AdminService.Web.UnitTests.Validators
{
    [TestFixture]
    public class FrameworkLearnerSearchResultsViewModelValidatorTests
    {
        private FrameworkLearnerSearchResultsViewModelValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new FrameworkLearnerSearchResultsViewModelValidator();
        }

        [Test]
        public void FrameworkSearchResults_NoneSelected_HasError()
        {
            var vm = new FrameworkLearnerSearchResultsViewModel();
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.SelectedResult)
                .WithErrorMessage("Select a course");
        }

        [Test]
        public void FrameworkSearchResults_CourseSelected_NoError()
        {
            var vm = new FrameworkLearnerSearchResultsViewModel { SelectedResult = Guid.NewGuid()};
            var result = _validator.TestValidate(vm);
            result.ShouldNotHaveValidationErrorFor(x => x.SelectedResult);
        }
    }
}