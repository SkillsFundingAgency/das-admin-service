using AutoFixture;
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

        private Fixture _autoFixture;

        private Mock<ISessionService> _mockSessionService;

        private MergeOrganisationSessionService _mergeSessionService;

        [SetUp]
        public void Arrange()
        {
            _autoFixture = new Fixture();

            _mockSessionService = new Mock<ISessionService>();

            _mergeSessionService = new MergeOrganisationSessionService(_mockSessionService.Object);
        }

        [Test]
        public void When_StartingNewMergeRequest_Then_SetEmptyMerge()
        {
            _mergeSessionService.StartNewMergeRequest();

            _mockSessionService.Verify(ms => ms.Set(_mergeOrganisationsSessionKey, 
                It.Is<MergeRequest>(r => r.PrimaryEpao == null && r.SecondaryEpao == null)));
        }

        [Test]
        public void When_GettingMergeRequest_Then_ReturnMergeRequest()
        {
            var mergeRequest = _autoFixture.Create<MergeRequest>();

            _mockSessionService.Setup(s => s.Get<MergeRequest>(_mergeOrganisationsSessionKey))
                .Returns(mergeRequest);

            var result = _mergeSessionService.GetMergeRequest();

            result.Should().BeEquivalentTo(mergeRequest);
        }
    }
}
