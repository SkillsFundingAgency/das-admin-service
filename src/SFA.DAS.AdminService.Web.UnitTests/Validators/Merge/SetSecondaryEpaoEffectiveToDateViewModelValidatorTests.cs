using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
using System;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Tests.Validators.Merge
{
    public class SetSecondaryEpaoEffectiveToDateViewModelValidatorTests
    {
        private SetSecondaryEpaoEffectiveToDateViewModelValidator Validator;

        [SetUp]
        public void Arrange()
        {
            Validator = new SetSecondaryEpaoEffectiveToDateViewModelValidator();
        }

        [Test]
        public void When_ValidDateIsEntered_Then_ReturnTrue()
        {
            var futureDate = DateTime.Now.AddDays(1);

            var viewModel = SetUpViewModel(futureDate);

            var result = Validator.Validate(viewModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void When_DateInPastIsEntered_Then_ReturnError()
        {
            var pastDate = DateTime.Now.AddDays(-1);

            var viewModel = SetUpViewModel(pastDate);

            var result = Validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
            result.Errors.Single().ErrorMessage.Should().Be("Effective to date must be in the future");
        }

        [TestCase("-1", "1", "2022")]
        [TestCase("32", "1", "2022")]
        [TestCase("1", "13", "2022")]
        [TestCase("30", "2", "2022")]
        public void When_DateIsInvalid_Then_ReturnError(string day, string month, string year)
        {
            var viewModel = SetUpViewModel(day, month, year);

            var result = Validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
            result.Errors.Single().ErrorMessage.Should().Be("Effective to date must be a real date");
        }

        [TestCase("1", "1", null, 1)]
        [TestCase("1", null, "2022", 1)]
        [TestCase(null, "1", "2022", 1)]
        [TestCase("1", null , null, 2)]
        [TestCase(null, "1", null, 2)]
        [TestCase(null, null, "2022", 2)]
        public void When_DateIsPartiallyCompleted_Then_ReturnErrors(string day, string month, string year, int expectedErrorCount)
        {
            var viewModel = SetUpViewModel(day, month, year);

            var result = Validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(expectedErrorCount);
        }

        [Test]
        public void When_NoDateIsEntered_Then_ReturnError()
        {
            var viewModel = SetUpViewModel(null, null, null);

            var result = Validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
            result.Errors.Single().ErrorMessage.Should().Be("Enter an Effective to date");
        }

        private static SetSecondaryEpaoEffectiveToDateViewModel SetUpViewModel(string day, string month, string year)
        {
            return new SetSecondaryEpaoEffectiveToDateViewModel
            {
                Day = day,
                Month = month,
                Year = year
            };
        }

        private static SetSecondaryEpaoEffectiveToDateViewModel SetUpViewModel(DateTime date)
        {
            return new SetSecondaryEpaoEffectiveToDateViewModel
            {
                Day = date.Day.ToString(),
                Month = date.Month.ToString(),
                Year = date.Year.ToString()
            };
        }
    }
}
