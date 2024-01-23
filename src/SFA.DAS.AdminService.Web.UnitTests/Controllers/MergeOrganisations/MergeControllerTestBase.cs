using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions.TagHelpers;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Web.Domain.Merge;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.Merge;
using SFA.DAS.AdminService.Web.Models.Merge;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class MergeControllerTestBase
    {
        protected MergeRequest _mergeRequest;
        protected Fixture _autoFixture;
        protected Mock<IMergeOrganisationsApiClient> _mergeOrganisationApiClient;
        protected Mock<IRegisterApiClient> _registerApiClient;
        protected Mock<IMergeOrganisationSessionService> _mockMergeSessionService;
        protected Mock<IHttpContextAccessor> _mockContextAccessor;

        protected MergeOrganisationsController MergeController;

        protected const int DefaultPageIndex = 1;
        protected const int DefaultMergesPerPage = 10;
        protected const string DefaultSortOrder = SortOrder.Desc;
        protected const string DefaultSortColumn = MergeOrganisationSortColumn.CompletedAt;

        [SetUp]
        public void BaseArrange()
        {
            _autoFixture = new Fixture();

            _mergeOrganisationApiClient = new Mock<IMergeOrganisationsApiClient>();
            _registerApiClient = new Mock<IRegisterApiClient>();
            _mockMergeSessionService = new Mock<IMergeOrganisationSessionService>();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();

            MergeController = new MergeOrganisationsController(_mergeOrganisationApiClient.Object, _registerApiClient.Object, _mockMergeSessionService.Object, _mockContextAccessor.Object, Mock.Of<ILogger<MergeOrganisationsController>>());

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
