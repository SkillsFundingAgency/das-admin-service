using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.AutoMapperProfiles;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading.Tasks;
using System;
using SFA.DAS.AdminService.Web.Orchestrators;

namespace SFA.DAS.AdminService.Web.Tests.Orchestrators
{
    public class UserViewModelOrchestratorTests
    {
        private readonly Mock<IOrganisationsApiClient> _organisationsApiClient;
        private readonly Mock<IContactsApiClient> _contactsApiClient;
        private readonly IMapper _mapper;

        public UserViewModelOrchestratorTests()
        {
            _organisationsApiClient = new Mock<IOrganisationsApiClient>();
            _contactsApiClient = new Mock<IContactsApiClient>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile<AutoMapperMappings>();
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;
        }

        [Test]
        public async Task User_Fields_Are_Mapped_Correctly_And_The_Other_Fields_Are_Ignored()
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
                OrganisationId = new Guid()
            };

            var organisation = new Organisation();

            _contactsApiClient.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(contactResponse);
            _organisationsApiClient.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(organisation);

            var sut = new UserViewModelOrchestrator(_contactsApiClient.Object, _organisationsApiClient.Object, _mapper);

            //Act

            var result = await sut.GetUserViewModel(new Guid());

            //Assert

            Assert.Multiple(() =>
            {
                Assert.That(() => result.EditPrivilegesViewModel, Throws.ArgumentNullException);
                Assert.AreEqual(contactResponse.Id, result.ContactId);
                Assert.AreEqual(contactResponse.Title, result.Title);
                Assert.AreEqual(contactResponse.GivenNames, result.GivenNames);
                Assert.AreEqual(contactResponse.FamilyName, result.FamilyName);
                Assert.AreEqual(contactResponse.Email, result.Email);
                Assert.AreEqual(contactResponse.PhoneNumber, result.PhoneNumber);
                Assert.AreEqual(contactResponse.Status, result.Status);
                Assert.IsNull(result.AllPrivilegeTypes);
                Assert.IsEmpty(result.ActionRequired);
                Assert.IsNull(result.AssignedPrivileges);
                Assert.IsNull(result.EndPointAssessorOrganisationId);
            });
        }
    }
}
