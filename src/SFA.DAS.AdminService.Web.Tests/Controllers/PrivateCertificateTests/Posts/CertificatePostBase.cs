﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AdminService.Web.Controllers;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Tests.MockedObjects;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.PrivateCertificateTests.Posts
{
    public class CertificatePostBase
    {
        protected Mock<ILogger<CertificateAmendController>> MockLogger;
        protected Mock<IHttpContextAccessor> MockHttpContextAccessor;
        protected ApiClient MockApiClient;

        protected Mock<ISessionService> MockSession;
        protected IOrganisationsApiClient MockOrganisationsApiClient;
        protected Certificate Certificate;
        protected CertificateData CertificateData;
        
        public CertificatePostBase()
        {
            Certificate = SetupCertificate();

            MockLogger = new Mock<ILogger<CertificateAmendController>>();
            var mockedApiClientLogger = new Mock<ILogger<ApiClient>>();
            var mockedOrganisationApiClientLogger = new Mock<ILogger<OrganisationsApiClient>>();
            MockSession = new Mock<ISessionService>();

            MockHttpContextAccessor = MockedHttpContextAccessor.Setup();
            MockApiClient = MockedApiClient.Setup(Certificate, mockedApiClientLogger);
            
            MockOrganisationsApiClient = MockedOrganisationsApiClient.Setup(mockedOrganisationApiClientLogger);
            CertificateData = JsonConvert.DeserializeObject<CertificateData>(Certificate.CertificateData);
        }

        private Certificate SetupCertificate()
        {
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