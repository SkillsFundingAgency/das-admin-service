using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Controllers.Apply;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.WithdrawalApplications
{
    public class WithdrawalApplicationsControllerTests
    {
        private WithdrawalApplicationController _controller;

        private Mock<IControllerSession> _controllerSession;
        private Mock<ISessionService> _sessionService;
        private Mock<IApplicationApiClient> _apiClient;
        private Mock<ILogger<WithdrawalApplicationController>> _logger;

        private Fixture _autoFixture;

        private const string WithdrawalApplication_NewApplications = "WithdrawalApplication_NewApplications";
        private const string WithdrawalApplication_InProgressApplications = "WithdrawalApplication_InProgressApplications";
        private const string WithdrawalApplication_FeedbackApplications = "WithdrawalApplication_FeedbackApplications";
        private const string WithdrawalApplication_ApprovedApplications = "WithdrawalApplication_ApprovedApplications";

        private const int DefaultItemsPerPage = 10;
        private const string DefaultSortColumn = "SubmittedDate";
        private const string DefaultSortDirection = "Desc";

        private ApplicationSummaryItem ApplicationSummaryItemNew;
        private ApplicationSummaryItem ApplicationSummaryItemInProgress;
        private ApplicationSummaryItem ApplicationSummaryItemFeedback;
        private ApplicationSummaryItem ApplicationSummaryItemApproved;
        private ApplicationSummaryItem ApplicationSummaryItemApproved2;

        [SetUp]
        public void Setup()
        {
            _autoFixture = new Fixture();

            ApplicationSummaryItemNew = _autoFixture.Build<ApplicationSummaryItem>().With(x => x.ReviewStatus, ApplicationReviewStatus.New).Create();
            ApplicationSummaryItemInProgress = _autoFixture.Build<ApplicationSummaryItem>().With(x => x.ReviewStatus, ApplicationReviewStatus.InProgress).Create();
            ApplicationSummaryItemFeedback = _autoFixture.Build<ApplicationSummaryItem>().With(x => x.ReviewStatus, ApplicationReviewStatus.HasFeedback).Create();
            ApplicationSummaryItemApproved = _autoFixture.Build<ApplicationSummaryItem>().With(x => x.ReviewStatus, ApplicationReviewStatus.Approved).Create();
            ApplicationSummaryItemApproved2 = _autoFixture.Build<ApplicationSummaryItem>().With(x => x.ReviewStatus, ApplicationReviewStatus.Approved).Create();

            _controllerSession = new Mock<IControllerSession>();
            _apiClient = new Mock<IApplicationApiClient>();
            _sessionService = new Mock<ISessionService>();
            _logger = new Mock<ILogger<WithdrawalApplicationController>>();

            SetupSessionServiceMock();
            SetupControllerSessionMock();

            _controller = new WithdrawalApplicationController(_controllerSession.Object, _apiClient.Object, _logger.Object);
        }

        [Test]
        public async Task When_RequestingWithdrawalApplicationsPage_Then_WithdrawalApplicationsViewIsReturned()
        {
            ViewResult viewResult = await _controller.WithdrawalApplications() as ViewResult;

            Assert.AreEqual("WithdrawalApplications", viewResult.ViewName);
        }

        [Test]
        public async Task When_RequestingWithdrawalApplicationsPage_Then_CorrectTabTitlesAreSet()
        {
            SetupSingleNewApplication();

            var result = await _controller.WithdrawalApplications() as ViewResult;
            var model = result.Model as ApplicationsDashboardViewModel;

            Assert.AreEqual("New", model.NewApplications.Title);
            Assert.AreEqual("In progress", model.InProgressApplications.Title);
            Assert.AreEqual("Feedback", model.FeedbackApplications.Title);
            Assert.AreEqual("Approved", model.ApprovedApplications.Title);
        }

        [Test]
        public async Task When_RequestingWithdrawalApplicationsPage_Then_ApplicationsAreMapped()
        {
            SetupMultipleApplications();

            var result = await _controller.WithdrawalApplications() as ViewResult;
            var model = result.Model as ApplicationsDashboardViewModel;

            Assert.AreEqual(1, model.NewApplications.PaginatedList.Items.Count);
            Assert.AreEqual(1, model.InProgressApplications.PaginatedList.Items.Count);
            Assert.AreEqual(1, model.FeedbackApplications.PaginatedList.Items.Count);
            Assert.AreEqual(2, model.ApprovedApplications.PaginatedList.Items.Count);
        }

        private void SetupSessionServiceMock()
        {
            _sessionService.Setup(x => x.Get<int>($"{WithdrawalApplication_NewApplications}_ItemsPerPage")).Returns(DefaultItemsPerPage);
            _sessionService.Setup(x => x.Get($"{WithdrawalApplication_NewApplications}_SortColumn")).Returns(DefaultSortColumn);
            _sessionService.Setup(x => x.Get($"{WithdrawalApplication_NewApplications}_SortDirection")).Returns(DefaultSortDirection);

            _sessionService.Setup(x => x.Get<int>($"{WithdrawalApplication_InProgressApplications}_ItemsPerPage")).Returns(DefaultItemsPerPage);
            _sessionService.Setup(x => x.Get($"{WithdrawalApplication_InProgressApplications}_SortColumn")).Returns(DefaultSortColumn);
            _sessionService.Setup(x => x.Get($"{WithdrawalApplication_InProgressApplications}_SortDirection")).Returns(DefaultSortDirection);

            _sessionService.Setup(x => x.Get<int>($"{WithdrawalApplication_FeedbackApplications}_ItemsPerPage")).Returns(DefaultItemsPerPage);
            _sessionService.Setup(x => x.Get($"{WithdrawalApplication_FeedbackApplications}_SortColumn")).Returns(DefaultSortColumn);
            _sessionService.Setup(x => x.Get($"{WithdrawalApplication_FeedbackApplications}_SortDirection")).Returns(DefaultSortDirection);

            _sessionService.Setup(x => x.Get<int>($"{WithdrawalApplication_ApprovedApplications}_ItemsPerPage")).Returns(DefaultItemsPerPage);
            _sessionService.Setup(x => x.Get($"{WithdrawalApplication_ApprovedApplications}_SortColumn")).Returns(DefaultSortColumn);
            _sessionService.Setup(x => x.Get($"{WithdrawalApplication_ApprovedApplications}_SortDirection")).Returns(DefaultSortDirection);

        }
        private void SetupControllerSessionMock()
        {
            _controllerSession.Setup(x => x.WithdrawalApplication_NewApplications).Returns(new PagingState(_sessionService.Object, WithdrawalApplication_NewApplications));
            _controllerSession.Setup(x => x.WithdrawalApplication_InProgressApplications).Returns(new PagingState(_sessionService.Object, WithdrawalApplication_InProgressApplications));
            _controllerSession.Setup(x => x.WithdrawalApplication_FeedbackApplications).Returns(new PagingState(_sessionService.Object, WithdrawalApplication_FeedbackApplications));
            _controllerSession.Setup(x => x.WithdrawalApplication_ApprovedApplications).Returns(new PagingState(_sessionService.Object, WithdrawalApplication_ApprovedApplications));
        }

        private void SetupSingleNewApplication()
        {
            _apiClient.Setup(x => x.GetWithdrawalApplications(It.Is<WithdrawalApplicationsRequest>(y => y.ReviewStatus == "New")))
                .ReturnsAsync(new PaginatedList<ApplicationSummaryItem>(new List<ApplicationSummaryItem>() {
                    ApplicationSummaryItemNew
                }, 1, 1, 10));
        }

        private void SetupMultipleApplications()
        {
            _apiClient.Setup(x => x.GetWithdrawalApplications(It.Is<WithdrawalApplicationsRequest>(y => y.ReviewStatus == "New")))
                .ReturnsAsync(new PaginatedList<ApplicationSummaryItem>(new List<ApplicationSummaryItem>() {
                    ApplicationSummaryItemNew
                }, 1, 1, 10));
            _apiClient.Setup(x => x.GetWithdrawalApplications(It.Is<WithdrawalApplicationsRequest>(y => y.ReviewStatus == "In Progress")))
                .ReturnsAsync(new PaginatedList<ApplicationSummaryItem>(new List<ApplicationSummaryItem>() {
                    ApplicationSummaryItemInProgress
                }, 1, 1, 10));
            _apiClient.Setup(x => x.GetWithdrawalApplications(It.Is<WithdrawalApplicationsRequest>(y => y.ReviewStatus == "Has Feedback")))
                .ReturnsAsync(new PaginatedList<ApplicationSummaryItem>(new List<ApplicationSummaryItem>() {
                    ApplicationSummaryItemFeedback
                }, 1, 1, 10));
            _apiClient.Setup(x => x.GetWithdrawalApplications(It.Is<WithdrawalApplicationsRequest>(y => y.ReviewStatus == "Approved")))
                .ReturnsAsync(new PaginatedList<ApplicationSummaryItem>(new List<ApplicationSummaryItem>() {
                    ApplicationSummaryItemApproved,
                    ApplicationSummaryItemApproved2
                }, 1, 1, 10));
        }
    }
}