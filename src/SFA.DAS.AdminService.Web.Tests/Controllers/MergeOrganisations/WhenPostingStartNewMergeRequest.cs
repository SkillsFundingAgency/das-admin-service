using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenPostingStartNewMergeRequest : MergeControllerTestBase
    {
        [Test]
        public void Then_StartNewRequest()
        {
            MergeController.StartNow();

            _mockMergeSessionService.Verify(ms => ms.StartNewMergeRequest(), Times.Once);
        }

        [Test]
        public void Then_ReturnRedirectToMergeOverview()
        {
            var result = MergeController.StartNow() as RedirectToActionResult;

            result.ActionName.Should().Be(nameof(MergeController.MergeOverview));
        }
    }
}
