using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.ExperienceAndAccreditationOrchestrator
{
    [TestFixture]
    public class OfstedDetailsTests : ExperienceAndAccreditationOrchestratorTestsBase
    {
        protected override string GatewayPageId => GatewayPageIds.Ofsted;

        [Test]
        public void check_ofsted_details_are_returned()
        {
            var ofstedDetails = new OfstedDetails
            {
                FullInspectionApprenticeshipGrade = "Pass", 
                FullInspectionOverallEffectivenessGrade = "Fail",
                GradeWithinTheLast3Years = true, 
                HasHadFullInspection = false, 
                HasHadMonitoringVisit = true,
                HasHadShortInspectionWithinLast3Years = false, 
                HasMaintainedFullGradeInShortInspection = true,
                HasMaintainedFundingSinceInspection = false,
                ReceivedFullInspectionGradeForApprenticeships = true
            };
            ExperienceAndAccreditationApiClient.Setup(x => x.GetOfstedDetails(ApplicationId)).ReturnsAsync(ofstedDetails);

            var request = new GetOfstedDetailsRequest(ApplicationId, UserName);
            var response = Orchestrator.GetOfstedDetailsViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(GatewayPageIds.Ofsted, viewModel.PageId);
            AssertCommonDetails(viewModel);

            Assert.AreEqual(ofstedDetails.FullInspectionApprenticeshipGrade, viewModel.FullInspectionApprenticeshipGrade);
            Assert.AreEqual(ofstedDetails.FullInspectionOverallEffectivenessGrade, viewModel.FullInspectionOverallEffectivenessGrade);
            Assert.AreEqual(ofstedDetails.GradeWithinTheLast3Years, viewModel.GradeWithinTheLast3Years);
            Assert.AreEqual(ofstedDetails.HasHadFullInspection, viewModel.HasHadFullInspection);
            Assert.AreEqual(ofstedDetails.HasHadMonitoringVisit, viewModel.HasHadMonitoringVisit);
            Assert.AreEqual(ofstedDetails.HasHadShortInspectionWithinLast3Years, viewModel.HasHadShortInspectionWithinLast3Years);
            Assert.AreEqual(ofstedDetails.HasMaintainedFullGradeInShortInspection, viewModel.HasMaintainedFullGradeInShortInspection);
            Assert.AreEqual(ofstedDetails.HasMaintainedFundingSinceInspection, viewModel.HasMaintainedFundingSinceInspection);
            Assert.AreEqual(ofstedDetails.ReceivedFullInspectionGradeForApprenticeships, viewModel.ReceivedFullInspectionGradeForApprenticeships);
        }
    }
}
