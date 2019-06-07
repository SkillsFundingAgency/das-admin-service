using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels;

namespace SFA.DAS.AdminService.Web.Tests.Validators
{
    public class WhenCreateAddressViewModelValidatorSuccceeds
    {
        private static ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            var certificateAddressViewModel = new Builder()
                .CreateNew<CertificateAddressViewModel>()
                .With(q => q.Postcode = "L6 3TY")
                .Build();

            _validationResult = new CertificateAddressViewModelValidator().Validate(certificateAddressViewModel);
        }

        [Test]
        public void ThenItShouldSucceed()
        {
            _validationResult.IsValid.Should().BeTrue();
        }
    }
}



