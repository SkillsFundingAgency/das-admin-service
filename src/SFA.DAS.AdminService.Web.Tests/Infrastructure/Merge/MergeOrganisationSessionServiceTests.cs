using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.Merge;
using SFA.DAS.AdminService.Web.Models.Merge;

namespace SFA.DAS.AdminService.Web.Tests.Infrastructure.Merge
{
    public class MergeOrganisationSessionServiceTests
    {
        private const string _mergeOrganisationsSessionKey = "Merge_Organisations";

        private Mock<IApiClient> _mockApiClient;
        private Mock<ISessionService> _mockSessionService;

        private MergeOrganisationSessionService _mergeSessionService;

        [SetUp]
        public void Arrange()
        {
            _mockSessionService = new Mock<ISessionService>();

            _mergeSessionService = new MergeOrganisationSessionService(_mockSessionService.Object);
        }


    }
}
