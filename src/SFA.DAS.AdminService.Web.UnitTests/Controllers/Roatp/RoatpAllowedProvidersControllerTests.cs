using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types.AllowedProviders;
using SFA.DAS.AdminService.Web.Controllers.Roatp.AllowedProviders;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowedProviders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Roatp
{
    [TestFixture]
    public class RoatpAllowedProvidersControllerTests
    {
        private Mock<IRoatpApplicationApiClient> _applicationApplyApiClient;

        private RoatpAllowedProvidersController _controller;

        [SetUp]
        public void Before_each_test()
        {
            _applicationApplyApiClient = new Mock<IRoatpApplicationApiClient>();

            _controller = new RoatpAllowedProvidersController(_applicationApplyApiClient.Object)
            {
                ControllerContext = MockedControllerContext.Setup()
            };
        }

        [TestCase(null, null, null, null)]
        [TestCase("column", "order", null, null)]
        [TestCase("column", "order", "2021-01-01", "2021-01-31")]
        public async Task Index_creates_correct_viewmodel(string sortColumn, string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            _applicationApplyApiClient.Setup(x => x.GetAllowedProvidersList(sortColumn, sortOrder)).ReturnsAsync(new List<AllowedProvider>());

            var expectedViewModel = new AddUkprnToAllowedProvidersListViewModel
            {
                SortColumn = sortColumn,
                SortOrder = sortOrder,
                Ukprn = null,
                StartDate = startDate,
                EndDate = endDate,
                AllowedProviders = new List<AllowedProvider>()
            };

            var result = await _controller.Index(sortColumn, sortOrder, startDate, endDate);

            var viewResult = result as ViewResult;
            var viewModel = viewResult?.Model as AddUkprnToAllowedProvidersListViewModel;

            Assert.Multiple(() =>
            {
                Assert.That(viewModel, Is.Not.Null);
                Assert.That(viewModel.SortColumn, Is.EqualTo(expectedViewModel.SortColumn));
                Assert.That(viewModel.SortOrder, Is.EqualTo(expectedViewModel.SortOrder));
                Assert.That(viewModel.Ukprn, Is.EqualTo(expectedViewModel.Ukprn));
                Assert.That(viewModel.StartDate, Is.EqualTo(expectedViewModel.StartDate));
                Assert.That(viewModel.EndDate, Is.EqualTo(expectedViewModel.EndDate));
                Assert.That(viewModel.AllowedProviders, Is.EquivalentTo(expectedViewModel.AllowedProviders));
            });
        }

        [TestCase(null, null, null, null)]
        [TestCase("column", "order", null, null)]
        [TestCase("column", "order", "2021-01-01", "2021-01-31")]
        public async Task Index_calls_GetAllowedUkprns_with_expected_parameters(string sortColumn, string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            var result = await _controller.Index(sortColumn, sortOrder, startDate, endDate);

            _applicationApplyApiClient.Verify(x => x.GetAllowedProvidersList(sortColumn, sortOrder), Times.Once);
        }

        [TestCase(12345678, "2021-01-01", "2021-01-31")]
        public async Task AddUkprn_when_valid_ModelState_calls_AddToAllowUkprns_with_expected_parameters(int ukprn, DateTime startDate, DateTime endDate)
        {
            Assert.That(_controller.ModelState.IsValid, Is.True, "Test requires valid ModelState");

            var request = new AddUkprnToAllowedProvidersListViewModel { Ukprn = ukprn.ToString(), StartDate = startDate, EndDate = endDate };

            var result = await _controller.AddUkprn(request);

            _applicationApplyApiClient.Verify(x => x.AddToAllowedProviders(ukprn, startDate, endDate), Times.Once);
            _applicationApplyApiClient.Verify(x => x.GetAllowedProvidersList(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestCase("12345678", "2021-01-01", "2021-01-31")]
        public async Task AddUkprn_when_valid_ModelState_redirects_to_Index(string ukprn, DateTime startDate, DateTime endDate)
        {
            Assert.That(_controller.ModelState.IsValid, Is.True, "Test requires valid ModelState");

            var request = new AddUkprnToAllowedProvidersListViewModel { Ukprn = ukprn, StartDate = startDate, EndDate = endDate };

            var result = await _controller.AddUkprn(request);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        [TestCase(12345678, "2021-01-01", "2021-01-31")]
        public async Task AddUkprn_when_invalid_ModelState_does_not_call_AddToAllowUkprns(int ukprn, DateTime startDate, DateTime endDate)
        {
            _controller.ModelState.AddModelError("Ukprn", "Forced ModelState error");
            Assert.That(_controller.ModelState.IsValid, Is.False, "Test requires invalid ModelState");

            var request = new AddUkprnToAllowedProvidersListViewModel { Ukprn = ukprn.ToString(), StartDate = startDate, EndDate = endDate };

            var result = await _controller.AddUkprn(request);

            _applicationApplyApiClient.Verify(x => x.AddToAllowedProviders(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }

        [TestCase(12345678, "2021-01-01", "2021-01-31")]
        public async Task AddUkprn_when_invalid_ModelState_PRG_redirects_to_Index(int ukprn, DateTime startDate, DateTime endDate)
        {
            _controller.ModelState.AddModelError("Ukprn", "Forced ModelState error");
            Assert.That(_controller.ModelState.IsValid, Is.False, "Test requires invalid ModelState");

            var request = new AddUkprnToAllowedProvidersListViewModel { Ukprn = ukprn.ToString(), StartDate = startDate, EndDate = endDate };

            var result = await _controller.AddUkprn(request);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        [Test]
        public async Task ConfirmRemoveUkprn_when_ukprn_missing_redirects_to_Index()
        {
            var result = await _controller.ConfirmRemoveUkprn(null);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(_controller.Index)));

            _applicationApplyApiClient.Verify(x => x.GetAllowedProviderDetails(It.IsAny<int>()), Times.Never);
        }

        [TestCase(12345678)]
        public async Task ConfirmRemoveUkprn_creates_correct_viewmodel(int ukprn)
        {
            _applicationApplyApiClient.Setup(x => x.GetAllowedProviderDetails(ukprn)).ReturnsAsync(new AllowedProvider { Ukprn = ukprn });

            var expectedViewModel = new RemoveUkprnFromAllowedProvidersListViewModel
            {
                AllowedProvider = new AllowedProvider { Ukprn = ukprn }
            };

            var result = await _controller.ConfirmRemoveUkprn(ukprn.ToString());

            var viewResult = result as ViewResult;
            var viewModel = viewResult?.Model as RemoveUkprnFromAllowedProvidersListViewModel;

            Assert.Multiple(() =>
            {
                Assert.That(viewModel, Is.Not.Null);
                Assert.That(viewModel.AllowedProvider, Is.Not.Null);
                Assert.That(viewModel.AllowedProvider.Ukprn, Is.EqualTo(expectedViewModel.AllowedProvider.Ukprn));
            });
        }

        [TestCase(12345678)]
        public async Task RemoveUkprn_when_valid_ModelState_and_Confirm_True_calls_RemoveFromAllowedProviders_with_expected_parameters(int ukprn)
        {
            Assert.That(_controller.ModelState.IsValid, Is.True, "Test requires valid ModelState");

            var request = new RemoveUkprnFromAllowedProvidersListViewModel
            {
                Confirm = true
            };

            var result = await _controller.RemoveUkprn(ukprn, request);

            _applicationApplyApiClient.Verify(x => x.RemoveFromAllowedProviders(ukprn), Times.Once);
        }

        [TestCase(12345678)]
        public async Task RemoveUkprn_when_valid_ModelState_and_Confirm_True_redirects_to_UkprnRemoved(int ukprn)
        {
            Assert.That(_controller.ModelState.IsValid, Is.True, "Test requires valid ModelState");

            var request = new RemoveUkprnFromAllowedProvidersListViewModel
            {
                Confirm = true
            };

            var result = await _controller.RemoveUkprn(ukprn, request);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(_controller.UkprnRemoved)));
        }

        [TestCase(12345678)]
        public async Task RemoveUkprn_when_valid_ModelState_and_Confirm_False_does_not_call_RemoveFromAllowedProviders(int ukprn)
        {
            Assert.That(_controller.ModelState.IsValid, Is.True, "Test requires valid ModelState");

            var request = new RemoveUkprnFromAllowedProvidersListViewModel
            {
                Confirm = false
            };

            var result = await _controller.RemoveUkprn(ukprn, request);

            _applicationApplyApiClient.Verify(x => x.RemoveFromAllowedProviders(ukprn), Times.Never);
        }

        [TestCase(12345678)]
        public async Task RemoveUkprn_when_valid_ModelState_and_Confirm_False_redirects_to_Index(int ukprn)
        {
            Assert.That(_controller.ModelState.IsValid, Is.True, "Test requires valid ModelState");

            var request = new RemoveUkprnFromAllowedProvidersListViewModel
            {
                Confirm = false
            };

            var result = await _controller.RemoveUkprn(ukprn, request);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(_controller.Index)));
        }

        [TestCase(12345678)]
        public async Task RemoveUkprn_when_invalid_ModelState_does_not_call_RemoveFromAllowedUkprns(int ukprn)
        {
            _controller.ModelState.AddModelError("Confirm", "Forced ModelState error");
            Assert.That(_controller.ModelState.IsValid, Is.False, "Test requires invalid ModelState");

            var request = new RemoveUkprnFromAllowedProvidersListViewModel();

            var result = await _controller.RemoveUkprn(ukprn, request);

            _applicationApplyApiClient.Verify(x => x.RemoveFromAllowedProviders(It.IsAny<int>()), Times.Never);
        }

        [TestCase(12345678)]
        public async Task RemoveUkprn_when_invalid_ModelState_PRG_redirects_to_ConfirmRemoveUkprn(int ukprn)
        {
            _controller.ModelState.AddModelError("Confirm", "Forced ModelState error");
            Assert.That(_controller.ModelState.IsValid, Is.False, "Test requires invalid ModelState");

            var request = new RemoveUkprnFromAllowedProvidersListViewModel();

            var result = await _controller.RemoveUkprn(ukprn, request);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(_controller.ConfirmRemoveUkprn)));
        }

        [TestCase(12345678)]
        public void UkprnRemoved_creates_correct_viewmodel(int ukprn)
        {
            var expectedViewModel = new UkprnRemovedFromAllowedProvidersListViewModel
            {
                Ukprn = ukprn
            };

            var result = _controller.UkprnRemoved(ukprn.ToString());

            var viewResult = result as ViewResult;
            var viewModel = viewResult?.Model as UkprnRemovedFromAllowedProvidersListViewModel;

            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.Ukprn, Is.EqualTo(expectedViewModel.Ukprn));
        }
    }
}
