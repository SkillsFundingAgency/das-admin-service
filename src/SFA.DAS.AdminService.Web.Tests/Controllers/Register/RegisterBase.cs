using Microsoft.AspNetCore.Hosting;
using Moq;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class RegisterBase
    {
        protected Mock<IApiClient> ApiClient;
        protected Mock<IContactsApiClient> ContactsApiClient;
        protected Mock<IStandardServiceClient> StandardServiceClient;
        protected Mock<IHostingEnvironment> Env;

        protected Guid OrganisationOneId = Guid.NewGuid();
        protected string OrganisationOneOrganisationId = "EPA0001";
        protected const int organisationStandardId = 1;

        protected Guid ContactOneId = Guid.NewGuid();
        protected Guid ContactTwoId = Guid.NewGuid();

        protected Guid UserOneId = Guid.NewGuid();
        protected Guid UserTwoId = Guid.NewGuid();
        protected Guid UserThreeId = Guid.NewGuid();

        protected RegisterController Sut;
       
        public RegisterBase()
        {
            EpaOrganisation organisation = new EpaOrganisation
            {
                Id = OrganisationOneId,
                OrganisationId = OrganisationOneOrganisationId,
                OrganisationTypeId = 1
            };

            List<OrganisationType> organisationTypes = new List<OrganisationType>
            {
                new OrganisationType
                {
                    Id = 1
                }
            };

            List<ContactResponse> contacts = new List<ContactResponse>
            {
                new ContactResponse { Id = ContactOneId },
                new ContactResponse { Id = ContactTwoId }
            };

            List<ContactIncludePrivilegesResponse> users = new List<ContactIncludePrivilegesResponse>
            {
                new ContactIncludePrivilegesResponse { Contact = new ContactResponse { Id = UserOneId }},
                new ContactIncludePrivilegesResponse { Contact = new ContactResponse { Id = UserTwoId }},
                new ContactIncludePrivilegesResponse { Contact = new ContactResponse { Id = UserThreeId }},
            };

            List<ContactResponse> contactsWhoCanBePrimary = new List<ContactResponse>
            {
                new ContactResponse { Id = ContactOneId },
                new ContactResponse { Id = ContactTwoId },
                new ContactResponse { Id = UserOneId },
                new ContactResponse { Id = UserTwoId },
                new ContactResponse { Id = UserThreeId }
            };

            List<OrganisationStandardSummary> organisationStandards = new List<OrganisationStandardSummary>
            {
                new OrganisationStandardSummary { Id = 1}
            };

            List<DeliveryArea> deliveryAreas = new List<DeliveryArea>
            {
                new DeliveryArea { Id = 1 }
            };

            var organisationStandard = new OrganisationStandard() { OrganisationId = OrganisationOneOrganisationId };

            ApiClient = new Mock<IApiClient>();
            ApiClient.Setup(p => p.GetEpaOrganisation(OrganisationOneOrganisationId)).ReturnsAsync(organisation);
            ApiClient.Setup(p => p.GetOrganisationTypes()).ReturnsAsync(organisationTypes);
            ApiClient.Setup(p => p.GetEpaOrganisationStandards(OrganisationOneOrganisationId)).ReturnsAsync(organisationStandards);
            ApiClient.Setup(p => p.GetOrganisationStandard(organisationStandardId)).ReturnsAsync(organisationStandard);
            ApiClient.Setup(p => p.GetDeliveryAreas()).ReturnsAsync(deliveryAreas);

            ContactsApiClient = new Mock<IContactsApiClient>();
            ContactsApiClient.Setup(p => p.GetAllContactsForOrganisation(OrganisationOneOrganisationId, null)).ReturnsAsync(contacts);            
            ContactsApiClient.Setup(p => p.GetAllContactsForOrganisation(OrganisationOneOrganisationId, false)).ReturnsAsync(contacts);
            ContactsApiClient.Setup(p => p.GetAllContactsForOrganisationIncludePrivileges(OrganisationOneOrganisationId, true)).ReturnsAsync(users);
            ContactsApiClient.Setup(p => p.GetAllContactsWhoCanBePrimaryForOrganisation(OrganisationOneOrganisationId)).ReturnsAsync(contactsWhoCanBePrimary);

            StandardServiceClient = new Mock<IStandardServiceClient>();
            Env = new Mock<IHostingEnvironment>();
        }
    }
}
