using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators.Roatp.AllowedProviders;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowedProviders;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.AllowedProviders;

namespace SFA.DAS.AdminService.Web.Tests.Validators.Roatp.AllowedProviders
{
    public class RemoveUkprnFromAllowListValidatorTests
    {
        private RemoveUkprnFromAllowProvidersListValidator _validator;

        private RemoveUkprnFromAllowedProvidersListViewModel _viewModel;

        [SetUp]
        public void Before_each_test()
        {
            _validator = new RemoveUkprnFromAllowProvidersListValidator();

            _viewModel = new RemoveUkprnFromAllowedProvidersListViewModel
            {
                AllowedProvider = new AllowedProvider { Ukprn = 12345678 },
                Confirm = false
            };
        }

        [Test]
        public void Validator_passes_when_viewmodel_is_valid()
        {
            var validationResponse = _validator.Validate(_viewModel);

            Assert.IsTrue(validationResponse.IsValid);
            CollectionAssert.IsEmpty(validationResponse.Errors);
        }

        [Test]
        public void Validator_rejects_missing_AllowedProvider()
        {
            _viewModel.AllowedProvider = null;

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "AllowedProvider");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_missing_Confirm()
        {
            _viewModel.Confirm = null;

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "Confirm");
            error.Should().NotBeNull();
        }
    }
}
