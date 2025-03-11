using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AdminService.Web.Models.Search;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class SearchControllerResultsFrameworkSearchTests : SearchControllerTestsBase
    {
        [Test]
        public async Task Results_FrameworkSearchWithInvalidInput_RedirectsToIndexWithCorrectError()
        {
            //Arrange
            SearchInputViewModel vm = new SearchInputViewModel { SearchType = SearchTypes.Frameworks };
            _controller.ModelState.AddModelError("Error", "Error message");

            //Act
            var result = await _controller.Results(vm);

            //Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");

            _controller.ModelState.IsValid.Should().BeFalse();
            redirectResult.RouteValues["SearchString"].Should().BeNull();
        }

        [Test]
        public async Task Results_FrameworkSearchWithValidInput_NoResults_RedirectsToNoResultsAndClearsSession()
        {
            // Arrange
            var vm = CreateValidSearchInputViewModel();
            var searchQuery = CreateValidFrameworkLearnerSearchRequest();
            SetupValidFrameworkSearchMapping(vm, searchQuery);
            SetupFrameworkSearchApiClient(searchQuery, new List<FrameworkLearnerSearchResponse>());

            // Act
            var result = await _controller.Results(vm);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("NoResults");
            _sessionServiceMock.Verify(s => s.ClearFrameworkSearchRequest(), Times.Once);
            redirectResult.RouteValues["FirstName"].Should().Be(searchQuery.FirstName);
            redirectResult.RouteValues["LastName"].Should().Be(searchQuery.LastName);
            redirectResult.RouteValues["DateOfBirth"].Should().Be(searchQuery.DateOfBirth);
        }

        [Test]
        public async Task Results_FrameworkSearchWithValidInput_MultipleResults_RedirectsToMultipleResultsAndSetsSession()
        {
            // Arrange
            var vm = CreateValidSearchInputViewModel();
            var searchQuery = CreateValidFrameworkLearnerSearchRequest();
            var frameworkResults = CreateFrameworkSearchResponses(2);
            var mappedResults = CreateMappedFrameworkResults(2);
            SetupValidFrameworkSearchMapping(vm, searchQuery);
            SetupFrameworkSearchApiClient(searchQuery, frameworkResults);
            SetupFrameworkSearchResultsMapping(frameworkResults, mappedResults);

            // Act
            var result = await _controller.Results(vm);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("MultipleResults");
            _sessionServiceMock.VerifySet(s => s.SessionFrameworkSearch = It.IsAny<FrameworkSearchSession>(), Times.Once);
        }

        [Test]
        public async Task Results_FrameworkSearchWithValidInput_OneResult_RedirectsToCertificateAndSetsSession()
        {
            // Arrange
            var vm = CreateValidSearchInputViewModel();
            var searchQuery = CreateValidFrameworkLearnerSearchRequest();
            var frameworkResults = CreateFrameworkSearchResponses(1);
            var mappedResults = CreateMappedFrameworkResults(1);
            SetupValidFrameworkSearchMapping(vm, searchQuery);
            SetupFrameworkSearchApiClient(searchQuery, frameworkResults);
            SetupFrameworkSearchResultsMapping(frameworkResults, mappedResults);

            // Act
            var result = await _controller.Results(vm);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("FrameworkLearnerDetails");
            _sessionServiceMock.VerifySet(s => s.SessionFrameworkSearch = It.IsAny<FrameworkSearchSession>(), Times.Once);

        }

        [TestCaseSource(nameof(SearchFrameworksInvalidInput))]
        public async Task Results_FrameworkSearchWithInvalidInput_RedirectsToIndexWithCorrectError(SearchInputViewModel vm)
        {
            //Arrange 
            _controller.ModelState.AddModelError("Error", "Error message");

            //Act
            var result = await _controller.Results(vm);

            //Assert
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("Index");

            _controller.ModelState.IsValid.Should().BeFalse();

            redirectResult.RouteValues["SearchType"].Should().Be(SearchTypes.Frameworks);
            redirectResult.RouteValues["SearchString"].Should().BeNull();
            redirectResult.RouteValues["FirstName"].Should().Be(vm.FirstName);
            redirectResult.RouteValues["LastName"].Should().Be(vm.LastName);
            redirectResult.RouteValues["Day"].Should().Be(vm.Day);
            redirectResult.RouteValues["Month"].Should().Be(vm.Month);
            redirectResult.RouteValues["Year"].Should().Be(vm.Year);
        }

        private static IEnumerable<SearchInputViewModel> SearchFrameworksInvalidInput()
        {
            yield return new SearchInputViewModel 
                { SearchType = SearchTypes.Frameworks, SearchString = "will-be-removed", FirstName = null, LastName="last", Day="1", Month="1", Year="2000" };
            yield return new SearchInputViewModel 
                { SearchType = SearchTypes.Frameworks, SearchString = "will-be-removed", FirstName = "first", LastName=null, Day="1", Month="1", Year="2000" };
            yield return new SearchInputViewModel 
                { SearchType = SearchTypes.Frameworks, SearchString = "will-be-removed", FirstName = "first", LastName="last", Day=null, Month="1", Year="2000" }; 
            yield return new SearchInputViewModel 
                { SearchType = SearchTypes.Frameworks, SearchString = "will-be-removed", FirstName = "first", LastName="last", Day="1", Month=null, Year="2000" };
            yield return new SearchInputViewModel 
                { SearchType = SearchTypes.Frameworks, SearchString = "will-be-removed", FirstName = "first", LastName="last", Day="1", Month="1", Year=null };
        }
    }
}