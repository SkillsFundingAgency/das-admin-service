﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Tests.MockedObjects
{
    public class MockedOrganisationsApiClient
    {

        public static IOrganisationsApiClient Setup(Mock<ILogger<OrganisationsApiClient>> mockedApiClientLogger)
        {
            var standardOrganisartionSummaries = new List<OrganisationStandardSummary>
            {
                new OrganisationStandardSummary
                {
                    StandardReference = "ST0093"
                },
                new OrganisationStandardSummary
                {
                    StandardReference = "ST0092"
                },
                new OrganisationStandardSummary
                {
                    StandardReference = "ST0091"
                }
            };


            var standards = new List<OrganisationStandardSummary>
            {
                new OrganisationStandardSummary
                {
                    Id = 91
                },
                new OrganisationStandardSummary
                {
                    Id = 92
                },
                new OrganisationStandardSummary
                {
                    Id = 93
                },
                new OrganisationStandardSummary
                {
                    Id = 94
                },
                new OrganisationStandardSummary
                {
                    Id = 95
                },
            };

            var mockHttp = new MockHttpMessageHandler();
            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("https://findapprenticeshiptraining-api.sfa.bis.gov.uk/");

            mockHttp.When($"/api/ao/assessment-organisations/EPA00001/standards")
                .Respond("application/json", JsonConvert.SerializeObject(standardOrganisartionSummaries));

            mockHttp.When($"https://findapprenticeshiptraining-api.sfa.bis.gov.uk/standards")
                .Respond("application/json", JsonConvert.SerializeObject(standards));

            var webConfigMock = new Mock<IWebConfiguration>();
            var hostMock = new Mock<IHostingEnvironment>();
            hostMock
                .Setup(m => m.EnvironmentName)
                .Returns(EnvironmentName.Development);

            var mockTokenService = new Mock<TokenService>(webConfigMock.Object, hostMock.Object);

            var apiBaseLogger = new Mock<ILogger<ApiClientBase>>();

            var apiClient = new OrganisationsApiClient(client, mockTokenService.Object, apiBaseLogger.Object);

            return apiClient;
        }

    }
}
