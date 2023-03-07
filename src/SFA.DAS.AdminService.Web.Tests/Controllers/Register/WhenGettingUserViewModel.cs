using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.AutoMapperProfiles;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class WhenGettingUserViewModel
    {
        private readonly Mock<IOrganisationsApiClient> _organisationsApiClient;
        private readonly Mock<IContactsApiClient> _contactsApiClient;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor httpContextAccessor;

        public WhenGettingUserViewModel()
        {
            _organisationsApiClient = new Mock<IOrganisationsApiClient>();
            _contactsApiClient = new Mock<IContactsApiClient>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile<AutoMapperMappings>();
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;

            httpContextAccessor = new HttpContextAccessor();
        }

        [Test]
        public async Task Then_The_User_Fields_Are_Mapped_Correctly_And_The_Other_Fields_Are_Ignored()
        {
            // Arrange

            var contactResponse = new ContactResponse
            {
                Id = new Guid(),
                Title = "Title",
                GivenNames = "GivenNames",
                FamilyName = "FamilyName",
                Email = "Email",
                PhoneNumber = "1234567890",
                Status = "Status",
                OrganisationId= new Guid()
            };

            var organisation = new Organisation
            {

            };

            _contactsApiClient.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(contactResponse);
            _organisationsApiClient.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(organisation);

            var sut = new RegisterUserController(_contactsApiClient.Object, httpContextAccessor, _organisationsApiClient.Object, _mapper);

            //Act

            var result = await sut.EditPermissions(new Guid()) as ViewResult;

            var actual = result.Model as RegisterViewAndEditUserViewModel;

            Assert.Multiple(() =>
            {
                Assert.That(() => actual.EditPrivilegesViewModel, Throws.ArgumentNullException);
                Assert.AreEqual(contactResponse.Id, actual.ContactId);
                Assert.AreEqual(contactResponse.Title, actual.Title);
                Assert.AreEqual(contactResponse.GivenNames, actual.GivenNames);
                Assert.AreEqual(contactResponse.FamilyName, actual.FamilyName);
                Assert.AreEqual(contactResponse.Email, actual.Email);
                Assert.AreEqual(contactResponse.PhoneNumber, actual.PhoneNumber);
                Assert.AreEqual(contactResponse.Status, actual.Status);
                Assert.IsNull(actual.AllPrivilegeTypes);
                Assert.IsEmpty(actual.ActionRequired);
                Assert.IsNull(actual.AssignedPrivileges);
                Assert.IsNull(actual.EndPointAssessorOrganisationId);
            });
        }
    }
}
