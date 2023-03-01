using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels;
using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Dashboard
{
    [TestFixture]
    public class DashboardControllerTests
    {
        private DashboardController _controller;
        private Mock<ILogger<DashboardController>> _logger;
        private Mock<IApplicationApiClient> _apiClient;
        private Mock<IWebConfiguration> _configuration;

        private ApplicationReviewStatusCounts _statusCounts;
        private string _dashboardUrl;


        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<DashboardController>>();
            _apiClient = new Mock<IApplicationApiClient>();
            _configuration = new Mock<IWebConfiguration>();
            _statusCounts = new ApplicationReviewStatusCounts
            {
                OrganisationApplicationsNew = 1,
                OrganisationApplicationsInProgress = 2,
                OrganisationApplicationsHasFeedback = 3,
                OrganisationApplicationsApproved = 4,
                StandardApplicationsNew = 5,
                StandardApplicationsInProgress = 6,
                StandardApplicationsHasFeedback = 7,
                StandardApplicationsApproved = 8
            };
            _dashboardUrl = "http://dashboard";

            _apiClient.Setup(x => x.GetApplicationReviewStatusCounts()).ReturnsAsync(_statusCounts);

            _configuration.Setup(c => c.RoatpOversightBaseUrl).Returns(_dashboardUrl);
            _configuration.Setup(c => c.RoatpAssessorBaseUrl).Returns(_dashboardUrl);
            _configuration.Setup(c => c.RoatpGatewayBaseUrl).Returns(_dashboardUrl);
            _configuration.Setup(c => c.RoatpFinanceBaseUrl).Returns(_dashboardUrl);

            _controller = new DashboardController(_logger.Object, _apiClient.Object, _configuration.Object);
        }

        [Test]
        public async Task confirm_viewModel_is_as_expected()
        {
            var result = await _controller.Index() as ViewResult;

            var viewModel = result.Model as DashboardViewModel;

            Assert.AreEqual(viewModel.RoatpOversightBaseUrl, _dashboardUrl);
            Assert.AreEqual(_statusCounts.OrganisationApplicationsNew, viewModel.OrganisationApplicationsNew);
            Assert.AreEqual(_statusCounts.OrganisationApplicationsInProgress, viewModel.OrganisationApplicationsInProgress);
            Assert.AreEqual(_statusCounts.OrganisationApplicationsHasFeedback, viewModel.OrganisationApplicationsHasFeedback);
            Assert.AreEqual(_statusCounts.OrganisationApplicationsApproved, viewModel.OrganisationApplicationsApproved);
            Assert.AreEqual(_statusCounts.StandardApplicationsNew, viewModel.StandardApplicationsNew);
            Assert.AreEqual(_statusCounts.StandardApplicationsInProgress, viewModel.StandardApplicationsInProgress);
            Assert.AreEqual(_statusCounts.StandardApplicationsHasFeedback, viewModel.StandardApplicationsHasFeedback);
            Assert.AreEqual(_statusCounts.StandardApplicationsApproved, viewModel.StandardApplicationsApproved);
        }

        [Test]
        public async Task confirm_exception_when_getting_statuscounts_is_handled_correctly()
        {
            var exception = new HttpRequestException();

            _apiClient.Setup(x => x.GetApplicationReviewStatusCounts()).ThrowsAsync(exception);

            var result = await _controller.Index() as ViewResult;
            var viewModel = result.Model as DashboardViewModel;

            _logger.Verify(l => l.Log(
                    It.Is<LogLevel>(level => level == LogLevel.Error),
                    It.Is<EventId>(id => id.Id == 0),
                    It.Is<It.IsAnyType>((obj, typ) => obj.ToString() == "Unable to GetApplicationReviewStatusCounts from EPAO Service" && typ.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(short.MinValue, viewModel.OrganisationApplicationsNew);
                Assert.AreEqual(short.MinValue, viewModel.OrganisationApplicationsInProgress);
                Assert.AreEqual(short.MinValue, viewModel.OrganisationApplicationsHasFeedback);
                Assert.AreEqual(short.MinValue, viewModel.OrganisationApplicationsApproved);
                Assert.AreEqual(short.MinValue, viewModel.StandardApplicationsNew);
                Assert.AreEqual(short.MinValue, viewModel.StandardApplicationsInProgress);
                Assert.AreEqual(short.MinValue, viewModel.StandardApplicationsHasFeedback);
                Assert.AreEqual(short.MinValue, viewModel.StandardApplicationsApproved);
            });
        }
    }
}
