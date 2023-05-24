using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests
{
    public class WhenUserLoadsGivenNamesPage : CertificateAmendQueryBase
    {     
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            var certificateNamesController = new CertificateNamesController(MockedLogger.Object, MockHttpContextAccessor.Object, ApiClient);
            _result = certificateNamesController.GivenNames(Certificate.Id).GetAwaiter().GetResult();
        }

        [Test]
        public void ThenShouldReturnValidViewModel()
        {
            var result = _result as ViewResult;
            var model = result.Model as CertificateGivenNamesViewModel;

            var certificateData = JsonConvert.DeserializeObject<CertificateData>(Certificate.CertificateData);

            model.Id.Should().Be(Certificate.Id);
            model.GivenNames.Should().Be(certificateData.LearnerGivenNames);            
        }
    }   
}
