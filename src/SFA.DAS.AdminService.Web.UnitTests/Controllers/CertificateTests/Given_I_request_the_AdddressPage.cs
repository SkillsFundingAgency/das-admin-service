﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests
{
    public class Given_I_request_the_AdddressPage : CertificateAmendQueryBase
    {     
        private IActionResult _result;
        private CertificateAddressViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {
             var certificateAddressController = new CertificateAddressController(MockedLogger.Object, MockHttpContextAccessor.Object, CertificateApiClient, LearnerDetailsApiClient, OrganisationsApiClient, ScheduleApiClient, StandardVersionApiClient);
            _result = certificateAddressController.Address(Certificate.Id).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            _viewModelResponse = result.Model as CertificateAddressViewModel;
        }

        [Test]
        public void ThenShouldReturnValidAdddressLine1()
        {         
            _viewModelResponse.Id.Should().Be(Certificate.Id);
            _viewModelResponse.AddressLine1.Should().Be(Certificate.CertificateData.ContactAddLine1);           
        }

        [Test]
        public void ThenShouldReturnValidAdddressLine2()
        {                      
            _viewModelResponse.AddressLine2.Should().Be(Certificate.CertificateData.ContactAddLine2);
       }

        [Test]
        public void ThenShouldReturnValidAdddressLine3()
        {                       
            _viewModelResponse.Id.Should().Be(Certificate.Id);
            _viewModelResponse.AddressLine3.Should().Be(Certificate.CertificateData.ContactAddLine3);            
        }
        [Test]
        public void ThenShouldReturnValidCity()
        {                     
            _viewModelResponse.City.Should().Be(Certificate.CertificateData.ContactAddLine4);            
        }

        [Test]
        public void ThenShouldReturnValidPostcode()
        {                     
            _viewModelResponse.Postcode.Should().Be(Certificate.CertificateData.ContactPostCode);
        }
    }   
}
