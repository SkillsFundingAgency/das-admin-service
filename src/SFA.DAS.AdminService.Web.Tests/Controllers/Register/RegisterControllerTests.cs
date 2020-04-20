﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{    
    [TestFixture]
    public class RegisterControllerTests : RegisterBase
    {
        [SetUp]
        public void Arrange()
        {
            Sut = new RegisterController(ControllerSession.Object, ApiClient.Object, ApplyApiClient.Object, ContactsApiClient.Object, StandardServiceClient.Object, Env.Object);
        }

        [Test]
        public async Task GivenEditOrganisationStandard_WhenEditingOrganisationStandard_ThenReturnContacts()
        {
            //Act
            var actionResult = await Sut.EditOrganisationStandard(organisationStandardId);

            //Assert
            ContactsApiClient.Verify(c => c.GetAllContactsForOrganisation(OrganisationOneOrganisationId, null), Times.Once);
            var result = actionResult as ViewResult;
            var registerViewAndEditOrganisationStandardViewModel = result.Model as RegisterViewAndEditOrganisationStandardViewModel;
            registerViewAndEditOrganisationStandardViewModel.Contacts.Should().HaveCount(2);
        }
    }
}
