using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using SFA.DAS.AdminService.Web.Models.Search;
using SFA.DAS.AdminService.Web.ViewModels.Search;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class SearchControllerFrameworkAddressTests : SearchControllerTestsBase 
    {
        [Test]
        public void Address_SessionIsNull_RedirectsToIndex()
        {
            // Arrange
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns((FrameworkSearchSession)null);

            // Act
            var result = _controller.FrameworkReprintAddress();

            // Assert
            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        public void Address_SelectedResultIsNull_RedirectsToIndex()
        {
            // Arrange
            var sessionModel = new FrameworkSearchSession { SelectedFrameworkLearnerId = null };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            // Act
            var result = _controller.FrameworkReprintAddress();

            // Assert
            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Test]
        public void Address_ValidSessionAndResults_ReturnsViewWithCorrectModel()
        {
            // Arrange
            var sessionModel = new FrameworkSearchSession
            {
                FrameworkResults = new List<FrameworkResultViewModel> { new FrameworkResultViewModel() },
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = DateTime.Now.AddYears(-20),
                SelectedFrameworkLearnerId = Guid.NewGuid(),
                AddressLine1 = "43 West Road",
                AddressLine2 = "Townlandish",
                County = "Portlanshire",
                TownOrCity ="Leeds",
                Postcode ="LS12 3RF"
            };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);
            var mappedViewModel = new FrameworkReprintAddressViewModel
            { 
                AddressLine1 = sessionModel.AddressLine1, 
                AddressLine2 = sessionModel.AddressLine2, 
                County = sessionModel.County,
                TownOrCity = sessionModel.TownOrCity, 
                Postcode = sessionModel.Postcode
            };
            _mapperMock.Setup(m => m.Map<FrameworkReprintAddressViewModel>(sessionModel)).Returns(mappedViewModel);

            // Act
            var result = _controller.FrameworkReprintAddress();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<FrameworkReprintAddressViewModel>().Subject;
            model.Should().BeEquivalentTo(mappedViewModel);
            _mapperMock.Verify(m => m.Map<FrameworkReprintAddressViewModel>(sessionModel), Times.Once);
        }

        [Test]
        public void UpdateAddress_ValidModelState_ClearsSessionAndRedirects()
        {
            // Arrange
            var vm = new FrameworkReprintAddressViewModel 
            { 
                AddressLine1 = "69 Southend Rd",
                AddressLine2 = "Wickford",
                TownOrCity = "Essex",
                County = "Essex",
                Postcode = "SS11 8DX"
            };
            _controller.ModelState.Clear();
            _controller.ModelState.IsValid.Should().BeTrue();

            var sessionModel = new FrameworkSearchSession
            {
                SelectedFrameworkLearnerId = Guid.NewGuid()
            };
            _sessionServiceMock.Setup(s => s.SessionFrameworkSearch).Returns(sessionModel);

            Action<FrameworkSearchSession> capturedAction = null;
            _sessionServiceMock.Setup(s => s.UpdateFrameworkSearchRequest(It.IsAny<Action<FrameworkSearchSession>>()))
                .Callback<Action<FrameworkSearchSession>>(action => capturedAction = action);

            // Act
            var result = _controller.FrameworkReprintAddress(vm);

            // Assert
            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("FrameworkReprintCheckDetails");
            _sessionServiceMock.Verify(s => s.UpdateFrameworkSearchRequest(It.IsAny<Action<FrameworkSearchSession>>()), Times.Once);

            capturedAction.Should().NotBeNull();
            capturedAction(sessionModel);
            sessionModel.AddressLine1.Should().Be(vm.AddressLine1);
            sessionModel.AddressLine2.Should().Be(vm.AddressLine2);
            sessionModel.TownOrCity.Should().Be(vm.TownOrCity);
            sessionModel.County.Should().Be(vm.County);
            sessionModel.Postcode.Should().Be(vm.Postcode);
        }

        [Test]
        public void UpdateAddress_InvalidModelState_UpdatesSessionAndRedirects()
        {
            // Arrange
            var vm = new FrameworkReprintAddressViewModel
            {
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
            var result = _controller.FrameworkReprintAddress(vm);

            // Assert
            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be("FrameworkReprintAddress");
            _sessionServiceMock.Verify(s => s.UpdateFrameworkSearchRequest(It.IsAny<Action<FrameworkSearchSession>>()), Times.Once);

            capturedAction.Should().NotBeNull();
            capturedAction(sessionModel); 
            sessionModel.AddressLine1.Should().Be(vm.AddressLine1);
            sessionModel.AddressLine2.Should().Be(vm.AddressLine2);
            sessionModel.TownOrCity.Should().Be(vm.TownOrCity);
            sessionModel.County.Should().Be(vm.County);
            sessionModel.Postcode.Should().Be(vm.Postcode);
        }
    }
}