using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.ExperienceAndAccreditationOrchestrator
{
    [TestFixture]
    public class OfficeForStudentsTests : ExperienceAndAccreditationOrchestratorTestsBase
    {
        protected override string GatewayPageId => GatewayPageIds.OfficeForStudents;

        [TestCase("Yes", true)]
        [TestCase("No", false)]
        public void check_office_for_students_details_are_returned(string returnedAnswer, bool expectedResult)
        {
            ExperienceAndAccreditationApiClient.Setup(x => x.GetOfficeForStudents(ApplicationId)).ReturnsAsync(returnedAnswer);

            var request = new GetOfficeForStudentsRequest(ApplicationId, UserName);
            var response = Orchestrator.GetOfficeForStudentsViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(GatewayPageIds.OfficeForStudents, viewModel.PageId);
            Assert.AreEqual(ApplicationId, viewModel.ApplicationId);
            Assert.AreEqual(CommonDetails.OptionFailText, viewModel.OptionFailText);
            Assert.AreEqual(CommonDetails.OptionInProgressText, viewModel.OptionInProgressText);
            Assert.AreEqual(CommonDetails.OptionPassText, viewModel.OptionPassText);
            Assert.AreEqual(CommonDetails.Status, viewModel.Status);
            Assert.AreEqual(CommonDetails.Ukprn, viewModel.Ukprn);
            Assert.AreEqual(CommonDetails.LegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(expectedResult, viewModel.IsOrganisationFundedByOfficeForStudents);
        }
    }
}
