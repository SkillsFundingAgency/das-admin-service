﻿using FluentAssertions;
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
                SelectedResult = selectedResult
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
        public async Task FrameworkLearnerDetails_SessionAndSelectedResultValid_CallsGetFrameworkLearner(GetFrameworkLearnerResponse frameworkLearnerDetailsResponse)
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(3).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>())).ReturnsAsync(frameworkLearnerDetailsResponse);

            await _controller.FrameworkLearnerDetails();

            _learnerDetailsApiClientMock.Verify(api => api.GetFrameworkLearner(sessionModel.SelectedResult.Value), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task FrameworkLearnerDetails_SessionAndSelectedResultValid_MapsViewModel(GetFrameworkLearnerResponse frameworkLearnerDetailsResponse, FrameworkLearnerDetailsViewModel frameworkLearnerDetailsViewModel)
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(3).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>())).ReturnsAsync(frameworkLearnerDetailsResponse);
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
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>())).ReturnsAsync(frameworkLearnerDetailsResponse);
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
            sessionModel.SelectedResult.Should().BeNull();

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
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>())).ThrowsAsync(new Exception("API Error"));

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
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>())).ReturnsAsync(_fixture.Create<GetFrameworkLearnerResponse>());
            _mapperMock.Setup(m => m.Map<FrameworkLearnerDetailsViewModel>(It.IsAny<GetFrameworkLearnerResponse>())).Throws(new Exception("Mapping Error"));

            // Act
            Func<Task> act = async () => await _controller.FrameworkLearnerDetails();

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Mapping Error");
        }
    }
}