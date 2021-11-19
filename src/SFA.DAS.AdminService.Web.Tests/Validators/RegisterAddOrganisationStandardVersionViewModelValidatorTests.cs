using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Tests.Validators
{
    public class RegisterAddOrganisationStandardVersionViewModelValidatorTests
    {
        private RegisterAddOrganisationStandardVersionViewModelValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new RegisterAddOrganisationStandardVersionViewModelValidator(new RegisterValidator());
        }

        [Test]
        public void When_DateIsNull_ValidatorReturnsError()
        {
            var viewModel = SetupViewModel(null, null, null);

            var result = _validator.Validate(viewModel);

            result.Errors.Single(e => e.PropertyName == "EffectiveFromDate").ErrorMessage.Should().Be("The effective from date is required");
        }

        [TestCase("0", "0", "0")]
        [TestCase("30", "2", "2020")]
        [TestCase("1", "1", "20")]
        public void When_DateIsInvalid_ValidationReturnsError(string day, string month, string year)
        {
            var viewModel = SetupViewModel(day, month, year);

            var result = _validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
        }

        public RegisterAddStandardVersionViewModel SetupViewModel(string day, string month, string year)
        {
            return new RegisterAddStandardVersionViewModel
            {
                EffectiveFromDay = day,
                EffectiveFromMonth = month,
                EffectiveFromYear = year
            };
        }
    }
}
