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
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
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

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.RoatpOversightBaseUrl, Is.EqualTo(_dashboardUrl));
                Assert.That(viewModel.OrganisationApplicationsNew, Is.EqualTo(_statusCounts.OrganisationApplicationsNew));
                Assert.That(viewModel.OrganisationApplicationsInProgress, Is.EqualTo(_statusCounts.OrganisationApplicationsInProgress));
                Assert.That(viewModel.OrganisationApplicationsHasFeedback, Is.EqualTo(_statusCounts.OrganisationApplicationsHasFeedback));
                Assert.That(viewModel.OrganisationApplicationsApproved, Is.EqualTo(_statusCounts.OrganisationApplicationsApproved));
                Assert.That(viewModel.StandardApplicationsNew, Is.EqualTo(_statusCounts.StandardApplicationsNew));
                Assert.That(viewModel.StandardApplicationsInProgress, Is.EqualTo(_statusCounts.StandardApplicationsInProgress));
                Assert.That(viewModel.StandardApplicationsHasFeedback, Is.EqualTo(_statusCounts.StandardApplicationsHasFeedback));
                Assert.That(viewModel.StandardApplicationsApproved, Is.EqualTo(_statusCounts.StandardApplicationsApproved));
            });
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
                Assert.That(viewModel.OrganisationApplicationsNew, Is.EqualTo(short.MinValue));
                Assert.That(viewModel.OrganisationApplicationsInProgress, Is.EqualTo(short.MinValue));
                Assert.That(viewModel.OrganisationApplicationsHasFeedback, Is.EqualTo(short.MinValue));
                Assert.That(viewModel.OrganisationApplicationsApproved, Is.EqualTo(short.MinValue));
                Assert.That(viewModel.StandardApplicationsNew, Is.EqualTo(short.MinValue));
                Assert.That(viewModel.StandardApplicationsInProgress, Is.EqualTo(short.MinValue));
                Assert.That(viewModel.StandardApplicationsHasFeedback, Is.EqualTo(short.MinValue));
                Assert.That(viewModel.StandardApplicationsApproved, Is.EqualTo(short.MinValue));
            });
        }
    }
}
