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

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class CertificateTests : SearchControllerTestsBase 
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
        public async Task Certificate_SessionIsNull_RedirectsToIndex()
        {
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns((FrameworkSearchSession)null);

            var result = await _controller.Certificate();

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        public async Task Certificate_SelectedResultIsNull_RedirectsToIndex()
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(1).ToList();
            var sessionModel = CreateSessionModel(results, null);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            var result = await _controller.Certificate();

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        [MoqAutoData]
        public async Task Certificate_SessionAndSelectedResultValid_CallsGetFrameworkLearner(GetFrameworkLearnerResponse certificateResult)
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(3).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>())).ReturnsAsync(certificateResult);

            await _controller.Certificate();

            _learnerDetailsApiClientMock.Verify(api => api.GetFrameworkLearner(sessionModel.SelectedResult.Value), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task Certificate_SessionAndSelectedResultValid_MapsViewModel(GetFrameworkLearnerResponse certificateResult, FrameworkLearnerViewModel certificateViewModel)
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(3).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>())).ReturnsAsync(certificateResult);
            _mapperMock.Setup(m => m.Map<FrameworkLearnerViewModel>(certificateResult)).Returns(certificateViewModel);

            await _controller.Certificate();

            _mapperMock.Verify(m => m.Map<FrameworkLearnerViewModel>(certificateResult), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task Certificate_SessionAndSelectedResultValid_ReturnsCorrectView(GetFrameworkLearnerResponse certificateResult, FrameworkLearnerViewModel certificateViewModel)
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(3).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>())).ReturnsAsync(certificateResult);
            _mapperMock.Setup(m => m.Map<FrameworkLearnerViewModel>(certificateResult)).Returns(certificateViewModel);

            var result = await _controller.Certificate();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var resultModel = viewResult.Model.Should().BeOfType<FrameworkLearnerViewModel>().Subject;
            resultModel.Should().BeEquivalentTo(certificateViewModel); // Assert model properties
        }

        [Test]
        public async Task CertificateBackAction_FrameworkResultsHasMultipleItems_UpdatesSessionAndRedirectsToMultipleResults()
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(3).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            Action<FrameworkSearchSession> capturedAction = null;
            _sessionServiceMock.Setup(s => s.UpdateFrameworkSearchRequest(It.IsAny<Action<FrameworkSearchSession>>()))
                .Callback<Action<FrameworkSearchSession>>(action => capturedAction = action);

            var result = await _controller.CertificateBackAction();

            capturedAction.Should().NotBeNull();
            capturedAction(sessionModel); 
            sessionModel.SelectedResult.Should().BeNull();

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("MultipleResults");
        }

        [Test]
        public async Task CertificateBackAction_FrameworkResultsHasOneItem_ClearSessionAndRedirectsToIndex()
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(1).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            var result = await _controller.CertificateBackAction();

            _sessionServiceMock.Verify(s => s.ClearFrameworkSearchRequest(), Times.Once);

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        public async Task CertificateBackAction_SessionIsNull_RedirectsToIndex()
        {
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns((FrameworkSearchSession)null);

            var result = await _controller.CertificateBackAction();

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        public async Task Certificate_ApiThrowsException_ReturnsErrorView()
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(1).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>())).ThrowsAsync(new Exception("API Error"));

            // Act
            Func<Task> act = async () => await _controller.Certificate();

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("API Error");
        }

        [Test]
        public async Task Certificate_MappingThrowsException_ReturnsErrorView()
        {
            var results = _fixture.CreateMany<FrameworkLearnerSummaryViewModel>(1).ToList();
            var sessionModel = CreateSessionModel(results, results[0].Id);
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>())).ReturnsAsync(_fixture.Create<GetFrameworkLearnerResponse>());
            _mapperMock.Setup(m => m.Map<FrameworkLearnerViewModel>(It.IsAny<GetFrameworkLearnerResponse>())).Throws(new Exception("Mapping Error"));

            // Act
            Func<Task> act = async () => await _controller.Certificate();

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Mapping Error");
        }
    }
}