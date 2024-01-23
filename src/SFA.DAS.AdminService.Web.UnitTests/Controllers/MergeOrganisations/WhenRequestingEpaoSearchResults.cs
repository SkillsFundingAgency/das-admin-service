using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenRequestingEpaoSearchResults : MergeControllerTestBase
    {
        private List<AssessmentOrganisationSummary> _response;

        [TestCase("primary")]
        [TestCase("secondary")]
        public async Task And_UserIsNavigatingBackwards_Then_SearchCommandIsAdded(string type)
        {
            SetUpApiResponse();

            var viewModel = _autoFixture.Create<SearchOrganisationViewModel>();

            await MergeController.EpaoSearchResults(type, true, viewModel);

            _mockMergeSessionService.Verify(ms => ms.AddSearchEpaoCommand(type, viewModel.SearchString), Times.Never());
        }

        [TestCase("primary")]
        [TestCase("secondary")]
        public async Task And_UserIsNavigatingForwards_Then_SearchCommandIsAdded(string type)
        {
            SetUpApiResponse();

            var viewModel = _autoFixture.Create<SearchOrganisationViewModel>();

            await MergeController.EpaoSearchResults(type, null, viewModel);

            _mockMergeSessionService.Verify(ms => ms.AddSearchEpaoCommand(type, viewModel.SearchString), Times.Once());
        }

        [TestCase("primary")]
        [TestCase("secondary")]
        public async Task Then_ResultsAreMapped(string type)
        {
            SetUpApiResponse();

            var searchViewModel = _autoFixture.Create<SearchOrganisationViewModel>();

            var result = await MergeController.EpaoSearchResults(type, true, searchViewModel) as ViewResult;

            var model = result.Model as EpaoSearchResultsViewModel;

            model.Results.Count.Should().Be(_response.Count);
        }

        [TestCase("primary")]
        [TestCase("secondary")]
        public async Task And_ModelIsInvalid_Then_ReturnView(string type)
        {
            MergeController.ModelState.AddModelError("Error", "Error message");

            var viewModel = _autoFixture.Create<SearchOrganisationViewModel>();

            var result = await MergeController.EpaoSearchResults(type, null, viewModel) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }

        private void SetUpApiResponse()
        {
            _response = _autoFixture.Create<List<AssessmentOrganisationSummary>>();

            _registerApiClient.Setup(c => c.SearchOrganisations(It.IsAny<string>()))
                .ReturnsAsync(_response);
        }
    }
}
