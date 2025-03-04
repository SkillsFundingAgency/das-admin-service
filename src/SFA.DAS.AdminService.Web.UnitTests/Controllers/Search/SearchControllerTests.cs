using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Threading.Tasks;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models;
using AutoMapper;
using SFA.DAS.AdminService.Web.Infrastructure.FrameworkSearch;
using System;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AdminService.Web.Models.Search;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using FizzWare.NBuilder;
using System.Linq;
using SFA.DAS.AdminService.Web.Infrastructure;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class SearchControllerTests
    {
        private SearchController _controller;
        private Mock<ILearnerDetailsApiClient> _learnerDetailsApiClientMock;
        private Mock<IRegisterApiClient> _registerApiClientMock;
        private Mock<IStaffSearchApiClient> _staffSearchApiClientMock;
        private Mock<IFrameworkSearchSessionService> _sessionServiceMock;
        private Mock<IFrameworkSearchApiClient> _frameworkSearchApiClient;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _learnerDetailsApiClientMock = new Mock<ILearnerDetailsApiClient>();
            _registerApiClientMock = new Mock<IRegisterApiClient>();
            _staffSearchApiClientMock = new Mock<IStaffSearchApiClient>();
             _sessionServiceMock = new Mock<IFrameworkSearchSessionService>();
            _frameworkSearchApiClient = new Mock<IFrameworkSearchApiClient>();
            _mapperMock = new Mock<IMapper>();
            _controller = new SearchController(_learnerDetailsApiClientMock.Object, _registerApiClientMock.Object, _staffSearchApiClientMock.Object,
                _sessionServiceMock.Object, _frameworkSearchApiClient.Object, _mapperMock.Object);
        }

        [TestCase]
        public void Index_ReturnsViewResult_WithNewViewModel_WhenVmIsNull()
        {
            // Arrange
            SearchInputViewModel vm = null;

            // Act
            var result = _controller.Index(vm);

            // Assert
            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();
                var resultModel = result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<SearchInputViewModel>().Which;
                resultModel.Should().NotBeNull();
            });
        }

        [TestCase]
        public void Index_ReturnsViewResult_WithViewModel_WhenVmIsNotNullAndPopulated()
        {
            // Arrange
            var vm = new SearchInputViewModel { FirstName = "John", LastName = "Doe" };
            _sessionServiceMock.Setup(s => s.UpdateFrameworkSearchRequest(It.IsAny<Action<FrameworkSearchSessionData>>()));

            // Act
            var result = _controller.Index(vm);

            // Assert
            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();
                var resultModel = result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<SearchInputViewModel>().Which;
                resultModel.FirstName.Should().Be(vm.FirstName);
                resultModel.LastName.Should().Be(vm.LastName);
            });
        }

        [TestCase]
        public void Index_ReturnsViewResult_WithViewModel()
        {
            // Act
            var result = _controller.Index(null);

            // Assert
            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();
                var resultModel = result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<SearchInputViewModel>().Which;
            });
        }

        [Test]
        public void Index_UpdatesSessionAndReturnsViewResult_WithNewViewModel_WhenVmIsNotNullAndSessionFrameworkSearchIsNotNull()
        {
            // Arrange
            var existingFrameworkSearch = new FrameworkSearchSessionData { FirstName = "Jane", LastName = "Doe" };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(existingFrameworkSearch);

            var newVm = new SearchInputViewModel { FirstName = "John", LastName = "Smith", Day = "01", Month = "01", Year = "2000" };
            _sessionServiceMock.Setup(s => s.UpdateFrameworkSearchRequest(It.IsAny<Action<FrameworkSearchSessionData>>()))
                .Callback<Action<FrameworkSearchSessionData>>(updateAction =>
                {
                    var frameworkSearch = new FrameworkSearchSessionData();
                    updateAction(frameworkSearch);
                    frameworkSearch.FirstName.Should().Be(newVm.FirstName);
                    frameworkSearch.LastName.Should().Be(newVm.LastName);
                    frameworkSearch.DateOfBirth.Should().Be(new DateTime(2000, 1, 1));
                });

            // Act
            var result = _controller.Index(newVm);

            // Assert
            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();
                var resultModel = result.Should().BeOfType<ViewResult>()
                    .Which.Model.Should().BeOfType<SearchInputViewModel>().Which;
                resultModel.FirstName.Should().Be(newVm.FirstName);
                resultModel.LastName.Should().Be(newVm.LastName);
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

        [Test][MoqAutoData]
        public async Task Results_FrameworksSearch_ValidInput_WithMultipleResults_RedirectsToMultipleResultsView(
            List<FrameworkSearchResult> searchResults, List<FrameworkCertificateSearchResultsViewModel> searchResultsViewModel)
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
            var searchQuery = new FrameworkSearchQuery
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                DateOfBirth = new DateTime(2000, 1, 1)
            };

            _mapperMock.Setup(m => m.Map<FrameworkSearchQuery>(vm)).Returns(searchQuery);
            _frameworkSearchApiClient.Setup(c => c.SearchFrameworks(searchQuery)).ReturnsAsync(searchResults);
            _mapperMock.Setup(m => m.Map<List<FrameworkCertificateSearchResultsViewModel>>(searchResults)).Returns(searchResultsViewModel);
            // Act
            var result = await _controller.Results(vm);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("MultipleResults");
            redirectResult.ControllerName.Should().BeNull();

            _controller.ModelState.IsValid.Should().BeTrue();
        }

        [Test]
        [MoqAutoData]
        public async Task Results_FrameworksSearch_ValidInput_WithMultipleResults_ShouldSetSessionObject(
            SearchInputViewModel vm, 
            FrameworkSearchQuery searchQuery)
        {
            // Arrange
            vm.SearchType = SearchTypes.Frameworks;

            var searchResults = Builder<FrameworkSearchResult>.CreateListOfSize(3)
                .All()
                .Build()
                .ToList();

            var searchResultsViewModel = Builder<FrameworkCertificateSummaryViewModel>.CreateListOfSize(3)
                .All()
                .Build()
                .ToList();

            _mapperMock.Setup(m => m.Map<FrameworkSearchQuery>(vm)).Returns(searchQuery);
            _frameworkSearchApiClient.Setup(c => c.SearchFrameworks(searchQuery)).ReturnsAsync(searchResults);
            _mapperMock.Setup(m => m.Map<List<FrameworkCertificateSummaryViewModel>>(searchResults)).Returns(searchResultsViewModel);

            FrameworkSearchSessionData capturedFrameworkSearch = null;
            _sessionServiceMock.SetupSet(s => s.SessionFrameworkSearch = It.IsAny<FrameworkSearchSessionData>())
                .Callback((FrameworkSearchSessionData value) => capturedFrameworkSearch = value);

            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(new FrameworkSearchSessionData());

            //Act
            var result = await _controller.Results(vm);

            // Assert
            _sessionServiceMock.VerifySet(s => s.SessionFrameworkSearch = It.IsAny<FrameworkSearchSessionData>(), Times.Once);

            capturedFrameworkSearch.Should().NotBeNull();
            capturedFrameworkSearch.FirstName.Should().Be(searchQuery.FirstName);
            capturedFrameworkSearch.LastName.Should().Be(searchQuery.LastName);
            capturedFrameworkSearch.DateOfBirth.Should().Be(searchQuery.DateOfBirth);
            capturedFrameworkSearch.FrameworkResults.Should().BeEquivalentTo(searchResultsViewModel);
            capturedFrameworkSearch.SelectedResult.Should().BeNull();
        }

        [Test]
        [MoqAutoData]
        public async Task Results_FrameworksSearch_ValidInput_NoResults_ShouldRemoveSessionObject(
            SearchInputViewModel vm, 
            FrameworkSearchQuery searchQuery)
        {
            // Arrange
            vm.SearchType = SearchTypes.Frameworks;
            List<FrameworkSearchResult> searchResults = new List<FrameworkSearchResult>();
            List<FrameworkCertificateSummaryViewModel> searchResultsViewModel = new List<FrameworkCertificateSummaryViewModel>();

            _mapperMock.Setup(m => m.Map<FrameworkSearchQuery>(vm)).Returns(searchQuery);
            _frameworkSearchApiClient.Setup(c => c.SearchFrameworks(searchQuery)).ReturnsAsync(searchResults);
            _mapperMock.Setup(m => m.Map<List<FrameworkCertificateSummaryViewModel>>(searchResults)).Returns(searchResultsViewModel);

            //Act
            var result = await _controller.Results(vm);

            // Assert
            _sessionServiceMock.Verify(s => s.ClearFrameworkSearchRequest(), Times.Once);
        }

        [Test]
        public async Task Results_FrameworksSearch_ValidInput_WithoutResults_RedirectsToNoResultsView()
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
            var searchQuery = new FrameworkSearchQuery 
            { 
                FirstName = vm.FirstName, 
                LastName = vm.LastName, 
                DateOfBirth = new DateTime(2000, 1, 1) 
            };
            var searchResults = new List<FrameworkSearchResult>();
            var searchResultsViewModel = new List<FrameworkCertificateSearchResultsViewModel>();   

            _mapperMock.Setup(m => m.Map<FrameworkSearchQuery>(vm)).Returns(searchQuery);
            _frameworkSearchApiClient.Setup(c => c.SearchFrameworks(searchQuery)).ReturnsAsync(searchResults);
            _mapperMock.Setup(m => m.Map<List<FrameworkCertificateSearchResultsViewModel>>(searchResults)).Returns(searchResultsViewModel);

            // Act
            var result = await _controller.Results(vm);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("NoResults"); 
            redirectResult.ControllerName.Should().BeNull(); 

            _controller.ModelState.IsValid.Should().BeTrue();

            _sessionServiceMock.Object.SessionFrameworkSearch.Should().BeNull();
        }

        [Test]
        public async Task Results_FrameworksSearch_ValidInput_WithOneResult_RedirectsToCertificateView()
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
            var searchQuery = new FrameworkSearchQuery
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                DateOfBirth = new DateTime(2000, 1, 1)
            };
            var searchResults = new List<FrameworkSearchResult>
            { 
                new FrameworkSearchResult
                {
                    ApprenticeshipLevelName = "Higher",
                    CertificationYear = "2002",
                    FrameworkName = "Framework A",
                    Id = Guid.NewGuid()
                }
            };
            var searchResultsViewModel = new List<FrameworkCertificateSearchResultsViewModel> { new FrameworkCertificateSearchResultsViewModel 
            { 
                FirstName = searchQuery.FirstName,
                LastName = searchQuery.LastName,
                DateOfBirth = searchQuery.DateOfBirth, 
                SelectedResult = searchResults[0].Id
            } };

            _mapperMock.Setup(m => m.Map<FrameworkSearchQuery>(vm)).Returns(searchQuery);
            _frameworkSearchApiClient.Setup(c => c.SearchFrameworks(searchQuery)).ReturnsAsync(searchResults);
            _mapperMock.Setup(m => m.Map<List<FrameworkCertificateSearchResultsViewModel>>(searchResults)).Returns(searchResultsViewModel);
            // Act
            var result = await _controller.Results(vm);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("Certificate");
            redirectResult.ControllerName.Should().BeNull();

            _controller.ModelState.IsValid.Should().BeTrue();
        }

        [Test]
        [MoqAutoData]
        public async Task Results_FrameworksSearch_ValidInput_WithOneResult_UpdatesSessionObject()
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
            var searchQuery = new FrameworkSearchQuery
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                DateOfBirth = new DateTime(2000, 1, 1)
            };
            var searchResults = new List<FrameworkSearchResult>
            { 
                new FrameworkSearchResult
                {
                    ApprenticeshipLevelName = "Higher",
                    CertificationYear = "2002",
                    FrameworkName = "Framework A",
                    Id = Guid.NewGuid()
                }
            };
            var searchResultsViewModel = new List<FrameworkCertificateSearchResultsViewModel> { new FrameworkCertificateSearchResultsViewModel 
            { 
                FirstName = searchQuery.FirstName,
                LastName = searchQuery.LastName,
                DateOfBirth = searchQuery.DateOfBirth, 
                SelectedResult = searchResults[0].Id
            } };

            _mapperMock.Setup(m => m.Map<FrameworkSearchQuery>(vm)).Returns(searchQuery);
            _frameworkSearchApiClient.Setup(c => c.SearchFrameworks(searchQuery)).ReturnsAsync(searchResults);
            _mapperMock.Setup(m => m.Map<List<FrameworkCertificateSearchResultsViewModel>>(searchResults)).Returns(searchResultsViewModel);

            FrameworkSearchSessionData capturedFrameworkSearch = null;
            _sessionServiceMock.SetupSet(s => s.SessionFrameworkSearch = It.IsAny<FrameworkSearchSessionData>())
                .Callback((FrameworkSearchSessionData value) => capturedFrameworkSearch = value);

            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(new FrameworkSearchSessionData());

            //Act
            var result = await _controller.Results(vm);

            // Assert
            _sessionServiceMock.VerifySet(s => s.SessionFrameworkSearch = It.IsAny<FrameworkSearchSessionData>(), Times.Once);

            capturedFrameworkSearch.Should().NotBeNull();
            capturedFrameworkSearch.FirstName.Should().Be(searchQuery.FirstName);
            capturedFrameworkSearch.LastName.Should().Be(searchQuery.LastName);
            capturedFrameworkSearch.DateOfBirth.Should().Be(searchQuery.DateOfBirth);
            capturedFrameworkSearch.SelectedResult.Should().Be(searchResultsViewModel[0].SelectedResult);
        }


        [Test]
        public async Task Select_ValidInput_ReturnsCorrectViewModel()
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
            var result = await _controller.Select(stdCode, uln, searchString, page, allLogs, batchNumber);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();

            var model = viewResult.Model as SelectViewModel;
            model.Should().NotBeNull();
            model.Learner.Should().Be(learnerDetails);
            model.SearchString.Should().Be(searchString);
            model.Page.Should().Be(page);
            model.ShowDetail.Should().Be(!allLogs);
            model.BatchNumber.Should().Be(batchNumber);
        }

        [Test]
        public async Task Select_LearnerDetailsApiClientThrowsException_APIErrorThrown()
        {
            // Arrange
            int stdCode = 123;
            long uln = 456;
            bool allLogs = false;

            _learnerDetailsApiClientMock.Setup(x => x.GetLearnerDetail(stdCode, uln, allLogs)).ThrowsAsync(new Exception("API Error"));

            // Act
            Func<Task> act = async () => await _controller.Select(stdCode, uln, "test search", 1, allLogs, null);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("API error");

        }

        [Test]
        public async Task Select_ReturnsCorrectView()
        {
            // Arrange
            int stdCode = 123;
            long uln = 456;
            bool allLogs = false;

            var learnerDetails = new LearnerDetailResult { Uln = uln, StandardCode = stdCode };
            _learnerDetailsApiClientMock.Setup(x => x.GetLearnerDetail(stdCode, uln, allLogs)).ReturnsAsync(learnerDetails);

            // Act
            var result = await _controller.Select(stdCode, uln, "test search", 1, allLogs, null);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().BeNullOrEmpty(); 
        }

        [Test]
        public void MultipleResults_SessionFrameworkSearchIsNull_ReturnsViewWithNullViewModel()
        {
            // Arrange
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns((FrameworkSearchSessionData)null);

            // Act
            var result = _controller.MultipleResults();

            // Assert
            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();

                var viewResult = result.Should().BeOfType<ViewResult>().Which;
                viewResult.Should().NotBeNull();

                var viewModel = viewResult.Model.Should().BeNull();
            });
        }

        [Test]
        public void MultipleResults_SessionFrameworkSearchIsNotNull_ReturnsViewWithMappedViewModel()
        {
            // Arrange
            var sessionFrameworkSearch = new FrameworkSearchSessionData { FirstName = "Kevin", LastName = "Edgewater" };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionFrameworkSearch);
            var mappedViewModel = new FrameworkCertificateSearchResultsViewModel { FirstName = "Kevin", LastName = "Edgewater" };
            _mapperMock.Setup(m => m.Map<FrameworkCertificateSearchResultsViewModel>(sessionFrameworkSearch)).Returns(mappedViewModel);

            // Act
            var result = _controller.MultipleResults();

            // Assert
            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();

                var viewResult = result.Should().BeOfType<ViewResult>().Which;
                viewResult.Should().NotBeNull();

                var viewModel = viewResult.Model.Should().BeOfType<FrameworkCertificateSearchResultsViewModel>().Which;
                viewModel.Should().NotBeNull();
                viewModel.FirstName.Should().Be("Kevin");
                viewModel.LastName.Should().Be("Edgewater");
            });
        }

        [Test]
        public async Task SelectFramework_ModelStateIsValid_UpdatesSessionAndRedirectsToCertificate()
        {
            // Arrange
            var vm = new FrameworkCertificateSearchResultsViewModel { SelectedResult = Guid.NewGuid() };
            _controller.ModelState.Clear(); 

            FrameworkSearchSessionData capturedSessionObject = null;
            _sessionServiceMock.Setup(s => s.UpdateFrameworkSearchRequest(It.IsAny<Action<FrameworkSearchSessionData>>()))
                .Callback<Action<FrameworkSearchSessionData>>(action =>
                {
                    capturedSessionObject = new FrameworkSearchSessionData();
                    action(capturedSessionObject);
                });

            // Act
            var result = await _controller.SelectFramework(vm);

            // Assert
            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();

                var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Which;
                redirectToActionResult.ActionName.Should().Be("Certificate");
            });

            _sessionServiceMock.Verify(s => s.UpdateFrameworkSearchRequest(It.IsAny<Action<FrameworkSearchSessionData>>()), Times.Once);
            capturedSessionObject.Should().NotBeNull();
            capturedSessionObject.SelectedResult.Should().Be(vm.SelectedResult);
        }

        [Test]
        public async Task Certificate_SessionIsNull_RedirectsToIndex()
        {
            // Arrange
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns((FrameworkSearchSessionData)null);

            // Act
            var result = await _controller.Certificate();

            // Assert
            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();

                var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Which;
                redirectToActionResult.ActionName.Should().Be("Index");
            });
        }

        [Test]
        public async Task Certificate_SelectedResultIsNull_RedirectsToIndex()
        {
            
            // Arrange
            var results = Builder<FrameworkCertificateSummaryViewModel>.CreateListOfSize(1)
                .All()
                .Build()
                .ToList();
            var sessionModel = new FrameworkSearchSessionData
            {
                FrameworkResults = results,
                SelectedResult = null,
                FirstName = "Bob",
                LastName = "Holland",
                DateOfBirth = DateTime.Now.AddYears(-22)
            };

            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            // Act
            var result = await _controller.Certificate();

            // Assert
            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();

                var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Which;
                redirectToActionResult.ActionName.Should().Be("Index");
            });
        }

        [Test]
        [MoqAutoData]
        public async Task Certificate_SessionAndSelectedResultValid_CallsGetFrameworkCertificate(
            GetFrameworkCertificateResult certificateResult)
        {
            // Arrange
            var results = Builder<FrameworkCertificateSummaryViewModel>.CreateListOfSize(3)
                .All()
                .Build()
                .ToList();
            var sessionModel = new FrameworkSearchSessionData
            {
                FirstName = "First",
                LastName = "",
                DateOfBirth = DateTime.Now.AddYears(-28),
                FrameworkResults = results,
                SelectedResult = results[0].Id
            };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            _frameworkSearchApiClient.Setup(api => api.GetFrameworkCertificate(It.IsAny<Guid>())).ReturnsAsync(certificateResult);

            // Act
            var result = await _controller.Certificate();

            // Assert
            _frameworkSearchApiClient.Verify(api => api.GetFrameworkCertificate(sessionModel.SelectedResult.Value), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task Certificate_SessionAndSelectedResultValid_MapsViewModel(
            GetFrameworkCertificateResult certificateResult, 
            FrameworkCertificateViewModel certificateViewModel)
        {
            // Arrange
            var results = Builder<FrameworkCertificateSummaryViewModel>.CreateListOfSize(3)
                .All()
                .Build()
                .ToList();
            var sessionModel = new FrameworkSearchSessionData
            {
                FirstName = "First",
                LastName = "",
                DateOfBirth = DateTime.Now.AddYears(-28),
                FrameworkResults = results,
                SelectedResult = results[0].Id
            };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            _frameworkSearchApiClient.Setup(api => api.GetFrameworkCertificate(It.IsAny<Guid>())).ReturnsAsync(certificateResult);
            _mapperMock.Setup(m => m.Map<FrameworkCertificateViewModel>(certificateResult)).Returns(certificateViewModel);

            // Act
            var result = await _controller.Certificate();

            // Assert
            _mapperMock.Verify(m => m.Map<FrameworkCertificateViewModel>(certificateResult), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task Certificate_SessionAndSelectedResultValid_ReturnsCorrectView(
            GetFrameworkCertificateResult certificateResult, 
            FrameworkCertificateViewModel certificateViewModel)
        {
            // Arrange
            var results = Builder<FrameworkCertificateSummaryViewModel>.CreateListOfSize(3)
                .All()
                .Build()
                .ToList();
            var sessionModel = new FrameworkSearchSessionData
            {
                FirstName = "First",
                LastName = "",
                DateOfBirth = DateTime.Now.AddYears(-28),
                FrameworkResults = results,
                SelectedResult = results[0].Id
            };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            _frameworkSearchApiClient.Setup(api => api.GetFrameworkCertificate(It.IsAny<Guid>())).ReturnsAsync(certificateResult);
            _mapperMock.Setup(m => m.Map<FrameworkCertificateViewModel>(certificateResult)).Returns(certificateViewModel);

            // Act
            var result = await _controller.Certificate();

            // Assert
            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();

                var viewResult = result.Should().BeOfType<ViewResult>().Which;

                var resultModel = viewResult.Model.Should().BeOfType<FrameworkCertificateViewModel>().Which;
                resultModel.Should().NotBeNull();
            });


        }

        [Test]
        public async Task CertificateBackAction_FrameworkResultsHasMultipleItems_UpdatesSessionAndRedirectsToMultipleResults()
        {
            // Arrange
            var sessionModel = new FrameworkSearchSessionData
            {
                FrameworkResults = Builder<FrameworkCertificateSummaryViewModel>.CreateListOfSize(3)
                .All()
                .Build()
                .ToList() 
            };

            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            // Act
            var result = await _controller.CertificateBackAction();

            // Assert
            _sessionServiceMock.Verify(s => s.UpdateFrameworkSearchRequest(It.IsAny<System.Action<FrameworkSearchSessionData>>()), Times.Once);
            sessionModel.SelectedResult.Should().BeNull();

            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();

                var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Which;
                redirectToActionResult.ActionName.Should().Be("MultipleResults");
            });
        }

        [Test]
        public async Task CertificateBackAction_FrameworkResultsHasOneItem_ClearSessionAndRedirectsToIndex()
        {
            // Arrange
            var sessionModel = new FrameworkSearchSessionData
            {
                FrameworkResults = Builder<FrameworkCertificateSummaryViewModel>.CreateListOfSize(1)
                .All()
                .Build()
                .ToList() 
            };

            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            // Act
            var result = await _controller.CertificateBackAction();

            // Assert
            _sessionServiceMock.Verify(s => s.ClearFrameworkSearchRequest(), Times.Once);

            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();

                var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Which;
                redirectToActionResult.ActionName.Should().Be("Index");
            });
        }

        [Test]
        public async Task CertificateBackAction_SessionIsNull_RedirectsToIndex()
        {
            // Arrange
            var sessionModel = new FrameworkSearchSessionData
            {
                FrameworkResults = Builder<FrameworkCertificateSummaryViewModel>.CreateListOfSize(1)
                .All()
                .Build()
                .ToList() 
            };

            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            // Act
            var result = await _controller.CertificateBackAction();

            // Assert
            Assert.Multiple(() =>
            {
                result.Should().NotBeNull();

                var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Which;
                redirectToActionResult.ActionName.Should().Be("Index");
            });
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
