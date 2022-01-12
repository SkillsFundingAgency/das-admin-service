using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenPostingStartNewMergeRequest : MergeControllerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [Test]
        public void Then_StartNewRequest()
        {
            MergeController.Start();

            _mockMergeSessionService.Verify(ms => ms.StartNewMergeRequest(), Times.Once);
        }

        [Test]
        public void Then_ReturnRedirectToMergeOverview()
        {
            var result = MergeController.Start() as RedirectToActionResult;

            result.ActionName.Should().Be("MergeOverview");
        }
    }
}
