using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenPostingConfirmAndComplete : MergeControllerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [Test]
        public async Task Then_ExecuteMergeRequest()
        {
            var viewModel = SetupViewModel();

            await MergeController.ConfirmAndComplete(viewModel);

            // Verify executed
           // _mockApiClient.Verify()
        }

        [Test]
        public async Task And_ExecuteMergeRequestSuccessful_Then_MarkMergeRequestComplete()
        {
            var viewModel = SetupViewModel();
            // setup valid response

            await MergeController.ConfirmAndComplete(viewModel);

            _mockMergeSessionService.Verify(ms => ms.MarkComplete(), Times.Once());
        }

        [Test]
        public async Task And_ExecuteMergeRequestSuccessful_Then_ReturnRedirectToComplete()
        {
            var viewModel = SetupViewModel();

            await MergeController.ConfirmAndComplete(viewModel);

            // Verify executed
            // _mockApiClient.Verify()
        }

        private ConfirmAndCompleteViewModel SetupViewModel()
        {
            return new ConfirmAndCompleteViewModel
            {
                AcceptWarning = true
            };
        }
    }
}
