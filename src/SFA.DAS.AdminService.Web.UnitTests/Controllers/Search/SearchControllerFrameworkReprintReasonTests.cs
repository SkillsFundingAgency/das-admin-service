using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Models.Search;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class FrameworkReprintReasonTests : SearchControllerTestsBase 
    {
        [Test]
        public async Task FrameworkReprintReason_SessionIsNull_RedirectsToIndex()
        {
            // Arrange
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns((FrameworkSearchSession)null);

            // Act
            var result = await _controller.FrameworkReprintReason();

            // Assert
            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        public async Task FrameworkReprintReason_SelectedResultIsNull_RedirectsToIndex()
        {
            // Arrange
            var sessionModel = new FrameworkSearchSession { SelectedFrameworkLearnerId = null };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            // Act
            var result = await _controller.FrameworkReprintReason();

            // Assert
            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        [MoqAutoData]
        public async Task FrameworkReprintReason_ValidSessionAndResults_ReturnsViewWithCorrectModel(GetFrameworkLearnerResponse frameworkLearnerResponse)
        {
            // Arrange
            var sessionModel = new FrameworkSearchSession
            {
                FrameworkResults = new List<FrameworkLearnerSummaryViewModel> { new FrameworkLearnerSummaryViewModel() },
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = DateTime.Now.AddYears(-20),
                SelectedFrameworkLearnerId = Guid.NewGuid(),
            };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            _learnerDetailsApiClientMock.Setup(c => c.GetFrameworkLearner(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(frameworkLearnerResponse);
            var mappedViewModel = new FrameworkLearnerReprintReasonViewModel { ApprenticeName = "Test User" };
            _mapperMock.Setup(m => m.Map<FrameworkLearnerReprintReasonViewModel>(sessionModel)).Returns(mappedViewModel);

            // Act
            var result = await _controller.FrameworkReprintReason();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<FrameworkLearnerReprintReasonViewModel>().Subject;
            model.ApprenticeName.Should().Be(mappedViewModel.ApprenticeName);
            model.BackAction.Should().Be("FrameworkLearnerDetails");
            model.BackToCheckAnswers.Should().BeFalse();
            model.CertificateReference.Should().Be(frameworkLearnerResponse.CertificateReference);
            model.CertificateStatus.Should().Be(frameworkLearnerResponse.CertificateStatus);
            model.CertificateStatusDate.Should().Be(frameworkLearnerResponse.CertificateStatusDate);
            _mapperMock.Verify(m => m.Map<FrameworkLearnerReprintReasonViewModel>(sessionModel), Times.Once);
        }

        

        [Test]
        public void UpdateFrameworkReprintReason_ValidModelState_ClearsSessionAndRedirects()
        {
            // Arrange
            var vm = new FrameworkLearnerAmendReprintReasonViewModel { SelectedReprintReasons = new List<string> { "Reason1" }, TicketNumber = "123", OtherReason = "Other" };
            _controller.ModelState.Clear(); 
            _controller.ModelState.IsValid.Should().BeTrue();

            var sessionModel = new FrameworkSearchSession
            {
                SelectedFrameworkLearnerId = Guid.NewGuid(),
                SelectedReprintReasons = vm.SelectedReprintReasons,
                TicketNumber = vm.TicketNumber,
                OtherReason = vm.OtherReason
            };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            Action<FrameworkSearchSession> capturedAction = null;
            _sessionServiceMock.Setup(s => s.UpdateFrameworkSearchRequest(It.IsAny<Action<FrameworkSearchSession>>()))
                .Callback<Action<FrameworkSearchSession>>(action => capturedAction = action);


            // Act
            var result = _controller.FrameworkReprintReason(vm);

            // Assert
            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("FrameworkAddress");
            _sessionServiceMock.Verify(s => s.UpdateFrameworkSearchRequest(It.IsAny<Action<FrameworkSearchSession>>()), Times.Once);

            capturedAction.Should().NotBeNull();
            capturedAction(sessionModel); 
            sessionModel.SelectedReprintReasons.Should().BeEquivalentTo(vm.SelectedReprintReasons);
            sessionModel.TicketNumber.Should().Be(vm.TicketNumber);
            sessionModel.OtherReason.Should().Be(vm.OtherReason);
        }

        [Test]
        public void UpdateFrameworkReprintReason_InvalidModelState_UpdatesSessionAndRedirects()
        {
            // Arrange
            var vm = new FrameworkLearnerAmendReprintReasonViewModel
            {
                SelectedReprintReasons = new List<string> { "Reason1" },
                TicketNumber = "123",
                OtherReason = "Other"
            };
            _controller.ModelState.AddModelError("Error", "Error");
            _controller.ModelState.IsValid.Should().BeFalse();

            var sessionModel = new FrameworkSearchSession();
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            // Capture the Action
            Action<FrameworkSearchSession> capturedAction = null;
            _sessionServiceMock.Setup(s => s.UpdateFrameworkSearchRequest(It.IsAny<Action<FrameworkSearchSession>>()))
                .Callback<Action<FrameworkSearchSession>>(action => capturedAction = action);

            // Act
            var result = _controller.FrameworkReprintReason(vm);

            // Assert
            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("FrameworkReprintReason");
            _sessionServiceMock.Verify(s => s.UpdateFrameworkSearchRequest(It.IsAny<Action<FrameworkSearchSession>>()), Times.Once);

            capturedAction.Should().NotBeNull();
            capturedAction(sessionModel); 
            sessionModel.SelectedReprintReasons.Should().BeEquivalentTo(vm.SelectedReprintReasons);
            sessionModel.TicketNumber.Should().Be(vm.TicketNumber);
            sessionModel.OtherReason.Should().Be(vm.OtherReason);
        }
    }
}