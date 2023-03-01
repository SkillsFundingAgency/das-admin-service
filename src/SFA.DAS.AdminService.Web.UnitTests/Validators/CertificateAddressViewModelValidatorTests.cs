using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Tests.Validators
{
    public class CertificateAddressViewModelValidatorTests
    {
        [TestCase(CertificateSendTo.Employer, true)]
        [TestCase(CertificateSendTo.Apprentice, false)]
        public void WhenNameIsEmpty_ErrorIsDisplay(CertificateSendTo sendTo, bool shouldHaveError)
        {
            var fixture = new CertificateAddressViewModelValidatorTestsFixture()
                .WithCertificateAddressViewModel(new Builder()
                .CreateNew<CertificateAddressViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Name = string.Empty)
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.Name), "NotEmptyValidator", "Enter recipient's name", shouldHaveError);
        }

        [TestCase(CertificateSendTo.Employer,  true)]
        [TestCase(CertificateSendTo.Apprentice, false)]
        public void WhenEmployerNameIsEmpty_ErrorIsDisplay(CertificateSendTo sendTo, bool shouldHaveError)
        {
            var fixture = new CertificateAddressViewModelValidatorTestsFixture()
                .WithCertificateAddressViewModel(new Builder()
                .CreateNew<CertificateAddressViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Employer = string.Empty)
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.Employer), "NotEmptyValidator", "Enter an employer", shouldHaveError);
        }

        [TestCase(CertificateSendTo.Employer, false)]
        [TestCase(CertificateSendTo.Apprentice, false)]
        public void WhenEmployerNameIsNotEmpty_ErrorIsDisplay(CertificateSendTo sendTo, bool shouldHaveError)
        {
            var fixture = new CertificateAddressViewModelValidatorTestsFixture()
                .WithCertificateAddressViewModel(new Builder()
                .CreateNew<CertificateAddressViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Employer = "An employer name")
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.Employer), "NotEmptyValidator", "Enter an employer", shouldHaveError);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenAddress1IsEmpty_ErrorIsDisplay(CertificateSendTo sendTo)
        {
            var fixture = new CertificateAddressViewModelValidatorTestsFixture()
                .WithCertificateAddressViewModel(new Builder()
                .CreateNew<CertificateAddressViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.AddressLine1 = string.Empty)
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.AddressLine1), "NotEmptyValidator", "Enter a building or street", true);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenAddress1NotIsEmpty_ErrorIsDisplay(CertificateSendTo sendTo)
        {
            var fixture = new CertificateAddressViewModelValidatorTestsFixture()
                .WithCertificateAddressViewModel(new Builder()
                .CreateNew<CertificateAddressViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.AddressLine1 = "A street address")
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.AddressLine1), "NotEmptyValidator", "Enter a building or street", false);
        }


        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenCityIsEmpty_ErrorIsDisplay(CertificateSendTo sendTo)
        {
            var fixture = new CertificateAddressViewModelValidatorTestsFixture()
                .WithCertificateAddressViewModel(new Builder()
                .CreateNew<CertificateAddressViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.City = string.Empty)
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.City), "NotEmptyValidator", "Enter a town or city", true);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenCityIsNotEmpty_ErrorIsDisplay(CertificateSendTo sendTo)
        {
            var fixture = new CertificateAddressViewModelValidatorTestsFixture()
                .WithCertificateAddressViewModel(new Builder()
                .CreateNew<CertificateAddressViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.City = "A city")
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.City), "NotEmptyValidator", "Enter a town or city", false);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenPostcodeIsEmpty_ErrorIsDisplay(CertificateSendTo sendTo)
        {
            var fixture = new CertificateAddressViewModelValidatorTestsFixture()
                .WithCertificateAddressViewModel(new Builder()
                .CreateNew<CertificateAddressViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Postcode = string.Empty)
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.Postcode), "NotEmptyValidator", "Enter a postcode", true);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenPostcodeIsNotEmpty_ErrorIsDisplay(CertificateSendTo sendTo)
        {
            var fixture = new CertificateAddressViewModelValidatorTestsFixture()
                .WithCertificateAddressViewModel(new Builder()
                .CreateNew<CertificateAddressViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Postcode = "123456")
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.Postcode), "NotEmptyValidator", "Enter a postcode", false);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenPostcodeIsInvalid_ErrorIsDisplay(CertificateSendTo sendTo)
        {
            var fixture = new CertificateAddressViewModelValidatorTestsFixture()
                .WithCertificateAddressViewModel(new Builder()
                .CreateNew<CertificateAddressViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Postcode = "123456")
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.Postcode), "RegularExpressionValidator", "Enter a valid UK postcode", true);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenPostcodeIsValid_ErrorIsDisplay(CertificateSendTo sendTo)
        {
            var fixture = new CertificateAddressViewModelValidatorTestsFixture()
                .WithCertificateAddressViewModel(new Builder()
                .CreateNew<CertificateAddressViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Postcode = "SW1A 2AA")
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.Postcode), "RegularExpressionValidator", "Enter a valid UK postcode", false);
        }

        public class CertificateAddressViewModelValidatorTestsFixture
        {
            private CertificateAddressViewModel _sut;
            private ValidationResult _validationResult;

            public CertificateAddressViewModelValidatorTestsFixture WithCertificateAddressViewModel(CertificateAddressViewModel viewModel)
            {
                _sut = viewModel;
                return this;
            }

            public void Validate()
            {
                _validationResult = new CertificateAddressViewModelValidator().Validate(_sut);
            }

            public void Verify(string propertyName, string errorCode, string errorMessage, bool shouldHaveError)
            {
                var errors = _validationResult.Errors.FirstOrDefault(q =>
                    q.PropertyName == propertyName && 
                    q.ErrorCode == errorCode &&
                    q.ErrorMessage == errorMessage );

                if (shouldHaveError)
                {
                    errors.Should().NotBeNull();
                }
                else
                {
                    errors.Should().BeNull();
                }
            }
        }
    }
}



