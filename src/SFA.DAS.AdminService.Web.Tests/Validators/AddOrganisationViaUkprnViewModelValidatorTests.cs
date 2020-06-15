using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Validators
{
    [TestFixture]
    public class AddOrganisationViaUkprnViewModelValidatorTests
    {
        private AddOrganisationViaUkprnViewModel _viewModel;
        private Mock<IRoatpOrganisationValidator> _roatpOrganisationValidator;
        private Mock<IRoatpApiClient> _apiClient;

        [SetUp]
        public void Before_each_test()
        {
            _apiClient = new Mock<IRoatpApiClient>();
            _apiClient.Setup(x => x.DuplicateUKPRNCheck(It.IsAny<Guid>(), It.IsAny<long>()))
                .ReturnsAsync(new DuplicateCheckResponse { DuplicateFound = false, DuplicateOrganisationName = null });
            _roatpOrganisationValidator = new Mock<IRoatpOrganisationValidator>();
        }

        [Test]
        public void Validator_passes_valid_ukprn()
        {
            var errors = new List<ValidationErrorDetail>();
            _roatpOrganisationValidator.Setup(x => x.IsValidUKPRN(It.IsAny<string>())).Returns(errors);
            _viewModel = new AddOrganisationViaUkprnViewModel { UKPRN = "11112222" };

            var validator = new AddOrganisationViaUkprnViewModelValidator(_roatpOrganisationValidator.Object, _apiClient.Object);
            var validationResult = validator.Validate(_viewModel);

            Assert.AreEqual(0, validationResult.Errors.Count);
        }

        [Test]
        public void Validator_fails_invalid_ukprn()
        {
            var errors = new List<ValidationErrorDetail>
            {
                new ValidationErrorDetail
                {
                    Field = "ukprn",
                    ErrorMessage = "wrong length"
                }
            };
            _roatpOrganisationValidator.Setup(x => x.IsValidUKPRN(It.IsAny<string>())).Returns(errors);
            _viewModel = new AddOrganisationViaUkprnViewModel { UKPRN = "111222" };

            var validator = new AddOrganisationViaUkprnViewModelValidator(_roatpOrganisationValidator.Object, _apiClient.Object);
            var validationResult = validator.Validate(_viewModel);

            Assert.AreEqual(1, validationResult.Errors.Count);
        }
    }
}