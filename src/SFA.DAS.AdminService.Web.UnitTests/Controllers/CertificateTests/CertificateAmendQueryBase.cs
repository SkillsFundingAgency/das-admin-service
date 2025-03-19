using FizzWare.NBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.CertificateTests
{
    public class CertificateAmendQueryBase
    {
        protected Mock<ILogger<CertificateAmendController>> MockedLogger;
        protected Mock<IHttpContextAccessor> MockHttpContextAccessor;

        protected CertificateApiClient CertificateApiClient;
        protected LearnerDetailsApiClient LearnerDetailsApiClient;
        protected OrganisationsApiClient OrganisationsApiClient;
        protected ScheduleApiClient ScheduleApiClient;
        protected StandardVersionApiClient StandardVersionApiClient;

        protected Certificate Certificate;

        public CertificateAmendQueryBase()
        {
            MockedLogger = new Mock<ILogger<CertificateAmendController>>();

            MockHttpContextAccessor = SetupMockedHttpContextAccessor();
            SetupApiClients();
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

            var clientFactory = new Mock<IAssessorApiClientFactory>();
            clientFactory.Setup(x => x.CreateHttpClient())
                .Returns(client);

            mockHttp.When($"http://localhost:59022/api/v1/certificates/{Certificate.Id}")
                .Respond("application/json", JsonConvert.SerializeObject(Certificate));

            mockHttp.When($"http://localhost:59022/api/v1/organisations/organisation/{Certificate.OrganisationId}")
                .Respond("application/json", JsonConvert.SerializeObject(Certificate));

            var options = SetupOptions();
            mockHttp.When($"http://localhost:59022/api/v1/standard-version/standard-options/StandardUId1")
                .Respond("application/json", JsonConvert.SerializeObject(options));

            mockHttp.When($"http://localhost:59022/api/v1/certificates/update")
                .Respond("application/json", JsonConvert.SerializeObject(Certificate));

            CertificateApiClient = new CertificateApiClient(clientFactory.Object, Mock.Of<ILogger<CertificateApiClient>>());
            LearnerDetailsApiClient = new LearnerDetailsApiClient(clientFactory.Object, Mock.Of<ILogger<LearnerDetailsApiClient>>());
            OrganisationsApiClient = new OrganisationsApiClient(clientFactory.Object, Mock.Of<ILogger<OrganisationsApiClient>>());
            ScheduleApiClient = new ScheduleApiClient(clientFactory.Object, Mock.Of<ILogger<ScheduleApiClient>>());
            StandardVersionApiClient = new StandardVersionApiClient(clientFactory.Object, Mock.Of<ILogger<StandardVersionApiClient>>());
        }

        private static Certificate SetupCertificate()
        {
            var certificateId = Guid.NewGuid();
            var certificate = new Builder().CreateNew<Certificate>()
                .With(q => q.CertificateData = new Builder()
                    .CreateNew<CertificateData>()
                    .With(x => x.AchievementDate = DateTime.Now)
                    .Build())
                .Build();

            var organisaionId = Guid.NewGuid();
            certificate.OrganisationId = organisaionId;

            var organisation = new Builder().CreateNew<Organisation>()
                .Build();

            certificate.Organisation = organisation;

            return certificate;
        }

        private static StandardOptions SetupOptions()
        {
            var option = new StandardOptions
            {
                CourseOption = new List<string> { "CourseOption1" },
                StandardCode = 1,
                StandardReference = "ST0001",
                StandardUId = "StandardUId1"
            };

            return option;
        }
    }
}