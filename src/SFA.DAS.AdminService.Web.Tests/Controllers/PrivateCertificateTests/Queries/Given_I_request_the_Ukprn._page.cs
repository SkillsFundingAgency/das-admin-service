using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AdminService.Web.Controllers.Private;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.PrivateCertificateTests.Queries
{
    public class Given_I_request_the_ukprn_page : CertificateQueryBase
    {
        private IActionResult _result;
        private CertificateUkprnViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {
            var certificatePrivateProviderUkprnController =
                new CertificatePrivateProviderUkprnController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockApiClientFactory.Object
                    );
          

            _result = certificatePrivateProviderUkprnController.Ukprn(Certificate.Id, true).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            _viewModelResponse = result.Model as CertificateUkprnViewModel;
        }

        [Test]
        public void ThenShouldReturnFirstName()
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(Certificate.CertificateData);

            _viewModelResponse.Id.Should().Be(Certificate.Id);
            _viewModelResponse.Ukprn.Should().Be(Certificate.ProviderUkPrn.ToString());
        }
    }
}

