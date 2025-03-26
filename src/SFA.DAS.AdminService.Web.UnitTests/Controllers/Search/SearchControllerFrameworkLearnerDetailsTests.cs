using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using SFA.DAS.AdminService.Web.Models.Search;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.Testing.AutoFixture;
using AutoFixture.NUnit3;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class FrameworkLearnerDetailsTests : SearchControllerTestsBase
    {
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        private FrameworkSearchSession CreateSessionModel(List<FrameworkLearnerSummaryViewModel> results, Guid? selectedResult)
        {
            return new FrameworkSearchSession
            {
                FirstName = "First",
                LastName = "",
                DateOfBirth = DateTime.Now.AddYears(-28),
                FrameworkResults = results,
                SelectedFrameworkLearnerId = selectedResult
            };
        }

        [Test]
        public async Task FrameworkLearnerDetails_SessionIsNull_RedirectsToIndex()
        {
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns((FrameworkSearchSession)null);

            var result = await _controller.FrameworkLearnerDetails();

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        public async Task FrameworkLearnerDetails_SelectedResultIsNull_RedirectsToIndex()
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(1).ToList();
            var sessionModel = CreateSessionModel(results, null);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            var result = await _controller.FrameworkLearnerDetails();

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        [MoqAutoData]
        public async Task FrameworkLearnerDetails_SessionAndSelectedResultValid_CallsGetFrameworkLearner(
            GetFrameworkLearnerResponse frameworkLearnerDetailsResponse,
            FrameworkLearnerDetailsViewModel frameworkLearnerDetailsViewModel)
        {
            var sessionModel = new FrameworkSearchSession { SelectedFrameworkLearnerId = Guid.NewGuid() };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>(), false)).ReturnsAsync(frameworkLearnerDetailsResponse);
            _mapperMock.Setup(m => m.Map<FrameworkLearnerDetailsViewModel>(frameworkLearnerDetailsResponse)).Returns(frameworkLearnerDetailsViewModel);

            await _controller.FrameworkLearnerDetails();

            _learnerDetailsApiClientMock.Verify(api => api.GetFrameworkLearner(sessionModel.SelectedFrameworkLearnerId.Value, false), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task FrameworkLearnerDetails_SessionAndSelectedResultValid_MapsViewModel(GetFrameworkLearnerResponse frameworkLearnerDetailsResponse, FrameworkLearnerDetailsViewModel frameworkLearnerDetailsViewModel)
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(3).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>(), false)).ReturnsAsync(frameworkLearnerDetailsResponse);
            _mapperMock.Setup(m => m.Map<FrameworkLearnerDetailsViewModel>(frameworkLearnerDetailsResponse)).Returns(frameworkLearnerDetailsViewModel);

            await _controller.FrameworkLearnerDetails();

            _mapperMock.Verify(m => m.Map<FrameworkLearnerDetailsViewModel>(frameworkLearnerDetailsResponse), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task FrameworkLearnerDetails_SessionAndSelectedResultValid_ReturnsCorrectView(GetFrameworkLearnerResponse frameworkLearnerDetailsResponse, FrameworkLearnerDetailsViewModel frameworklearnerDetailsViewModel)
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(3).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>(), false)).ReturnsAsync(frameworkLearnerDetailsResponse);
            _mapperMock.Setup(m => m.Map<FrameworkLearnerDetailsViewModel>(frameworkLearnerDetailsResponse)).Returns(frameworklearnerDetailsViewModel);

            var result = await _controller.FrameworkLearnerDetails();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var resultModel = viewResult.Model.Should().BeOfType<FrameworkLearnerDetailsViewModel>().Subject;
            resultModel.Should().BeEquivalentTo(frameworklearnerDetailsViewModel);
        }

        [Test]
        public async Task FrameworkLearnerDetailsBackAction_FrameworkResultsHasMultipleItems_UpdatesSessionAndRedirectsToMultipleResults()
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(3).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            Action<FrameworkSearchSession> capturedAction = null;
            _sessionServiceMock.Setup(s => s.UpdateFrameworkSearchRequest(It.IsAny<Action<FrameworkSearchSession>>()))
                .Callback<Action<FrameworkSearchSession>>(action => capturedAction = action);

            var result = await _controller.FrameworkLearnerDetailsBackAction();

            capturedAction.Should().NotBeNull();
            capturedAction(sessionModel);
            sessionModel.SelectedFrameworkLearnerId.Should().BeNull();

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("MultipleResults");
        }

        [Test]
        public async Task FrameworkLearnerDetailsBackAction_FrameworkResultsHasOneItem_ClearSessionAndRedirectsToIndex()
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(1).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            var result = await _controller.FrameworkLearnerDetailsBackAction();

            _sessionServiceMock.Verify(s => s.ClearFrameworkSearchRequest(), Times.Once);

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        public async Task FrameworkLearnerDetailsBackAction_SessionIsNull_RedirectsToIndex()
        {
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns((FrameworkSearchSession)null);

            var result = await _controller.FrameworkLearnerDetailsBackAction();

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        public async Task FrameworkLearnerDetails_ApiThrowsException_ReturnsErrorView()
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(1).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>(), false)).ThrowsAsync(new Exception("API Error"));

            // Act
            Func<Task> act = async () => await _controller.FrameworkLearnerDetails();

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("API Error");
        }

        [Test]
        public async Task FrameworkLearnerDetails_MappingThrowsException_ReturnsErrorView()
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(1).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>(), false)).ReturnsAsync(_fixture.Create<GetFrameworkLearnerResponse>());
            _mapperMock.Setup(m => m.Map<FrameworkLearnerDetailsViewModel>(It.IsAny<GetFrameworkLearnerResponse>())).Throws(new Exception("Mapping Error"));

            // Act
            Func<Task> act = async () => await _controller.FrameworkLearnerDetails();

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Mapping Error");
        }

       [TestCase("d2719b2e-4f5b-4c5e-9a2e-2a4f5b4c5e9a", true)]
       [TestCase("d2719b2e-4f5b-4c5e-9a2e-2a4f5b4c5e9a", false)]
        public async Task FrameworkLearnerDetails_ShouldPassCorrectParametersToGetFrameworkLearner(string frameworkLearnerId, bool allLogsValue)
        {
            // Arrange
            var frameworkLearnerGuid = new Guid(frameworkLearnerId);
            var sessionModel = new FrameworkSearchSession { SelectedFrameworkLearnerId = frameworkLearnerGuid };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _mapperMock.Setup(m => m.Map<FrameworkLearnerDetailsViewModel>(It.IsAny<GetFrameworkLearnerResponse>())).Returns(new FrameworkLearnerDetailsViewModel());

            var learnerDetails = new GetFrameworkLearnerResponse();
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(frameworkLearnerGuid, allLogsValue)).ReturnsAsync(learnerDetails);

            // Act
            var result = await _controller.FrameworkLearnerDetails(allLogs: allLogsValue);

            // Assert
            _learnerDetailsApiClientMock.Verify(api => api.GetFrameworkLearner(frameworkLearnerGuid, allLogsValue), Times.Once);
        }

        [Test]
        [InlineAutoData("d2719b2e-4f5b-4c5e-9a2e-2a4f5b4c5e9a", 1, true)]
        [InlineAutoData("d2719b2e-4f5b-4c5e-9a2e-2a4f5b4c5e9a", 1, false)]
        public async Task FrameworkLearnerDetails_FromBatchSearch_ShouldPassCorrectParametersToGetFrameworkLearner(
            string frameworkLearnerId,
            int batchNumber,
            bool allLogs,
            GetFrameworkLearnerResponse frameworkLearnerDetailsResponse,
            FrameworkLearnerDetailsViewModel frameworkLearnerDetailsViewModel)
        {
            var id = new Guid(frameworkLearnerId);
            
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(id, allLogs)).ReturnsAsync(frameworkLearnerDetailsResponse);
            _mapperMock.Setup(m => m.Map<FrameworkLearnerDetailsViewModel>(frameworkLearnerDetailsResponse)).Returns(frameworkLearnerDetailsViewModel);

            await _controller.FrameworkLearnerDetails(id, batchNumber, allLogs);

            _learnerDetailsApiClientMock.Verify(api => api.GetFrameworkLearner(id, allLogs), Times.Once);
        }

        [Test]
        [InlineAutoData("d2719b2e-4f5b-4c5e-9a2e-2a4f5b4c5e9a", 1, true)]
        [InlineAutoData("d2719b2e-4f5b-4c5e-9a2e-2a4f5b4c5e9a", 1, false)]
        public async Task FrameworkLearnerDetails_FromBatchSearch_ShouldReturnCorrectView(
            string frameworkLearnerId,
            int batchNumber,
            bool allLogs,
            GetFrameworkLearnerResponse frameworkLearnerDetailsResponse,
            FrameworkLearnerDetailsViewModel frameworkLearnerDetailsViewModel)
        {
            var id = new Guid(frameworkLearnerId);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(id, allLogs)).ReturnsAsync(frameworkLearnerDetailsResponse);
            _mapperMock.Setup(m => m.Map<FrameworkLearnerDetailsViewModel>(frameworkLearnerDetailsResponse)).Returns(frameworkLearnerDetailsViewModel);

            var result = await _controller.FrameworkLearnerDetails(id, batchNumber, allLogs);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var resultModel = viewResult.Model.Should().BeOfType<FrameworkLearnerDetailsViewModel>().Subject;
            resultModel.BatchNumber.Should().Be(batchNumber);

        } 
    }
}