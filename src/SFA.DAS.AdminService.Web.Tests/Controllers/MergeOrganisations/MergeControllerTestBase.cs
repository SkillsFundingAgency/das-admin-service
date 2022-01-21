﻿using AutoFixture;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.Merge;
using SFA.DAS.AdminService.Web.Models.Merge;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class MergeControllerTestBase
    {
        protected MergeRequest _mergeRequest;
        protected Fixture _autoFixture;
        protected Mock<IApiClient> _mockApiClient;
        protected Mock<IMergeOrganisationSessionService> _mockMergeSessionService;
        protected Mock<IHttpContextAccessor> _mockContextAccessor;

        protected MergeOrganisationsController MergeController;

        [SetUp]
        public void BaseArrange()
        {
            _autoFixture = new Fixture();

            _mockApiClient = new Mock<IApiClient>();
            _mockMergeSessionService = new Mock<IMergeOrganisationSessionService>();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();

            MergeController = new MergeOrganisationsController(_mockApiClient.Object, _mockMergeSessionService.Object, _mockContextAccessor.Object);

            _mergeRequest = _autoFixture.Create<MergeRequest>();

            _mockMergeSessionService.Setup(ms => ms.GetMergeRequest())
                .Returns(_mergeRequest);
        }

        protected void VerifyGetMergeRequest()
        {
            _mockMergeSessionService.Verify(ms => ms.GetMergeRequest(), Times.Once);
        }
    }
}
