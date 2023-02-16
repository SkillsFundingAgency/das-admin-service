using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests
{
    public class Given_I_request_the_options_page : CertificateAmendQueryBase
    {     
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            var certificateGradeController = new CertificateOptionController(MockedLogger.Object, MockHttpContextAccessor.Object, ApiClient);
            _result = certificateGradeController.Option(Certificate.Id).GetAwaiter().GetResult();           
        }

        [Test]
        public void ThenShouldReturnValidViewModel()
        {
            var result = _result as ViewResult;
            var model = result.Model as CertificateOptionViewModel;

            var certificateData = JsonSerializer.Deserialize<CertificateData>(Certificate.CertificateData);

            model.Id.Should().Be(Certificate.Id);
            model.Option.Should().Be(certificateData.CourseOption);            
        }
    }   
}
