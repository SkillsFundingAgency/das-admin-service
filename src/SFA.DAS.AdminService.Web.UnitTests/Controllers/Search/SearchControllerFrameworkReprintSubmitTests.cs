using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Models.Search;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class SearchControllerFrameworkReprintSubmitTests : SearchControllerTestsBase
    {
        private SearchController _sut;
        private FrameworkSearchSession _session;

        [SetUp]
        public void SetUp()
        {
            _sut = _controller;
            _session = new FrameworkSearchSession
            {
                SelectedFrameworkLearnerId = Guid.NewGuid(),
                FirstName = "Case",
                LastName = "Examp",
                TicketNumber = "INC-123",
                SelectedReprintReasons = new() { "LostOrDamaged" },
                AddressLine1 = "1 Example Road",
                Postcode = "CV1 2WT"
            };

            _sessionServiceMock.Setup(service => service.SessionFrameworkSearch).Returns(_session);
            _learnerDetailsApiClientMock
                .Setup(client => client.GetFrameworkLearner(_session.SelectedFrameworkLearnerId.Value, false))
                .ReturnsAsync(new GetFrameworkLearnerResponse
                {
                    ApprenticeForename = "Casey",
                    ApprenticeSurname = "Example"
                });
        }

        [Test]
        public async Task WhenReprintIsRejected_RedirectsToAddressAndPreservesTheSession()
        {
            // Arrange
            _certificateApiClientMock
                .Setup(client => client.ReprintFrameworkCertificate(It.IsAny<ReprintFrameworkCertificateCommand>()))
                .ThrowsAsync(new HttpRequestException("Assessor API error", null, HttpStatusCode.BadRequest));

            // Act
            var result = await _sut.FrameworkReprintSubmit();

            // Assert
            var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ActionName.Should().Be(nameof(SearchController.FrameworkReprintAddress));
            redirect.RouteValues["backToCheckAnswers"].Should().Be(true);
            _sut.ModelState["AddressLine1"].Errors.Should()
                .ContainSingle(error => error.ErrorMessage == "Check the delivery details before requesting a reprint.");
            _sessionServiceMock.Verify(service => service.ClearFrameworkSearchRequest(), Times.Never);
        }

        [Test]
        public async Task WhenReprintSucceeds_UsesTheSelectedLearnersNameAndClearsTheSession()
        {
            // Arrange
            _certificateApiClientMock
                .Setup(client => client.ReprintFrameworkCertificate(It.IsAny<ReprintFrameworkCertificateCommand>()))
                .ReturnsAsync(new FrameworkCertificate());

            // Act
            var result = await _sut.FrameworkReprintSubmit();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Subject.ActionName
                .Should().Be(nameof(SearchController.FrameworkReprintSubmitted));
            _certificateApiClientMock.Verify(client => client.ReprintFrameworkCertificate(
                It.Is<ReprintFrameworkCertificateCommand>(command =>
                    command.FrameworkLearnerId == _session.SelectedFrameworkLearnerId.Value
                    && command.ContactName == "Casey Example"
                    && command.ContactAddLine1 == _session.AddressLine1
                    && command.ContactPostcode == _session.Postcode)), Times.Once);
            _sessionServiceMock.Verify(service => service.ClearFrameworkSearchRequest(), Times.Once);
        }
    }
}
