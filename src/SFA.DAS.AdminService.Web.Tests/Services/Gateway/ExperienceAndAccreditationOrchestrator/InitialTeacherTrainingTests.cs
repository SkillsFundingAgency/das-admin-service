using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.ExperienceAndAccreditationOrchestrator
{
    [TestFixture]
    public class InitialTeacherTrainingTests : ExperienceAndAccreditationOrchestratorTestsBase
    {
        protected override string GatewayPageId => GatewayPageIds.InitialTeacherTraining;

        [Test]
        public void check_initial_teacher_training_details_are_returned()
        {
            var initialTeacherTraining = new InitialTeacherTraining { DoesOrganisationOfferInitialTeacherTraining = true, IsPostGradOnlyApprenticeship = false };
            ExperienceAndAccreditationApiClient.Setup(x => x.GetInitialTeacherTraining(ApplicationId)).ReturnsAsync(initialTeacherTraining);

            var request = new GetInitialTeacherTrainingRequest(ApplicationId, UserName);
            var response = Orchestrator.GetInitialTeacherTrainingViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(GatewayPageIds.InitialTeacherTraining, viewModel.PageId);
            Assert.AreEqual(ApplicationId, viewModel.ApplicationId);
            Assert.AreEqual(CommonDetails.GatewayReviewStatus, viewModel.GatewayReviewStatus);
            Assert.AreEqual(CommonDetails.OptionFailText, viewModel.OptionFailText);
            Assert.AreEqual(CommonDetails.OptionInProgressText, viewModel.OptionInProgressText);
            Assert.AreEqual(CommonDetails.OptionPassText, viewModel.OptionPassText);
            Assert.AreEqual(CommonDetails.Status, viewModel.Status);
            Assert.AreEqual(CommonDetails.Ukprn, viewModel.Ukprn);
            Assert.AreEqual(CommonDetails.LegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(initialTeacherTraining.DoesOrganisationOfferInitialTeacherTraining, viewModel.DoesOrganisationOfferInitialTeacherTraining);
            Assert.AreEqual(initialTeacherTraining.IsPostGradOnlyApprenticeship, viewModel.IsPostGradOnlyApprenticeship);
        }
    }
}
