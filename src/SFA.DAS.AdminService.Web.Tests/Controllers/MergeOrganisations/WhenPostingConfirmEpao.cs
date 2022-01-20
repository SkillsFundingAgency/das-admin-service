using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Models.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenPostingConfirmEpao : MergeControllerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [TestCase("primary")]
        [TestCase("secondary")]
        public void And_ModelIsValid_Then_UpdateMergeRequest(string type)
        {
            SetUpGetMergeRequest();

            var viewModel = _autoFixture.Build<ConfirmEpaoViewModel>()
                .With(vm => vm.OrganisationType, type)
                .Create();

            MergeController.ConfirmEpao(type, viewModel);

            if (type == "primary")
            {
                _mockMergeSessionService.Verify(ms => ms.UpdateMergeRequest(It.Is<MergeRequest>(r => r.PrimaryEpao.Id == viewModel.EpaoId)), Times.Once);
            }
            else if (type == "secondary")
            {
                _mockMergeSessionService.Verify(ms => ms.UpdateMergeRequest(It.Is<MergeRequest>(r => r.SecondaryEpao.Id == viewModel.EpaoId)), Times.Once);
            }
        }

        [TestCase("primary")]
        [TestCase("secondary")]
        public void And_ModelIsValid_Then_RedirectToMergeOverview(string type)
        {
            SetUpGetMergeRequest();

            var viewModel = _autoFixture.Create<ConfirmEpaoViewModel>();

            var result = MergeController.ConfirmEpao(type, viewModel) as RedirectToActionResult;

            result.ActionName.Should().Be(nameof(MergeController.MergeOverview));
        }

        [TestCase("primary")]
        [TestCase("secondary")]
        public void And_ModelIsInvalid_Then_ReturnView(string type)
        {
            MergeController.ModelState.AddModelError("Error", "Error message");

            var viewModel = _autoFixture.Create<ConfirmEpaoViewModel>();

            var result = MergeController.ConfirmEpao(type, viewModel) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }

        private void SetUpGetMergeRequest()
        {
            _mergeRequest = _autoFixture.Create<MergeRequest>();

            _mockMergeSessionService.Setup(ms => ms.GetMergeRequest())
                .Returns(_mergeRequest);
        }
    }
}
