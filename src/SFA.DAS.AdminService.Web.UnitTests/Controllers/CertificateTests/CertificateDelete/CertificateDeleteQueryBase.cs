﻿using System;
using System.Security.Claims;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests.CertificateDelete
{
    public class CertificateDeleteQueryBase
    {
        protected Mock<ILogger<CertificateDeleteController>> MockedLogger;
        protected Mock<IHttpContextAccessor> MockHttpContextAccessor;

        protected CertificateApiClient CertificateApiClient;
        protected LearnerDetailsApiClient LearnerDetailsApiClient;
        protected OrganisationsApiClient OrganisationsApiClient;
        protected ScheduleApiClient ScheduleApiClient;
        protected StandardVersionApiClient StandardVersionApiClient;

        protected Certificate Certificate;
        protected CertificateData CertificateData;

        public CertificateDeleteQueryBase()
        {
            MockedLogger = new Mock<ILogger<CertificateDeleteController>>();
            MockHttpContextAccessor = SetupMockedHttpContextAccessor();
            SetupApiClients();
            CertificateData = JsonConvert.DeserializeObject<CertificateData>(Certificate.CertificateData);
        }

        private static Mock<IHttpContextAccessor> SetupMockedHttpContextAccessor()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "jcoxhead")
            }));

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext { User = user };

            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            return mockHttpContextAccessor;
        }

        private void SetupApiClients()
        {
            Certificate = SetupCertificate();

            var mockHttp = new MockHttpMessageHandler();

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost:59022/");

            mockHttp.When($"http://localhost:59022/api/v1/certificates/{Certificate.Id}")
                .Respond("application/json", JsonConvert.SerializeObject(Certificate));

            mockHttp.When($"http://localhost:59022/api/v1/organisations/organisation/{Certificate.OrganisationId}")
                .Respond("application/json", JsonConvert.SerializeObject(Certificate));

            mockHttp.When($"http://localhost:59022/api/v1/organisations/organisation/{Certificate.OrganisationId}")
                .Respond("application/json", JsonConvert.SerializeObject(Certificate));

            CertificateApiClient = new CertificateApiClient(client, Mock.Of<IAssessorTokenService>(), Mock.Of<ILogger<ApiClientBase>>());
            LearnerDetailsApiClient = new LearnerDetailsApiClient(client, Mock.Of<IAssessorTokenService>(), Mock.Of<ILogger<ApiClientBase>>());
            OrganisationsApiClient = new OrganisationsApiClient(client, Mock.Of<IAssessorTokenService>(), Mock.Of<ILogger<ApiClientBase>>());
            ScheduleApiClient = new ScheduleApiClient(client, Mock.Of<IAssessorTokenService>(), Mock.Of<ILogger<ApiClientBase>>());
            StandardVersionApiClient = new StandardVersionApiClient(client, Mock.Of<IAssessorTokenService>(), Mock.Of<ILogger<ApiClientBase>>());
        }

        private static Certificate SetupCertificate()
        {
            var certificateId = Guid.NewGuid();
            var certificate = new Builder().CreateNew<Certificate>()
                .With(q => q.CertificateData = JsonConvert.SerializeObject(new Builder()
                    .CreateNew<CertificateData>()
                    .With(x => x.AchievementDate = DateTime.Now)
                    .Build()))
                .Build();

            var organisaionId = Guid.NewGuid();
            certificate.OrganisationId = organisaionId;

            var organisation = new Builder().CreateNew<Organisation>()
                .Build();

            certificate.Organisation = organisation;

            return certificate;
        }
    }
}