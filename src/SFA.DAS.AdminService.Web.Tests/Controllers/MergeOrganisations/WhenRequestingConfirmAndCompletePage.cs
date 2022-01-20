using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.ViewModels.Merge;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenRequestingConfirmAndCompletePage : MergeControllerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [Test]
        public void Then_MapMergeRequest()
        {
            var result = MergeController.ConfirmAndComplete() as ViewResult;

            var model = result.Model as ConfirmAndCompleteViewModel;

            model.AcceptWarning.Should().BeFalse();
            model.PrimaryEpaoName.Should().Be(_mergeRequest.PrimaryEpao.Name);
            model.SecondaryEpaoName.Should().Be(_mergeRequest.SecondaryEpao.Name);
        }
    }
}
