﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests
{
    public class WhenUserLoadsAmendsRecipientPage : CertificateAmendQueryBase
    {
        private IActionResult _result;
        private CertificateRecipientViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {
            var certificateApprenticeDetailsController =
                new CertificateRecipientController(MockedLogger.Object,
                    MockHttpContextAccessor.Object,
                    ApiClient);
            _result = certificateApprenticeDetailsController.Recipient(Certificate.Id, true).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            _viewModelResponse = result.Model as CertificateRecipientViewModel;
        }

        [Test]
        public void ThenShouldReturnValidContactName()
        {
            _viewModelResponse.Id.Should().Be(Certificate.Id);
            _viewModelResponse.Name.Should().Be(CertificateData.ContactName);

        }

        [Test]
        public void ThenShouldReturnValidContactDepartmenr()
        {
            _viewModelResponse.Dept.Should().Be(CertificateData.Department);
        }        
    }
}

