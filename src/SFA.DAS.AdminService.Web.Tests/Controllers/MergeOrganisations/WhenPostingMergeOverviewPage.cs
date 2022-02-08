using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.ViewModels.Merge;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenPostingMergeOverviewPage : MergeControllerTestBase
    {
        [Test]
        public void And_ModelIsValid_Then_RedirectToConfirmAndCompletePage()
        {
            var viewModel = _autoFixture.Create<MergeOverviewViewModel>();

            var result = MergeController.MergeOverview(viewModel) as RedirectToActionResult;

            result.ActionName.Should().Be(nameof(MergeController.ConfirmAndComplete));
        }

        [Test]
        public void And_ModelIsInvalid_Then_ReturnView()
        {
            MergeController.ModelState.AddModelError("Error", "Error message");

            var viewModel = _autoFixture.Build<MergeOverviewViewModel>().Create();

            var result = MergeController.MergeOverview(viewModel) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }
    }
}
