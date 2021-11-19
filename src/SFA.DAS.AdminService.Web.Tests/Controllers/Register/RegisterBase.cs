using AutoFixture;
using Microsoft.AspNetCore.Hosting;
using Moq;
using SFA.DAS.AdminService.Common.Extensions.TagHelpers;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Domain.Apply;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
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
        protected const int organisationStandardId = 1;        

        protected Guid ContactOneId = Guid.NewGuid();
        protected Guid ContactTwoId = Guid.NewGuid();

        protected Guid UserOneId = Guid.NewGuid();
        protected Guid UserTwoId = Guid.NewGuid();
        protected Guid UserThreeId = Guid.NewGuid();

        protected OrganisationStandardVersion StandardVersion1;
        protected OrganisationStandardVersion StandardVersion2;

        protected Fixture Fixture;

        protected RegisterController Sut;

        public RegisterBase()
        {
            Fixture = new Fixture();

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

            List<OrganisationStandardSummary> organisationStandards = new List<OrganisationStandardSummary>();
            for (int standard = 0; standard < 20; standard++)
            { 
                organisationStandards.Add(new OrganisationStandardSummary
                {
                    Id = standard,
                    StandardCollation = new StandardCollation
                    {
                        ReferenceNumber = string.Format("ST{0:4}", standard),
                        StandardId = standard,
                        Title = string.Format("{0} Gravyboat Maker", NumberToAlpha(standard / 10))
                    },
                    DateStandardApprovedOnRegister = DateTime.Now.AddDays(-100)
                }); ;
            }

            List<DeliveryArea> deliveryAreas = new List<DeliveryArea>
            {
                new DeliveryArea { Id = 1 }
            };

            var organisationStandard = new OrganisationStandard() { OrganisationId = OrganisationOneOrganisationId, Id = organisationStandardId };

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

            int itemsPerPage = 10;
            int pageIndex = 1;
            string sortColumn = OrganisationStandardSortColumn.StandardName;
            string sortDirection = SortOrder.Asc;

            ApprovedStandards = new Mock<IPagingState>();
            
            ApprovedStandards.SetupGet(s => s.ItemsPerPage).Returns(itemsPerPage);
            ApprovedStandards.SetupSet(p => p.ItemsPerPage = It.IsAny<int>()).Callback<int>(r => 
            { 
                itemsPerPage = r;
                ApprovedStandards.SetupGet(s => s.ItemsPerPage).Returns(itemsPerPage);
            });

            ApprovedStandards.SetupGet(s => s.PageIndex).Returns(pageIndex);
            ApprovedStandards.SetupSet(p => p.PageIndex = It.IsAny<int>()).Callback<int>(r => 
            { 
                pageIndex = r;
                ApprovedStandards.SetupGet(s => s.PageIndex).Returns(pageIndex);
            });

            ApprovedStandards.SetupGet(s => s.SortColumn).Returns(sortColumn);
            ApprovedStandards.SetupSet(p => p.SortColumn = It.IsAny<string>()).Callback<string>(r => 
            { 
                sortColumn = r;
                ApprovedStandards.SetupGet(s => s.SortColumn).Returns(sortColumn);
            });

            ApprovedStandards.SetupGet(s => s.SortDirection).Returns(sortDirection);
            ApprovedStandards.SetupSet(p => p.SortDirection = It.IsAny<string>()).Callback<string>(r => 
            { 
                sortDirection = r;
                ApprovedStandards.SetupGet(s => s.SortDirection).Returns(sortDirection);
            });

            ControllerSession = new Mock<IControllerSession>();
            ControllerSession.Setup(p => p.Register_SessionValid).Returns(true);
            ControllerSession.Setup(p => p.Register_ApprovedStandards).Returns(ApprovedStandards.Object);
            
            ApiClient = new Mock<IApiClient>();
            ApiClient.Setup(p => p.GetEpaOrganisation(OrganisationOneOrganisationId)).ReturnsAsync(organisation);
            ApiClient.Setup(p => p.GetOrganisationTypes()).ReturnsAsync(organisationTypes);
            ApiClient.Setup(p => p.GetEpaOrganisationStandards(OrganisationOneOrganisationId)).ReturnsAsync(organisationStandards);
            ApiClient.Setup(p => p.GetOrganisationStandard(organisationStandardId)).ReturnsAsync(organisationStandard);
            ApiClient.Setup(p => p.GetDeliveryAreas()).ReturnsAsync(deliveryAreas);

            ApplyApiClient = new Mock<IApplicationApiClient>();
            ApplyApiClient.Setup(p => p.GetStandardApplications(It.IsAny<StandardApplicationsRequest>())).ReturnsAsync(standardApplicationPaginatedList);

            ContactsApiClient = new Mock<IContactsApiClient>();
            ContactsApiClient.Setup(p => p.GetAllContactsForOrganisation(OrganisationOneOrganisationId, null)).ReturnsAsync(contacts);            
            ContactsApiClient.Setup(p => p.GetAllContactsForOrganisation(OrganisationOneOrganisationId, false)).ReturnsAsync(contacts);
            ContactsApiClient.Setup(p => p.GetAllContactsForOrganisationIncludePrivileges(OrganisationOneOrganisationId, true)).ReturnsAsync(users);
            ContactsApiClient.Setup(p => p.GetAllContactsWhoCanBePrimaryForOrganisation(OrganisationOneOrganisationId)).ReturnsAsync(contactsWhoCanBePrimary);

            StandardServiceClient = new Mock<IStandardServiceClient>();
            Env = new Mock<IHostingEnvironment>();
        }

        public string NumberToAlpha(long number, bool isLower = false)
        {
            string returnVal = "";
            char c = isLower ? 'a' : 'A';
            while (number >= 0)
            {
                returnVal = (char)(c + number % 26) + returnVal;
                number /= 26;
                number--;
            }

            return returnVal;
        }
    }
}
