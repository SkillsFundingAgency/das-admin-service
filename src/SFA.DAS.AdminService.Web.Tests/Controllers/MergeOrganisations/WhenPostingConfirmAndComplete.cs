using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Web.Models.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
using SFA.DAS.AssessorService.Api.Types.Commands;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenPostingConfirmAndComplete : MergeControllerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();

            SetupContextAccessor();
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
            SetupApiResponse();
            //SetupMergeRequest();

            var viewModel = SetupViewModel();

            await MergeController.ConfirmAndComplete(viewModel);

            _mockMergeSessionService.Verify(ms => ms.UpdateMergeRequest(It.Is<MergeRequest>(r => r.Completed == true)));
        }

        [Test]
        public async Task And_ExecuteMergeRequestSuccessful_Then_ReturnRedirectToComplete()
        {
            var viewModel = SetupViewModel();

            await MergeController.ConfirmAndComplete(viewModel);

            // Verify executed
            // _mockApiClient.Verify()
        }

        [Test]
        public async Task And_MergeCommandThrowsException_Then_RedirectToErrorPage()
        {
            _mockApiClient.Setup(c => c.MergeOrganisations(It.IsAny<MergeOrganisationsCommand>()))
                .ThrowsAsync(new Exception());

            var viewModel = SetupViewModel();

            var result = await MergeController.ConfirmAndComplete(viewModel) as RedirectToActionResult;

            result.ActionName.Should().Be(nameof(MergeController.MergeError));
        }

        [Test]
        public async Task And_MergeCommandThrowsException_Then_MergeRequestIsNotUpdated()
        {
            _mockApiClient.Setup(c => c.MergeOrganisations(It.IsAny<MergeOrganisationsCommand>()))
                .ThrowsAsync(new Exception());

            var viewModel = SetupViewModel();

            await MergeController.ConfirmAndComplete(viewModel);

            _mockMergeSessionService.Verify(ms => ms.UpdateMergeRequest(It.Is<MergeRequest>(r => r.Completed == true)), Times.Never()); ;
        }

        private ConfirmAndCompleteViewModel SetupViewModel()
        {
            return new ConfirmAndCompleteViewModel
            {
                AcceptWarning = true
            };
        }

        private void SetupApiResponse()
        {
            var response = new { id = Guid.NewGuid() };

            _mockApiClient.Setup(c => c.MergeOrganisations(It.IsAny<MergeOrganisationsCommand>()))
                .ReturnsAsync(response);
        }

        private void SetupContextAccessor()
        {
            _mockContextAccessor.Setup(a => a.HttpContext.User.UserId())
                .Returns("user@test.com");
        }
    }
}
