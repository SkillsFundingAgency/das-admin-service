﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests
{
    public class WhenUserLoadsFamilyNamePage : CertificateAmendQueryBase
    {     
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            var certificateNamesController = new CertificateNamesController(MockedLogger.Object, MockHttpContextAccessor.Object, CertificateApiClient, LearnerDetailsApiClient, OrganisationsApiClient, ScheduleApiClient, StandardVersionApiClient);
            _result = certificateNamesController.FamilyName(Certificate.Id).GetAwaiter().GetResult();
        }

        [Test]
        public void ThenShouldReturnValidViewModel()
        {
            var result = _result as ViewResult;
            var model = result.Model as CertificateFamilyNameViewModel;

            model.Id.Should().Be(Certificate.Id);
            model.FamilyName.Should().Be(Certificate.CertificateData.LearnerFamilyName);            
        }
    }   
}
