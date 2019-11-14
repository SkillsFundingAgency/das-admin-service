using Microsoft.AspNetCore.Hosting;
using Moq;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Extensions.TagHelpers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class RegisterBase
    {
        protected Mock<IPagingState> ApprovedStandards;
        protected Mock<IControllerSession> ControllerSession;
        protected Mock<IApiClient> ApiClient;
        protected Mock<IApplicationApiClient> ApplyApiClient;
        protected Mock<IContactsApiClient> ContactsApiClient;
        protected Mock<IStandardServiceClient> StandardServiceClient;
        protected Mock<IHostingEnvironment> Env;

        protected Guid OrganisationOneId = Guid.NewGuid();
        protected string OrganisationOneOrganisationId = "EPA0001";

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
                new OrganisationStandardSummary
                {
                    Id = 1,
                    StandardCollation = new StandardCollation
                    {
                        ReferenceNumber = "ST0001",
                        StandardId = 1,
                        Title = "Gravyboat Maker"
                    },
                    DateStandardApprovedOnRegister = DateTime.Now.AddDays(-100)
                } 
            };

            List<ApplicationSummaryItem> standardApplications = new List<ApplicationSummaryItem>
            {
                new ApplicationSummaryItem
                {
                    ApplicationId = Guid.NewGuid(),
                    StandardReference = "ST0002",
                    StandardName = "Senior Gravyboat Maker"
                }
            };

            PaginatedList<ApplicationSummaryItem> standardApplicationPaginatedList 
                = new PaginatedList<ApplicationSummaryItem>(standardApplications, standardApplications.Count, 1, short.MaxValue, 6);
            
            ApprovedStandards = new Mock<IPagingState>();
            ApprovedStandards.Setup(p => p.ItemsPerPage).Returns(10);
            ApprovedStandards.Setup(p => p.PageIndex).Returns(1);
            ApprovedStandards.Setup(p => p.SortColumn).Returns(OrganisationStandardSortColumn.StandardName);
            ApprovedStandards.Setup(p => p.SortDirection).Returns(SortOrder.Asc);

            ControllerSession = new Mock<IControllerSession>();
            ControllerSession.Setup(p => p.Register_SessionValid).Returns(true);
            ControllerSession.Setup(p => p.Register_ApprovedStandards).Returns(ApprovedStandards.Object);

            ApiClient = new Mock<IApiClient>();
            ApiClient.Setup(p => p.GetEpaOrganisation(OrganisationOneOrganisationId)).ReturnsAsync(organisation);
            ApiClient.Setup(p => p.GetOrganisationTypes()).ReturnsAsync(organisationTypes);
            ApiClient.Setup(p => p.GetEpaOrganisationStandards(OrganisationOneOrganisationId)).ReturnsAsync(organisationStandards);

            ApplyApiClient = new Mock<IApplicationApiClient>();
            ApplyApiClient.Setup(p => p.GetStandardApplications(It.IsAny<StandardApplicationsRequest>())).ReturnsAsync(standardApplicationPaginatedList);

            ContactsApiClient = new Mock<IContactsApiClient>();
            ContactsApiClient.Setup(p => p.GetAllContactsForOrganisation(OrganisationOneOrganisationId, false)).ReturnsAsync(contacts);
            ContactsApiClient.Setup(p => p.GetAllContactsForOrganisationIncludePrivileges(OrganisationOneOrganisationId, true)).ReturnsAsync(users);
            ContactsApiClient.Setup(p => p.GetAllContactsWhoCanBePrimaryForOrganisation(OrganisationOneOrganisationId)).ReturnsAsync(contactsWhoCanBePrimary);

            StandardServiceClient = new Mock<IStandardServiceClient>();
            Env = new Mock<IHostingEnvironment>();
        }
    }
}
