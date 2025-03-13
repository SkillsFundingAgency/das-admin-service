using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Models.Search;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class SubmitTestsTests : SearchControllerTestsBase
    {
        [Test]
        public async Task Submit_SessionIsNull_RedirectsToIndex()
        {
            // Arrange
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns((FrameworkSearchSession)null);

            // Act
            var result = await _controller.Submit();

            // Assert
            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        public async Task Submit_SelectedResultIsNull_RedirectsToIndex()
        {
            // Arrange
            var sessionModel = new FrameworkSearchSession { SelectedResult = null };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            // Act
            var result = await _controller.Submit();

            // Assert
            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        public async Task Submit_ValidSessionAndSelectedResult_CallsAPIToCreateReprintRequest()
        {
            // Arrange
            var sessionModel = new FrameworkSearchSession
            {
                FrameworkResults = new List<FrameworkLearnerSummaryViewModel> { new FrameworkLearnerSummaryViewModel() },
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = DateTime.Now.AddYears(-20),
                SelectedResult = Guid.NewGuid(),
            };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            var mappedViewModel = new FrameworkLearnerReprintReasonViewModel { ApprenticeName = "Test User" };
            _mapperMock.Setup(m => m.Map<FrameworkLearnerReprintReasonViewModel>(sessionModel)).Returns(mappedViewModel);

            // Act
            var result = await _controller.Submit();

            // Assert
            //TODO : Waiting on #2356 to create the reprint request
            //_apiMock.Verify(s => s.SubmitFrameworkReprintRequest(), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task Submit_ValidSessionAndSelectedResult_CallsAPIToGetNextPrintRunDate(ScheduleRun scheduleRun)
        {
            // Arrange
            var sessionModel = new FrameworkSearchSession
            {
                FrameworkResults = new List<FrameworkLearnerSummaryViewModel> { new FrameworkLearnerSummaryViewModel() },
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = DateTime.Now.AddYears(-20),
                SelectedResult = Guid.NewGuid(),
            };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            var mappedViewModel = new FrameworkLearnerReprintReasonViewModel { ApprenticeName = "Test User" };
            _mapperMock.Setup(m => m.Map<FrameworkLearnerReprintReasonViewModel>(sessionModel)).Returns(mappedViewModel);
            _scheduleApiClientMock.Setup(s => s.GetNextScheduledRun((int)ScheduleType.PrintRun)).ReturnsAsync(scheduleRun);

            // Act
            var result = await _controller.Submit();

            // Assert
            _scheduleApiClientMock.Verify(s => s.GetNextScheduledRun((int)ScheduleType.PrintRun), Times.Once);
        }

        [Test]
        public async Task Submit_ValidSessionAndSelectedResult_ClearsSession()
        {
            // Arrange
            var sessionModel = new FrameworkSearchSession
            {
                FrameworkResults = new List<FrameworkLearnerSummaryViewModel> { new FrameworkLearnerSummaryViewModel() },
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = DateTime.Now.AddYears(-20),
                SelectedResult = Guid.NewGuid(),
            };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            var mappedViewModel = new FrameworkLearnerReprintReasonViewModel { ApprenticeName = "Test User" };
            _mapperMock.Setup(m => m.Map<FrameworkLearnerReprintReasonViewModel>(sessionModel)).Returns(mappedViewModel);

            // Act
            var result = await _controller.Submit();

            // Assert
            _sessionServiceMock.Verify(s => s.ClearFrameworkSearchRequest(), Times.Once);
        }

        [Test]
        public async Task Submit_ValidSessionAndSelectedResult_ReturnsViewWithCorrectData()
        {
            // Arrange
            _sessionServiceMock.Setup(s =>  s.SessionFrameworkSearch).Returns(new FrameworkSearchSession
            {
                SelectedResult = Guid.NewGuid()
            });

            // Act
            var result = await _controller.Submit();

            //Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            //TODO: Waiting on #2356
            //redirectResult.ActionName.Should().Be("Confirmation");
            //redirectResult.RouteValues["printRunDate"].Should().NotBeNull();
        }
    }
}