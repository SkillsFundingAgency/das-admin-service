using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.JsonData;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.UnitTests.Models
{
    public class StaffBatchSearchViewModelTests
    {
        [TestFixture]
        public class StaffBatchSearchResultViewModelTests
        {

            [Test]
            public void FormatTrainingCourse_HandlesNullCertificateData()
            {
                // Arrange
                var searchResult = new StaffBatchSearchResult
                {
                    StandardCode = 0,
                    CertificateData = null
                };

                // Act
                var viewModel = new StaffBatchSearchResultViewModel(searchResult);

                // Assert
                viewModel.TrainingCourse.Should().BeNullOrEmpty();
            }

            [Test]
            public void FormatTrainingCourse_HandlesStandardCertificateData()
            {
                // Arrange
                var searchResult = new StaffBatchSearchResult
                {
                    StandardCode = 100,
                    CertificateData = new CertificateData { StandardName = "Standard Name" }
                };

                // Act
                var viewModel = new StaffBatchSearchResultViewModel(searchResult);

                // Assert
                Assert.That(viewModel.TrainingCourse, Is.EqualTo("Standard Name(100)"));
            }

            [Test]
            public void FormatTrainingCourse_HandlesFrameworkCertificateData()
            {
                // Arrange
                var searchResult = new StaffBatchSearchResult
                {
                    StandardCode = 0,
                    CertificateData = new CertificateData { FrameworkName = "Framework 1", TrainingCode = "FW1" }
                };

                // Act
                var viewModel = new StaffBatchSearchResultViewModel(searchResult);

                // Assert
                Assert.That(viewModel.TrainingCourse, Is.EqualTo("Framework 1(FW1)"));
            }

            [Test]
            public void GenerateRouteData_HandlesNullBatchNumber()
            {
                // Arrange
                var searchResult = new StaffBatchSearchResult
                {
                    StandardCode = 100,
                    Uln = 1234567890,
                    CertificateReference = "Ref123",
                    BatchNumber = null
                };

                // Act
                var viewModel = new StaffBatchSearchResultViewModel(searchResult);

                // Assert
                Assert.That(viewModel.ViewCertificateLinkRouteData, Is.EqualTo(new Dictionary<string, string>
                {
                    { "stdcode", "100" },
                    { "uln", "1234567890" },
                    { "searchstring", "Ref123" },
                    { "batchNumber", null }
                }));
            }

            [Test]
            public void DisplayUln_HandlesEmptyUln()
            {
                //Arrange
                var searchResult = new StaffBatchSearchResult
                {
                    Uln = 0
                };

                //Act
                var viewModel = new StaffBatchSearchResultViewModel(searchResult);

                //Assert
                Assert.That(viewModel.DisplayUln, Is.EqualTo("None"));
            }
        }
    }
}
