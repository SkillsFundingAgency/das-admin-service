using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Models.Merge;
using SFA.DAS.AdminService.Web.ViewModels.Merge;
using SFA.DAS.AssessorService.Api.Types.Commands;
using System;
using System.Collections.Generic;
using System.Security.Claims;
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
            SetupApiResponse();
        }

        [Test]
        public async Task Then_ExecuteMergeRequest()
        {
            var viewModel = SetupViewModel();

            await MergeController.ConfirmAndComplete(viewModel);

            _mockApiClient.Verify(client => client.MergeOrganisations(It.Is<MergeOrganisationsCommand>(
                c => c.ActionedByUser == "user@test.com"
                    && c.PrimaryEndPointAssessorOrganisationId == _mergeRequest.PrimaryEpao.Id
                    && c.SecondaryEndPointAssessorOrganisationId == _mergeRequest.SecondaryEpao.Id
                    && c.SecondaryStandardsEffectiveTo == _mergeRequest.SecondaryEpaoEffectiveTo.Value)), Times.Once());
        }

        [Test]
        public async Task And_ExecuteMergeRequestSuccessful_Then_MarkMergeRequestComplete()
        {
            var viewModel = SetupViewModel();

            await MergeController.ConfirmAndComplete(viewModel);

            _mockMergeSessionService.Verify(ms => ms.UpdateMergeRequest(It.Is<MergeRequest>(r => r.Completed == true)));
        }

        [Test]
        public async Task And_ExecuteMergeRequestSuccessful_Then_ReturnRedirectToComplete()
        {
            var viewModel = SetupViewModel();

            var result = await MergeController.ConfirmAndComplete(viewModel) as RedirectToActionResult;

            result.ActionName.Should().Be(nameof(MergeController.MergeComplete));
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

            _mockMergeSessionService.Verify(ms => ms.UpdateMergeRequest(It.IsAny<MergeRequest>()), Times.Never()); 
        }

        [Test]
        public async Task And_WarningIsNotAccepted_Then_ReturnView()
        {
            MergeController.ModelState.AddModelError("Error", "Error message");

            var viewModel = _autoFixture.Build<ConfirmAndCompleteViewModel>()
                .With(c => c.AcceptWarning, false)
                .Create();

            var result = await MergeController.ConfirmAndComplete(viewModel) as ViewResult;

            var model = result.Model as ConfirmAndCompleteViewModel;

            model.Should().BeEquivalentTo(viewModel);
        }

        private ConfirmAndCompleteViewModel SetupViewModel()
        {
            return _autoFixture.Build<ConfirmAndCompleteViewModel>()
                .With(c => c.AcceptWarning, true)
                .Create();
        }

        private void SetupApiResponse()
        {
            var response = new { id = Guid.NewGuid() };

            _mockApiClient.Setup(c => c.MergeOrganisations(It.IsAny<MergeOrganisationsCommand>()))
                .ReturnsAsync(response);
        }

        private void SetupContextAccessor()
        {
            var claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "user@test.com")
            };

            var identity = new ClaimsIdentity(claims);

            var user = new ClaimsPrincipal(identity);

            _mockContextAccessor.Setup(a => a.HttpContext.User)
                .Returns(user);
        }
    }
}
