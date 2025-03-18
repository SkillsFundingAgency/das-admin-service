using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Models;
using System.Security.Claims;
using FluentAssertions.Execution;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Threading.Tasks;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class SearchControllerTests
    {
        private SearchController _controller;
        private Mock<ILearnerDetailsApiClient> _learnerDetailsApiClientMock;
        private Mock<IRegisterApiClient> _registerApiClientMock;
        private Mock<IStaffSearchApiClient> _staffSearchApiClientMock;

        [SetUp]
        public void Setup()
        {
            _learnerDetailsApiClientMock = new Mock<ILearnerDetailsApiClient>();
            _registerApiClientMock = new Mock<IRegisterApiClient>();
            _staffSearchApiClientMock = new Mock<IStaffSearchApiClient>();
            _controller = new SearchController(_learnerDetailsApiClientMock.Object, _registerApiClientMock.Object, _staffSearchApiClientMock.Object);
        }

        public void Then_Index_Returns_ViewModel()
        {
            //act
            var result = _controller.Index();

            //assert
            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();
                var resultModel = result.Should().BeOfType<ViewResult>().
                    Which.Model.Should().BeOfType<SearchInputViewModel>().Which;
            });
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task Results_StandardSearchWithInvalidInput_RedirectsToIndexWithErrorMessage(string searchString)
        {
            //Arrange 
            SearchInputViewModel vm = new SearchInputViewModel
            {
                SearchString = searchString,
                SearchType = SearchTypes.Standards,
                FirstName = "first-should-be-removed",
                LastName = "last-should-be-removed",
                Day = "day-should-be-removed",
                Month = "day-should-be-removed",
                Year = "year-should-be-removed",
            };
            _controller.ModelState.AddModelError("SearchString", "Enter name or number");

            //Act
            var result = await _controller.Results(vm);

            //Assert
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
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
        public async Task Results_StandardsSearch_ValidSearchString_ReturnsResultsView()
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
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().BeNullOrEmpty(); 

            var model = viewResult.Model as SearchResultsViewModel;
            model.Should().NotBeNull();
            model.OrganisationName.Should().Be("Test Org"); 
            model.StaffSearchResult.Should().Be(searchResults); 
            model.SearchString.Should().Be("valid search string"); 
            model.Page.Should().Be(1); 

            _staffSearchApiClientMock.Verify(x => x.Search(vm.SearchString, 1), Times.Once); 
            _registerApiClientMock.Verify(x => x.GetEpaOrganisation(searchResults.EndpointAssessorOrganisationId), Times.Once); 
        }


        [TestCaseSource(nameof(SearchFrameworksInvalidInput))]
        public async Task Results_FrameworkSearchWithInvalidInput_RedirectsToIndexWithErrorMessage(SearchInputViewModel vm)
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

        [Test]
        public async Task Results_FrameworksSearch_ValidInput_RedirectsToIndex()
        {
            // Arrange
            var vm = new SearchInputViewModel
            {
                SearchType = SearchTypes.Frameworks,
                FirstName = "Test", 
                LastName = "User", 
                Day = "1",      
                Month = "1",     
                Year = "2000"    
            };

            // Act
            var result = await _controller.Results(vm);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("Index"); 
            redirectResult.ControllerName.Should().BeNull(); 

            _controller.ModelState.IsValid.Should().BeTrue(); 
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
