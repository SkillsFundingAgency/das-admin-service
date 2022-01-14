using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Models.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenRequestingMergeOverviewPage : MergeControllerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [Test]
        public void Then_GetMergeRequestFromSession()
        {
            MergeController.MergeOverview();

            VerifyGetMergeRequest();
        }

        [Test]
        public void And_SessionIsSet_Then_MapViewModel_And_ReturnView()
        {
            var mergeRquest = _autoFixture.Build<MergeRequest>().Create();

            _mockMergeSessionService.Setup(ms => ms.GetMergeRequest())
                .Returns(mergeRquest);

            var viewResult = MergeController.MergeOverview() as ViewResult;

            viewResult.ViewName.Should().Be("");

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
            var mergeRquest = new MergeRequest();

            _mockMergeSessionService.Setup(ms => ms.GetMergeRequest())
                .Returns(mergeRquest);

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
