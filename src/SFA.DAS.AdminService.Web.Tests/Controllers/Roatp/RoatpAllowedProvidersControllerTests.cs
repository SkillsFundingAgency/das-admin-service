using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.AdminService.Web.Controllers.Roatp;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowedProviders;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.AllowedProviders;

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
        public async Task List_creates_correct_viewmodel(string sortColumn, string sortOrder, DateTime? startDate, DateTime? endDate)
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

            var result = await _controller.List(sortColumn, sortOrder, startDate, endDate);

            var viewResult = result as ViewResult;
            var viewModel = viewResult?.Model as AddUkprnToAllowedProvidersListViewModel;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(expectedViewModel.SortColumn, viewModel.SortColumn);
            Assert.AreEqual(expectedViewModel.SortOrder, viewModel.SortOrder);
            Assert.AreEqual(expectedViewModel.Ukprn, viewModel.Ukprn);
            Assert.AreEqual(expectedViewModel.StartDate, viewModel.StartDate);
            Assert.AreEqual(expectedViewModel.EndDate, viewModel.EndDate);
            CollectionAssert.AreEquivalent(expectedViewModel.AllowedProviders, viewModel.AllowedProviders);
        }

        [TestCase(null, null, null, null)]
        [TestCase("column", "order", null, null)]
        [TestCase("column", "order", "2021-01-01", "2021-01-31")]
        public async Task List_calls_GetAllowedUkprns_with_expected_parameters(string sortColumn, string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            var result = await _controller.List(sortColumn, sortOrder, startDate, endDate);

            _applicationApplyApiClient.Verify(x => x.GetAllowedProvidersList(sortColumn, sortOrder), Times.Once);
        }

        [TestCase(12345678, "2021-01-01", "2021-01-31")]
        public async Task AddUkprn_when_valid_ModelState_calls_AddToAllowUkprns_with_expected_parameters(int ukprn, DateTime startDate, DateTime endDate)
        {
            Assert.IsTrue(_controller.ModelState.IsValid, "Test requires valid ModelState");

            var request = new AddUkprnToAllowedProvidersListViewModel { Ukprn = ukprn.ToString(), StartDate = startDate, EndDate = endDate };

            var result = await _controller.AddUkprn(request);

            _applicationApplyApiClient.Verify(x => x.AddToAllowedProviders(ukprn, startDate, endDate), Times.Once);
            _applicationApplyApiClient.Verify(x => x.GetAllowedProvidersList(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestCase("12345678", "2021-01-01", "2021-01-31")]
        public async Task AddUkprn_when_valid_ModelState_redirects_to_List(string ukprn, DateTime startDate, DateTime endDate)
        {
            Assert.IsTrue(_controller.ModelState.IsValid, "Test requires valid ModelState");

            var request = new AddUkprnToAllowedProvidersListViewModel { Ukprn = ukprn, StartDate = startDate, EndDate = endDate };

            var result = await _controller.AddUkprn(request);

            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("List", redirectResult.ActionName);
        }

        [TestCase(12345678, "2021-01-01", "2021-01-31")]
        public async Task AddUkprn_when_invalid_ModelState_does_not_call_AddToAllowUkprns(int ukprn, DateTime startDate, DateTime endDate)
        {
            _controller.ModelState.AddModelError("Ukprn", "Forced ModelState error");
            Assert.IsFalse(_controller.ModelState.IsValid, "Test requires invalid ModelState");

            var request = new AddUkprnToAllowedProvidersListViewModel { Ukprn = ukprn.ToString(), StartDate = startDate, EndDate = endDate };

            var result = await _controller.AddUkprn(request);

            _applicationApplyApiClient.Verify(x => x.AddToAllowedProviders(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
            _applicationApplyApiClient.Verify(x => x.GetAllowedProvidersList(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestCase("12345678", "2021-01-01", "2021-01-31")]
        public async Task AddUkprn_when_invalid_ModelState_shows_List_view(string ukprn, DateTime startDate, DateTime endDate)
        {
            _controller.ModelState.AddModelError("Ukprn", "Forced ModelState error");
            Assert.IsFalse(_controller.ModelState.IsValid, "Test requires invalid ModelState");

            var request = new AddUkprnToAllowedProvidersListViewModel { Ukprn = ukprn, StartDate = startDate, EndDate = endDate };

            var result = await _controller.AddUkprn(request);

            var viewResult = result as ViewResult;
            Assert.IsTrue(viewResult.ViewName.EndsWith("List.cshtml"));
        }
    }
}
