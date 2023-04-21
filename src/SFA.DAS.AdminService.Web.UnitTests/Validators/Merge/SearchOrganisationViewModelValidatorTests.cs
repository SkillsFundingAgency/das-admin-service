using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;

namespace SFA.DAS.AdminService.Web.Tests.Validators.Merge
{
    public class SearchOrganisationViewModelValidatorTests
    {
        private SearchOrganisationViewModelValidator Validator;

        [SetUp]
        public void Arrange()
        {
            Validator = new SearchOrganisationViewModelValidator();
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("a", false)]
        [TestCase("ab", true)]
        public void ValidateSearchString(string searchString, bool expectedResult)
        {
            var viewModel = new SearchOrganisationViewModel
            {
                SearchString = searchString
            };

            var result = Validator.Validate(viewModel);

            result.IsValid.Should().Be(expectedResult);
        }
    }
}
