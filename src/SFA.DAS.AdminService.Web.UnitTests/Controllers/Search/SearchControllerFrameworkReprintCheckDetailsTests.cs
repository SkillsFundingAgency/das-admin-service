﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Models.Search;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class SearchControllerFrameworkReprintCheckDetailsTests : SearchControllerTestsBase 
    {
        [Test]
        public async Task Check_SessionIsNull_RedirectsToIndex()
        {
            // Arrange
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns((FrameworkSearchSession)null);

            // Act
            var result = await _controller.FrameworkReprintCheckDetails();

            // Assert
            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        public async Task Check_SelectedResultIsNull_RedirectsToIndex()
        {
            // Arrange
            var sessionModel = new FrameworkSearchSession { SelectedFrameworkLearnerId = null };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            // Act
            var result = await _controller.FrameworkReprintCheckDetails();

            // Assert
            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        [MoqAutoData]
        public async Task Check_SessionAndSelectedResultValid_CallsGetFrameworkLearner(GetFrameworkLearnerResponse certificateResult)
        {
            var sessionModel = new FrameworkSearchSession { SelectedFrameworkLearnerId = Guid.NewGuid() };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>(), false)).ReturnsAsync(certificateResult);

            await _controller.FrameworkReprintCheckDetails();

            _learnerDetailsApiClientMock.Verify(api => api.GetFrameworkLearner(sessionModel.SelectedFrameworkLearnerId.Value, false), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task Check_SessionAndSelectedResultValid_MapsViewModel(
            GetFrameworkLearnerResponse certificateResult, 
            FrameworkLearnerDetailsViewModel certificateViewModel)
        {
            var sessionModel = new FrameworkSearchSession { SelectedFrameworkLearnerId = Guid.NewGuid() };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>(), false)).ReturnsAsync(certificateResult);
            _mapperMock.Setup(m => m.Map<FrameworkLearnerDetailsViewModel>(certificateResult)).Returns(certificateViewModel);

            await _controller.FrameworkLearnerDetails();

            _mapperMock.Verify(m => m.Map<FrameworkLearnerDetailsViewModel>(certificateResult), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task Check_SessionAndSelectedResultValid_ReturnsViewWithCorrectModel(
            GetFrameworkLearnerResponse certificateResult, 
            FrameworkLearnerDetailsViewModel certificateViewModel)
        {
            var sessionModel = new FrameworkSearchSession { SelectedFrameworkLearnerId = Guid.NewGuid() };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(api => api.GetFrameworkLearner(It.IsAny<Guid>(), false)).ReturnsAsync(certificateResult);
            _mapperMock.Setup(m => m.Map<FrameworkLearnerDetailsViewModel>(certificateResult)).Returns(certificateViewModel);

            var result = await _controller.FrameworkLearnerDetails();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var resultModel = viewResult.Model.Should().BeOfType<FrameworkLearnerDetailsViewModel>().Subject;
            resultModel.Should().BeEquivalentTo(certificateViewModel); 
        }
    }
}