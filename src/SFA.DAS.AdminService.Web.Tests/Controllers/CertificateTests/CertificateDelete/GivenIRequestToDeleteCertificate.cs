using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.CertificateDelete;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests.CertificateDelete
{
    public class GivenIRequestToDeleteCertificate : CertificateDeleteQueryBase
    {
        private IActionResult _result;
        private CertificateDeleteController _sut;

        [SetUp]
        public void Arrange()
        {
            _sut = new CertificateDeleteController(MockedLogger.Object, MockHttpContextAccessor.Object, ApiClient, CertificateApiClient);
        }

        [Test]
        public void ThenShouldReturnValidCertificateSubmitDeleteViewModel()
        {
            _result = _sut.ConfirmAndSubmit(Certificate.Id, "searchstring", 0).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            var certificateSubmitDeleteViewModel = result.Model as CertificateSubmitDeleteViewModel;

            certificateSubmitDeleteViewModel.Id.Should().Be(Certificate.Id);
        }

        [Test]
        public void ThenShouldReturnValidCertificateAuditDeleteViewModel()
        {
            _result = _sut.AuditDetails(Certificate.Id, "reasonForChange", "INC123").GetAwaiter().GetResult();

            var result = _result as ViewResult;
            var certificateSubmitDeleteViewModel = result.Model as CertificateAuditDetailsViewModel;

            certificateSubmitDeleteViewModel.Id.Should().Be(Certificate.Id);
        }

        [Test]
        public void ThenShouldReturnValidCertificateConfirmDeleteViewModel()
        {
            _result = _sut.ConfirmDelete(Certificate.Id, "reasonForChange", "INC123").GetAwaiter().GetResult();

            var result = _result as ViewResult;
            var certificateSubmitDeleteViewModel = result.Model as CertificateConfirmDeleteViewModel;

            certificateSubmitDeleteViewModel.Id.Should().Be(Certificate.Id);
        }
    }
}
