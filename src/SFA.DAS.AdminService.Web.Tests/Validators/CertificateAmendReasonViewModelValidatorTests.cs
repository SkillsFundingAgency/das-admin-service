using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Api.Types.Enums;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Tests.Validators
{
    public class CertificateAmendReasonViewModelValidatorTests
    {
        [TestCaseSource(nameof(ValidIncidentNumberCases))]
        public void WhenValidatedPassed_ValidIncidentNumber_ThenNoErrorsAreRaised(string incidentNumber, List<string> reasons, string otherReason)
        {
            // Arrange
            var fixture = new CertificateAmendReasonViewModelValidatorTestsFixture()
                .WithIncidentNumber(incidentNumber)
                .WithReasons(reasons)
                .WithOtherReason(otherReason);

            // Act
            var result = fixture.Validate();

            // Assert
            fixture.VerifyErrors(result, 0, new List<ValidationFailure>());
        }

        [TestCaseSource(nameof(InvalidIncidentNumberCases))]
        public void WhenValidatedPassed_InvalidIncidentNumber_ThenErrorsAreRaised(string incidentNumber, List<string> reasons, string otherReason)
        {
            // Arrange
            var fixture = new CertificateAmendReasonViewModelValidatorTestsFixture()
                .WithIncidentNumber(incidentNumber)
                .WithReasons(reasons)
                .WithOtherReason(otherReason);

            // Act
            var result = fixture.Validate();

            // Assert
            fixture.VerifyErrors(result, 1, new List<ValidationFailure>
            {
                new ValidationFailure(nameof(CertificateAmendReasonViewModel.IncidentNumber), "Enter the ticket reference", incidentNumber)
            });
        }

        [TestCaseSource(nameof(ValidReasonsCases))]
        public void WhenValidatedPassed_ValidReasons_ThenNoErrorsAreRaised(string incidentNumber, List<string> reasons, string otherReason)
        {
            // Arrange
            var fixture = new CertificateAmendReasonViewModelValidatorTestsFixture()
                .WithIncidentNumber(incidentNumber)
                .WithReasons(reasons)
                .WithOtherReason(otherReason);

            // Act
            var result = fixture.Validate();

            // Assert
            fixture.VerifyErrors(result, 0, new List<ValidationFailure>());
        }

        [TestCaseSource(nameof(InvalidReasonsCases))]
        public void WhenValidatedPassed_InvalidReasons_ThenErrorsAreRaised(string incidentNumber, List<string> reasons, string otherReason)
        {
            // Arrange
            var fixture = new CertificateAmendReasonViewModelValidatorTestsFixture()
                .WithIncidentNumber(incidentNumber)
                .WithReasons(reasons)
                .WithOtherReason(otherReason);

            // Act
            var result = fixture.Validate();

            // Assert
            fixture.VerifyErrors(result, 1, new List<ValidationFailure>
            {
                new ValidationFailure(nameof(CertificateAmendReasonViewModel.Reasons), "Select reason(s) for amending certificate information", reasons)
            });
        }

        [TestCaseSource(nameof(ValidOtherReasonCases))]
        public void WhenValidatedPassed_ValidOtherReason_ThenNoErrorsAreRaised(string incidentNumber, List<string> reasons, string otherReason)
        {
            // Arrange
            var fixture = new CertificateAmendReasonViewModelValidatorTestsFixture()
                .WithIncidentNumber(incidentNumber)
                .WithReasons(reasons)
                .WithOtherReason(otherReason);

            // Act
            var result = fixture.Validate();

            // Assert
            fixture.VerifyErrors(result, 0, new List<ValidationFailure>());
        }

        [TestCaseSource(nameof(InvalidOtherReasonCases))]
        public void WhenValidatedPassed_InvalidOtherReason_ThenErrorsAreRaised(string incidentNumber, List<string> reasons, string otherReason)
        {
            // Arrange
            var fixture = new CertificateAmendReasonViewModelValidatorTestsFixture()
                .WithIncidentNumber(incidentNumber)
                .WithReasons(reasons)
                .WithOtherReason(otherReason);

            // Act
            var result = fixture.Validate();

            // Assert
            fixture.VerifyErrors(result, 1, new List<ValidationFailure>
            {
                new ValidationFailure(nameof(CertificateAmendReasonViewModel.OtherReason), "Give details", otherReason)
            });
        }

        static object[] ValidIncidentNumberCases =
        {
            new object[] { "1", new List<string> { AmendReasons.ApprenticeAddress.ToString() }, string.Empty },
            new object[] { "12", new List<string> { AmendReasons.ApprenticeAddress.ToString() }, string.Empty },
            new object[] { "123", new List<string> { AmendReasons.ApprenticeAddress.ToString() }, string.Empty }
        };

        static object[] ValidReasonsCases =
        {
            new object[] { "1", new List<string> { AmendReasons.ApprenticeAddress.ToString() }, string.Empty },
            new object[] { "1", new List<string> 
            { 
                AmendReasons.ApprenticeAddress.ToString(),
                AmendReasons.ApprenticeDetails.ToString()
            }, string.Empty },
            new object[] { "1", new List<string>
            {
                AmendReasons.ApprenticeAddress.ToString(),
                AmendReasons.ApprenticeDetails.ToString(),
                AmendReasons.EmployerAddress.ToString()
            }, string.Empty },
        };

        static object[] ValidOtherReasonCases =
        {
            new object[] { "1", new List<string> { "Other" }, "Some value" },
            new object[] { "1", new List<string>
            {
                AmendReasons.ApprenticeAddress.ToString(),
                "Other"
            }, "Some value" }
        };

        static object[] InvalidIncidentNumberCases =
        {
            new object[] { string.Empty, new List<string> { AmendReasons.ApprenticeAddress.ToString() }, string.Empty },
            new object[] { " ", new List<string> { AmendReasons.ApprenticeAddress.ToString() }, string.Empty }
        };

        static object[] InvalidReasonsCases =
        {
            new object[] { "1", new List<string>(), string.Empty }
        };

        static object[] InvalidOtherReasonCases =
        {
            new object[] { "1", new List<string> { "Other" }, string.Empty }
        };


        public class CertificateAmendReasonViewModelValidatorTestsFixture
        {
            private CertificateAmendReasonViewModel _viewModel;
            private CertificateAmendReasonViewModelValidator _sut;

            public CertificateAmendReasonViewModelValidatorTestsFixture()
            {
                _viewModel = new CertificateAmendReasonViewModel();
                _sut = new CertificateAmendReasonViewModelValidator();
            }

            public CertificateAmendReasonViewModelValidatorTestsFixture WithIncidentNumber(string incidentNumber)
            {
                _viewModel.IncidentNumber = incidentNumber;
                return this;
            }

            public CertificateAmendReasonViewModelValidatorTestsFixture WithReasons(List<string> amendReasons)
            {
                _viewModel.Reasons = amendReasons;
                return this;
            }

            public CertificateAmendReasonViewModelValidatorTestsFixture WithOtherReason(string  otherReason)
            {
                _viewModel.OtherReason = otherReason;
                return this;
            }

            public ValidationResult Validate()
            {
                return _sut.Validate(_viewModel);
            }

            public void VerifyErrors(ValidationResult validationResult, int errorCount, IList<ValidationFailure> expectedValidationFailures)
            {
                validationResult.Errors.Count.Should().Be(errorCount);
                foreach (var expectedValidationFailure in expectedValidationFailures)
                {
                    validationResult.Errors.Should().Contain(p =>
                        p.PropertyName == expectedValidationFailure.PropertyName &&
                        p.ErrorMessage == expectedValidationFailure.ErrorMessage &&
                        p.AttemptedValue == expectedValidationFailure.AttemptedValue);
                }
            }
        }
    }
}
