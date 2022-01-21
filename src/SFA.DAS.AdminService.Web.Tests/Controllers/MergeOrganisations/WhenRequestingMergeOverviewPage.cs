using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Models.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenRequestingMergeOverviewPage : MergeControllerTestBase
    {
        [Test]
        public void Then_GetMergeRequestFromSession()
        {
            MergeController.MergeOverview();

            VerifyGetMergeRequest();
        }

        [Test]
        public void And_SessionIsSet_Then_MapViewModel()
        {
            var mergeRquest = _autoFixture.Build<MergeRequest>().Create();

            _mockMergeSessionService.Setup(ms => ms.GetMergeRequest())
                .Returns(mergeRquest);

            var viewResult = MergeController.MergeOverview() as ViewResult;

            var viewModel = viewResult.Model as MergeOverviewViewModel;

            viewModel.PrimaryEpaoId.Should().Be(mergeRquest.PrimaryEpao.Id);
            viewModel.PrimaryEpaoName.Should().Be(mergeRquest.PrimaryEpao.Name);
            viewModel.SecondaryEpaoId.Should().Be(mergeRquest.SecondaryEpao.Id);
            viewModel.SecondaryEpaoName.Should().Be(mergeRquest.SecondaryEpao.Name);
            viewModel.SecondaryEpaoEffectiveTo.Should().Be(mergeRquest.SecondaryEpaoEffectiveTo);
        }

        [Test]
        public void And_SessionIsNull_ThenMapViewModel()
        {
            var mergeRequest = new MergeRequest();
            mergeRequest.StartNewRequest();

            _mockMergeSessionService.Setup(ms => ms.GetMergeRequest())
                .Returns(mergeRequest);

            var viewResult = MergeController.MergeOverview() as ViewResult;

            var viewModel = viewResult.Model as MergeOverviewViewModel;

            viewModel.PrimaryEpaoId.Should().BeNull();
            viewModel.PrimaryEpaoName.Should().BeNull();
            viewModel.SecondaryEpaoId.Should().BeNull();
            viewModel.SecondaryEpaoName.Should().BeNull();
            viewModel.SecondaryEpaoEffectiveTo.Should().BeNull();
        }
    }
}
