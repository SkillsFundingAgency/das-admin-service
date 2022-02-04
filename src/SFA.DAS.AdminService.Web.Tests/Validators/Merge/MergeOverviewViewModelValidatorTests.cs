using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Tests.Validators.Merge
{
    public class MergeOverviewViewModelValidatorTests
    {
        private Fixture _autoFixture;

        private MergeOverviewViewModelValidator Validator;

        [SetUp]
        public void Arrange()
        {
            _autoFixture = new Fixture();

            Validator = new MergeOverviewViewModelValidator();
        }

        [Test]
        public void ValidatePrimaryEpaoId()
        {
            var viewModel = _autoFixture.Build<MergeOverviewViewModel>()
                .Without(vm => vm.PrimaryEpaoId).Create();

            var result = Validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
            result.Errors.Where(e => e.ErrorMessage == "Confirm a primary EPAO").Count().Should().Be(1);
        }

        [Test]
        public void ValidateSecondaryEpaoId()
        {
            var viewModel = _autoFixture.Build<MergeOverviewViewModel>()
                .Without(vm => vm.SecondaryEpaoId).Create();

            var result = Validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
            result.Errors.Where(e => e.ErrorMessage == "Confirm a secondary EPAO").Count().Should().Be(1);
        }

        [Test]
        public void ValidateSecondaryEpaoEffectiveTo()
        {
            var viewModel = _autoFixture.Build<MergeOverviewViewModel>()
                .Without(vm => vm.SecondaryEpaoEffectiveTo).Create();

            var result = Validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
            result.Errors.Where(e => e.ErrorMessage == "Confirm secondary EPAO effective to").Count().Should().Be(1);
        }
    }
}
