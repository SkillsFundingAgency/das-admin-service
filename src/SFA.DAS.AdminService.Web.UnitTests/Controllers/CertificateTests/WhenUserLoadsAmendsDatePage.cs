using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests
{
    public class WhenUserLoadsAmendsDatePage : CertificateAmendQueryBase
    {
        private IActionResult _result;
        private CertificateDateViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {
            var certificateDateViewModelValidator = new CertificateDateViewModelValidator();
            var certificateApprenticeDetailsController =
                new CertificateDateController(MockedLogger.Object, MockHttpContextAccessor.Object, certificateDateViewModelValidator,
                    CertificateApiClient, LearnerDetailsApiClient, OrganisationsApiClient, ScheduleApiClient, StandardVersionApiClient);
            _result = certificateApprenticeDetailsController.Date(Certificate.Id).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            _viewModelResponse = result.Model as CertificateDateViewModel;
        }

        [Test]
        public void ThenShouldReturnValidDay()
        {
            _viewModelResponse.Id.Should().Be(Certificate.Id);
            _viewModelResponse.Day.PadLeft(2, '0').Should().Be(Certificate.CertificateData.AchievementDate.Value.ToString("dd"));

        }

        [Test]
        public void ThenShouldReturnValidMonth()
        {
            _viewModelResponse.Month.PadLeft(2, '0').Should().Be(Certificate.CertificateData.AchievementDate.Value.ToString("MM"));
        }

        [Test]
        public void ThenShouldReturnValidYear()
        {
            _viewModelResponse.Year.Should().Be(Certificate.CertificateData.AchievementDate.Value.ToString("yyyy"));
        }
    }
}

