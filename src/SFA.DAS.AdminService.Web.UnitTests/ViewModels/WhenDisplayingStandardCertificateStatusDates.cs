using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.ViewModels.Shared;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AdminService.Web.UnitTests.ViewModels
{
    [TestFixture]
    public class WhenDisplayingStandardCertificateStatusDates
    {
        private LearnerDetailViewModel _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new LearnerDetailViewModel
            {
                Learner = new LearnerDetailResult
                {
                    PrintRequestedAt = new DateTime(2026, 10, 1),
                    PrintStatusAt = new DateTime(2026, 10, 8),
                    LastUpdatedAt = new DateTime(2026, 10, 9)
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
            _sut.Learner.PrintRequestedAt = null;
            _sut.Learner.PrintStatusAt = null;

            // Act
            var result = _sut.CertificateStatusDateValue;

            // Assert
            result.Should().BeNull();
        }
    }
}
