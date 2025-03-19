using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models;
using AutoMapper;
using SFA.DAS.AdminService.Web.Infrastructure.FrameworkSearch;
using System;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AdminService.Web.Models.Search;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class SearchControllerTestsBase
    {
        protected SearchController _controller;
        protected Mock<ILearnerDetailsApiClient> _learnerDetailsApiClientMock;
        protected Mock<IRegisterApiClient> _registerApiClientMock;
        protected Mock<IStaffSearchApiClient> _staffSearchApiClientMock;
        protected Mock<IFrameworkSearchSessionService> _sessionServiceMock;
        protected Mock<IScheduleApiClient> _scheduleApiClientMock;
        protected Mock<IMapper> _mapperMock;

        [SetUp]
        public void SetupBase()
        {
            _learnerDetailsApiClientMock = new Mock<ILearnerDetailsApiClient>();
            _registerApiClientMock = new Mock<IRegisterApiClient>();
            _staffSearchApiClientMock = new Mock<IStaffSearchApiClient>();
             _sessionServiceMock = new Mock<IFrameworkSearchSessionService>();
            _scheduleApiClientMock = new Mock<IScheduleApiClient>();
            _mapperMock = new Mock<IMapper>();
            _controller = new SearchController(_learnerDetailsApiClientMock.Object, _registerApiClientMock.Object, _staffSearchApiClientMock.Object,
                _sessionServiceMock.Object, _scheduleApiClientMock.Object, _mapperMock.Object);
        }
        protected SearchInputViewModel CreateValidSearchInputViewModel()
        {
            return new SearchInputViewModel
            {
                SearchType = SearchTypes.Frameworks,
                FirstName = "John",
                LastName = "Doe",
                Day = "01",
                Month = "01",
                Year = "2000"
            };
        }
        protected FrameworkLearnerSearchRequest CreateValidFrameworkLearnerSearchRequest()
        {
            return new FrameworkLearnerSearchRequest
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(2000, 1, 1)
            };
        }
        protected void SetupValidFrameworkSearchMapping(SearchInputViewModel vm, FrameworkLearnerSearchRequest searchQuery)
        {
            _mapperMock.Setup(m => m.Map<FrameworkLearnerSearchRequest>(vm)).Returns(searchQuery);
        }
        protected void SetupFrameworkSearchApiClient(FrameworkLearnerSearchRequest searchQuery, List<FrameworkLearnerSearchResponse> results)
        {
            _staffSearchApiClientMock.Setup(x => x.SearchFrameworkLearners(searchQuery)).ReturnsAsync(results);
        }
        protected void SetupFrameworkSearchResultsMapping(List<FrameworkLearnerSearchResponse> results, List<FrameworkLearnerSearchResultsViewModel> mappedResults)
        {
            _mapperMock.Setup(m => m.Map<List<FrameworkLearnerSearchResultsViewModel>>(results)).Returns(mappedResults);
        }
        protected List<FrameworkLearnerSearchResultsViewModel> CreateMappedFrameworkResults(int count)
        {
            var mappedResults = new List<FrameworkLearnerSearchResultsViewModel>();
            for (int i = 0; i < count; i++)
            {
                mappedResults.Add(new FrameworkLearnerSearchResultsViewModel {  });
            }
            return mappedResults;
        }
        protected List<FrameworkLearnerSearchResponse> CreateFrameworkSearchResponses(int count)
        {
            var results = new List<FrameworkLearnerSearchResponse>();
            for (int i = 0; i < count; i++)
            {
                results.Add(new FrameworkLearnerSearchResponse {  });
            }
            return results;
        }
    }
}
