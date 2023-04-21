using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Validators.Roatp.AllowedProviders;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowedProviders;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.AllowedProviders;

namespace SFA.DAS.AdminService.Web.Tests.Validators.Roatp.AllowedProviders
{
    public class AllowedListValidatorTests
    {
        private Mock<IRoatpApplicationApiClient> _applicationApplyApiClient;

        private AddUkprnToAllowProvidersListValidator _validator;

        private AddUkprnToAllowedProvidersListViewModel _viewModel;

        [SetUp]
        public void Before_each_test()
        {
            _applicationApplyApiClient = new Mock<IRoatpApplicationApiClient>();

            _applicationApplyApiClient.Setup(x => x.GetAllowedProviderDetails(It.IsAny<int>())).ReturnsAsync(default(AllowedProvider));

            _validator = new AddUkprnToAllowProvidersListValidator(_applicationApplyApiClient.Object);

            _viewModel = new AddUkprnToAllowedProvidersListViewModel
            {
                Ukprn = "12345678",
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MaxValue
            };
        }

        [Test]
        public async Task Validator_passes_when_viewmodel_is_valid()
        {
            var validationResponse = await _validator.ValidateAsync(_viewModel);

            Assert.IsTrue(validationResponse.IsValid);
            CollectionAssert.IsEmpty(validationResponse.Errors);
        }

        [Test]
        public void Validator_rejects_missing_Ukprn()
        {
            _viewModel.Ukprn = null;

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "Ukprn");
            error.Should().NotBeNull();
        }

        [Test]
        public async Task Validator_rejects_incorrect_formatted_Ukprn()
        {
            _viewModel.Ukprn = "invalid format";

            var validationResponse = await _validator.ValidateAsync(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "Ukprn");
            error.Should().NotBeNull();
        }

        [Test]
        public async Task Validator_rejects_Ukprn_below_10000000()
        {
            _viewModel.Ukprn = (10000000 - 1).ToString();

            var validationResponse = await _validator.ValidateAsync(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "Ukprn");
            error.Should().NotBeNull();
        }

        [Test]
        public async Task Validator_rejects_Ukprn_above_19999999()
        {
            _viewModel.Ukprn = (19999999 + 1).ToString();

            var validationResponse = await _validator.ValidateAsync(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "Ukprn");
            error.Should().NotBeNull();
        }

        [Test]
        public async Task Validator_rejects_Ukprn_if_already_in_allow_list()
        {
            int.TryParse(_viewModel.Ukprn, out var ukprn);
            var allowedUkprnEntry = new AllowedProvider { Ukprn = ukprn };
            _applicationApplyApiClient.Setup(x => x.GetAllowedProviderDetails(ukprn)).ReturnsAsync(new AllowedProvider());

            var validationResponse = await _validator.ValidateAsync(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "Ukprn");
            error.Should().NotBeNull();
        }

        [Test]
        public async Task Validator_rejects_missing_StartDate()
        {
            _viewModel.StartDate = null;

            var validationResponse = await _validator.ValidateAsync(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "StartDate");
            error.Should().NotBeNull();
        }

        [Test]
        public async Task Validator_rejects_StartDate_later_than_EndDate()
        {
            _viewModel.StartDate = DateTime.MaxValue;
            _viewModel.EndDate = DateTime.MinValue;

            var validationResponse = await _validator.ValidateAsync(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "StartDate");
            error.Should().NotBeNull();
        }

        [Test]
        public async Task Validator_rejects_missing_EndDate()
        {
            _viewModel.EndDate = null;

            var validationResponse = await _validator.ValidateAsync(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "EndDate");
            error.Should().NotBeNull();
        } 
    }
}
