using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using System.Threading.Tasks;
using System;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class SearchControllerLearnerDetailsTests : SearchControllerTestsBase
    {
        [Test]
        public async Task LearnerDetails_ValidInput_ReturnsCorrectViewModel()
        {
            // Arrange
            int stdCode = 123;
            long uln = 456;
            string searchString = "test search";
            int page = 1;
            bool allLogs = false;
            int? batchNumber = 789;

            var learnerDetails = new LearnerDetailResult { Uln = uln, StandardCode = stdCode };
            _learnerDetailsApiClientMock.Setup(x => x.GetLearnerDetail(stdCode, uln, allLogs)).ReturnsAsync(learnerDetails);

            // Act
            var result = await _controller.LearnerDetails(stdCode, uln, searchString, page, allLogs, batchNumber);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<StandardLearnerDetailsViewModel>().Subject;

            model.Learner.Should().Be(learnerDetails);
            model.SearchString.Should().Be(searchString);
            model.Page.Should().Be(page);
            model.ShowDetail.Should().Be(!allLogs);
            model.BatchNumber.Should().Be(batchNumber);
        }

        [Test]
        public async Task LearnerDetails_LearnerDetailsApiClientThrowsException_APIErrorThrown()
        {
            // Arrange
            int stdCode = 123;
            long uln = 456;
            bool allLogs = false;

            _learnerDetailsApiClientMock.Setup(x => x.GetLearnerDetail(stdCode, uln, allLogs)).ThrowsAsync(new Exception("API Error"));

            // Act
            Func<Task> act = async () => await _controller.LearnerDetails(stdCode, uln, "test search", 1, allLogs, null);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("API Error");
        }

        [Test]
        public async Task LearnerDetails_ReturnsCorrectView()
        {
            // Arrange
            int stdCode = 123;
            long uln = 456;
            bool allLogs = false;

            var learnerDetails = new LearnerDetailResult { Uln = uln, StandardCode = stdCode };
            _learnerDetailsApiClientMock.Setup(x => x.GetLearnerDetail(stdCode, uln, allLogs)).ReturnsAsync(learnerDetails);

            // Act
            var result = await _controller.LearnerDetails(stdCode, uln, "test search", 1, allLogs, null);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNullOrEmpty();
        }
    }
}
