using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AdminService.Web.UnitTests.ViewModels
{
    [TestFixture]
    public class WhenDisplayingFrameworkCertificateStatusDates
    {
        private FrameworkLearnerDetailsViewModel _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new FrameworkLearnerDetailsViewModel
            {
                Learner = new GetFrameworkLearnerResponse
                {
                    CertificatePrintRequestedAt = new DateTime(2026, 10, 1),
                    CertificatePrintStatusAt = new DateTime(2026, 10, 8),
                    CertificateLastUpdatedAt = new DateTime(2026, 10, 9)
                }
            };
        }

        [TestCase(CertificateStatus.PrintRequested)]
        [TestCase(CertificateStatus.Reprint)]
        public void ThenQueuedRequestsShowTheRequestDate(string status)
        {
            // Arrange
            _sut.Learner.CertificateStatus = status;

            // Act
            var result = _sut.CertificateStatusDateValue;

            // Assert
            result.Should().Be(new DateTime(2026, 10, 1));
        }

        [TestCase(CertificateStatus.SentToPrinter)]
        [TestCase(CertificateStatus.Printed)]
        [TestCase(CertificateStatus.Delivered)]
        [TestCase(CertificateStatus.NotDelivered)]
        public void ThenPrinterStatusesShowTheEventDate(string status)
        {
            // Arrange
            _sut.Learner.CertificateStatus = status;

            // Act
            var result = _sut.CertificateStatusDateValue;

            // Assert
            result.Should().Be(new DateTime(2026, 10, 8));
        }

        [TestCase(CertificateStatus.PrintRequested)]
        [TestCase(CertificateStatus.Reprint)]
        [TestCase(CertificateStatus.Delivered)]
        public void AndPrintingDateIsMissingThenTheUpdateDateIsNotShown(string status)
        {
            // Arrange
            _sut.Learner.CertificateStatus = status;
            _sut.Learner.CertificatePrintRequestedAt = null;
            _sut.Learner.CertificatePrintStatusAt = null;

            // Act
            var result = _sut.CertificateStatusDateValue;

            // Assert
            result.Should().BeNull();
        }
    }
}
