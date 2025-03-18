using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Models.Search;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Threading.Tasks;
using System;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class SearchControllerResultsStandardTests : SearchControllerTestsBase
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        public async Task Results_StandardSearchWithInvalidInput_RedirectsToIndexWithErrorMessage(string searchText)
        {
            // Arrange
            SearchInputViewModel vm = new SearchInputViewModel
            {
                SearchString = searchText,
                SearchType = SearchTypes.Standards,
                FirstName = "first-should-be-removed",
                LastName = "last-should-be-removed",
                Day = "day-should-be-removed",
                Month = "day-should-be-removed",
                Year = "year-should-be-removed",
            };
            _controller.ModelState.AddModelError("SearchString", "Enter name or number");

            // Act
            var result = await _controller.Results(vm);

            // Assert
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");

            _controller.ModelState.IsValid.Should().BeFalse();

            redirectResult.RouteValues["SearchType"].Should().Be(SearchTypes.Standards);
            redirectResult.RouteValues["SearchString"].Should().Be(vm.SearchString);
            redirectResult.RouteValues["FirstName"].Should().BeNull();
            redirectResult.RouteValues["LastName"].Should().BeNull();
            redirectResult.RouteValues["Day"].Should().BeNull();
            redirectResult.RouteValues["Month"].Should().BeNull();
            redirectResult.RouteValues["Year"].Should().BeNull();
        }

        [Test]
        public async Task Results_StandardsSearch_ValidSearchString_ReturnsCorrectResultsView()
        {
            // Arrange
            var vm = new SearchInputViewModel { SearchType = SearchTypes.Standards, SearchString = "valid search string" };
            var searchResults = new StaffSearchResult { EndpointAssessorOrganisationId = "123" };
            var organisation = new EpaOrganisation { Name = "Test Org" };

            _staffSearchApiClientMock.Setup(x => x.Search(vm.SearchString, 1)).ReturnsAsync(searchResults);
            _registerApiClientMock.Setup(x => x.GetEpaOrganisation(searchResults.EndpointAssessorOrganisationId)).ReturnsAsync(organisation);

            // Act
            var result = await _controller.Results(vm);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNullOrEmpty();

            var model = viewResult.Model.Should().BeOfType<SearchResultsViewModel>().Subject;
            model.OrganisationName.Should().Be("Test Org");
            model.StaffSearchResult.Should().Be(searchResults);
            model.SearchString.Should().Be("valid search string");
            model.Page.Should().Be(1);

            _staffSearchApiClientMock.Verify(x => x.Search(vm.SearchString, 1), Times.Once);
            _registerApiClientMock.Verify(x => x.GetEpaOrganisation(searchResults.EndpointAssessorOrganisationId), Times.Once);
        }

        [Test]
        public async Task Results_StandardsSearch_StaffSearchClientThrowsException_ExceptionThrown()
        {
            // Arrange
            var vm = new SearchInputViewModel { SearchType = SearchTypes.Standards, SearchString = "valid search string" };
            _staffSearchApiClientMock.Setup(x => x.Search(vm.SearchString, 1)).ThrowsAsync(new System.Exception("API Error"));

            // Act
            Func<Task> act = async () => await _controller.Results(vm);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("API Error");
        }

        [Test]
        public async Task Results_StandardsSearch_RegisterClientThrowsException_ExceptionThrown()
        {
            // Arrange
            var vm = new SearchInputViewModel { SearchType = SearchTypes.Standards, SearchString = "valid search string" };
            var searchResults = new StaffSearchResult { EndpointAssessorOrganisationId = "123" };
            _staffSearchApiClientMock.Setup(x => x.Search(vm.SearchString, 1)).ReturnsAsync(searchResults);
            _registerApiClientMock.Setup(x => x.GetEpaOrganisation(searchResults.EndpointAssessorOrganisationId)).ThrowsAsync(new System.Exception("API Error"));

            // Act
            Func<Task> act = async () => await _controller.Results(vm);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("API Error");
        }
    }
}
