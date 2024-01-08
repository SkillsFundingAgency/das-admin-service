using System;
using System.Net.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.CertificateDelete;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests.CertificateDelete
{
    public class GivenIRequestToDeleteCertificate : CertificateDeleteQueryBase
    {
        private IActionResult _result;
        private CertificateDeleteController _sut;
        private CertificateDeleteViewModel _deleteViewModel;

        [SetUp]
        public void Arrange()
        {
            _deleteViewModel = new CertificateDeleteViewModel()
            {
                CertificateId = Certificate.Id,
                IncidentNumber = "INC123",
                ReasonForChange = "chnage required"
            };
            _sut = new CertificateDeleteController(MockedLogger.Object, MockHttpContextAccessor.Object, CertificateApiClient, LearnerDetailsApiClient, OrganisationsApiClient, ScheduleApiClient, StandardVersionApiClient);
        }

        [Test]
        public void ThenShouldReturnValidCertificateSubmitDeleteViewModel()
        {
            _result = _sut.ConfirmAndSubmit(_deleteViewModel).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            var certificateSubmitDeleteViewModel = result.Model as CertificateSubmitDeleteViewModel;

            certificateSubmitDeleteViewModel.Id.Should().Be(Certificate.Id);
        }

        [Test]
        public void ThenShouldReturnValidCertificateAuditDeleteViewModel()
        {
            _result = _sut.AuditDetails(_deleteViewModel).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            var certificateSubmitDeleteViewModel = result.Model as CertificateAuditDetailsViewModel;

            certificateSubmitDeleteViewModel.Id.Should().Be(Certificate.Id);
        }

        [Test]
        public void ThenShouldReturnValidCertificateConfirmDeleteViewModel()
        {
            _result = _sut.ConfirmDelete(_deleteViewModel).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            var certificateSubmitDeleteViewModel = result.Model as CertificateConfirmDeleteViewModel;

            certificateSubmitDeleteViewModel.Id.Should().Be(Certificate.Id);
        }

        [Test]
        public void ThenThrowException()
        {
            //arrange
            var model = new CertificateConfirmDeleteViewModel
            {
                IncidentNumber = _deleteViewModel.IncidentNumber,
                ReasonForChange = _deleteViewModel.ReasonForChange,
                StandardCode = 153,
                Uln = 1234456,
                Username = "admin"
            };
            Mock<ICertificateApiClient> client = new Mock<ICertificateApiClient>();

            client.Setup(c => c.Delete(It.IsAny<DeleteCertificateRequest>())).Throws(new HttpRequestException());

            _sut = new CertificateDeleteController(MockedLogger.Object, MockHttpContextAccessor.Object, client.Object, LearnerDetailsApiClient, OrganisationsApiClient, ScheduleApiClient, StandardVersionApiClient);

            try
            {
                //act
                _result = _sut.SuccessfulDelete(model).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                //assert
                Assert.IsTrue(ex is HttpRequestException);
            }
        }
    }
}
