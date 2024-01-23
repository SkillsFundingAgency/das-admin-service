using AutoFixture;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Tests.Validators
{
    public class CertificateCheckViewModelValidatorTests
    {
        [TestCase(CertificateStatus.Draft)]
        public void WhenCertificateIsNotPrintStatus_ErrorsAreNotDisplayed(string certificateStatus)
        {
            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(new Builder()
                    .CreateNew<CertificateCheckViewModel>()
                    .With(q => q.SendTo = CertificateSendTo.None)
                    .With(q => q.Status = certificateStatus)
                    .With(q => q.Name = string.Empty)
                    .With(q => q.AddressLine1 = string.Empty)
                    .With(q => q.City = string.Empty)
                    .With(q => q.Postcode = string.Empty)
                    .Build());

            fixture.Validate();

            fixture.VerifyErrorCount(0);
        }

        [Test]
        [MoqAutoData]
        public void WhenOptionIsEmpty_ErrorsAreDisplayed(
            CertificateCheckViewModel vm,
            StandardOptions options)
        {
            // Arrange
            vm.SendTo = CertificateSendTo.None;
            vm.Status = CertificateStatus.Printed;
            vm.StandardUId = null;
            vm.Option = null;

            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(vm)
                .WithOrganisation()
                .WithCertificate(vm.Id, vm.StandardCode, vm.StandardUId, vm.Option)
                .WithStandardOptions(options);

            fixture.Validate();

            fixture.Verify(nameof(CertificateCheckViewModel.Option), null, "Add an option", true);
        }

        public void WhenSendToIsEmpty_ErrorIsDisplayed()
        {
            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(new Builder()
                .CreateNew<CertificateCheckViewModel>()
                .With(q => q.SendTo = CertificateSendTo.None)
                .With(q => q.Status = CertificateStatus.Printed)
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.Name), "NotEmptyValidator", "Enter the certificate receiver", true);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenSendToIsNotEmpty_ErrorIsNotDisplayed(CertificateSendTo sendTo)
        {
            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(new Builder()
                .CreateNew<CertificateCheckViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Status = CertificateStatus.Printed)
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.Name), "NotEmptyValidator", "Enter the certificate receiver", false);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenNameIsEmpty_ErrorIsDisplayed(CertificateSendTo sendTo)
        {
            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(new Builder()
                .CreateNew<CertificateCheckViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Status = CertificateStatus.Printed)
                .With(q => q.Name = string.Empty)
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.Name), "NotEmptyValidator", "You need to give a name of who will receive the certificate", true);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenNameIsNotEmpty_ErrorIsNotDisplayed(CertificateSendTo sendTo)
        {
            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(new Builder()
                .CreateNew<CertificateCheckViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Status = CertificateStatus.Printed)
                .With(q => q.Name = "A name")
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.Name), "NotEmptyValidator", "You need to give a name of who will receive the certificate", false);
        }

        [TestCase(CertificateSendTo.Employer,  true)]
        [TestCase(CertificateSendTo.Apprentice, false)]
        public void WhenEmployerNameIsEmpty_ErrorIsDisplayed(CertificateSendTo sendTo, bool shouldHaveError)
        {
            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(new Builder()
                .CreateNew<CertificateCheckViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Status = CertificateStatus.Printed)
                .With(q => q.Employer = string.Empty)
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.AddressLine1), "NotEmptyValidator", "Enter an address", shouldHaveError);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenEmployerNameIsNotEmpty_ErrorIsNotDisplayed(CertificateSendTo sendTo)
        {
            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(new Builder()
                .CreateNew<CertificateCheckViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Status = CertificateStatus.Printed)
                .With(q => q.Employer = "An employer name")
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.AddressLine1), "NotEmptyValidator", "Enter an address", false);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenAddress1IsEmpty_ErrorIsDisplay(CertificateSendTo sendTo)
        {
            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(new Builder()
                .CreateNew<CertificateCheckViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Status = CertificateStatus.Printed)
                .With(q => q.AddressLine1 = string.Empty)
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.AddressLine1), "NotEmptyValidator", "Enter an address", true);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenAddress1IsNotEmpty_ErrorIsNotDisplayed(CertificateSendTo sendTo)
        {
            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(new Builder()
                .CreateNew<CertificateCheckViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Status = CertificateStatus.Printed)
                .With(q => q.AddressLine1 = "A street address")
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.AddressLine1), "NotEmptyValidator", "Enter an address", false);
        }


        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenCityIsEmpty_ErrorIsDisplay(CertificateSendTo sendTo)
        {
            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(new Builder()
                .CreateNew<CertificateCheckViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Status = CertificateStatus.Printed)
                .With(q => q.City = string.Empty)
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.AddressLine1), "NotEmptyValidator", "Enter an address", true);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenCityIsNotEmpty_ErrorIsNotDisplayed(CertificateSendTo sendTo)
        {
            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(new Builder()
                .CreateNew<CertificateCheckViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Status = CertificateStatus.Printed)
                .With(q => q.City = "A city")
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.AddressLine1), "NotEmptyValidator", "Enter an address", false);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenPostcodeIsEmpty_ErrorIsDisplay(CertificateSendTo sendTo)
        {
            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(new Builder()
                .CreateNew<CertificateCheckViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Status = CertificateStatus.Printed)
                .With(q => q.Postcode = string.Empty)
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.AddressLine1), "NotEmptyValidator", "Enter an address", true);
        }

        [TestCase(CertificateSendTo.Employer)]
        [TestCase(CertificateSendTo.Apprentice)]
        public void WhenPostcodeIsNotEmpty_ErrorIsNotDisplayed(CertificateSendTo sendTo)
        {
            var fixture = new CertificateCheckViewModelValidatorTestsFixture()
                .WithCertificateCheckViewModel(new Builder()
                .CreateNew<CertificateCheckViewModel>()
                .With(q => q.SendTo = sendTo)
                .With(q => q.Status = CertificateStatus.Printed)
                .With(q => q.Postcode = "123456")
                .Build());

            fixture.Validate();

            fixture.Verify(nameof(CertificateAddressViewModel.AddressLine1), "NotEmptyValidator", "Enter an address", false);
        }

        public class CertificateCheckViewModelValidatorTestsFixture
        {
            private Fixture _fixture;
            private Certificate _certificate;
            private Organisation _organisation;

            private Mock<IStandardVersionApiClient> _standardVersionApiClient;
            private CertificateCheckViewModel _sut;
            private ValidationResult _validationResult;

            public CertificateCheckViewModelValidatorTestsFixture()
            {
                _fixture = new Fixture();
                _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

                _standardVersionApiClient = new Mock<IStandardVersionApiClient>();
            }

            public CertificateCheckViewModelValidatorTestsFixture WithCertificateCheckViewModel(CertificateCheckViewModel viewModel)
            {
                _sut = viewModel;
                return this;
            }

            public CertificateCheckViewModelValidatorTestsFixture WithOrganisation()
            {
                _organisation = _fixture.Create<Organisation>();

                return this;
            }

            public CertificateCheckViewModelValidatorTestsFixture WithCertificate(Guid id, int standardCode, string standardUId, string option)
            {
                _certificate = _fixture.Create<Certificate>();
                _certificate.Id = id;
                _certificate.StandardCode = standardCode;
                _certificate.StandardUId = standardUId;
                var certificateData = _fixture.Create<CertificateData>();
                certificateData.CourseOption = option;
                _certificate.CertificateData = JsonConvert.SerializeObject(certificateData);
                _certificate.OrganisationId = _organisation.Id;

                return this;
            }

            public CertificateCheckViewModelValidatorTestsFixture WithStandardOptions(StandardOptions standardOptions)
            {
                // when the StandardUid is missing the StandardId used to return options is the StandardCode instead
                if (!string.IsNullOrWhiteSpace(_certificate.StandardUId))
                {
                    _standardVersionApiClient.Setup(p => p.GetStandardOptions(_certificate.StandardUId)).ReturnsAsync(standardOptions);
                }
                else
                {
                    _standardVersionApiClient.Setup(p => p.GetStandardOptions(_certificate.StandardCode.ToString())).ReturnsAsync(standardOptions);
                }

                return this;
            }

            public void Validate()
            {
                _validationResult = new CertificateCheckViewModelValidator(_standardVersionApiClient.Object).Validate(_sut);
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

            internal void VerifyErrorCount(int count)
            {
                _validationResult.Errors.Count.Should().Be(count);
            }
        }
    }
}



