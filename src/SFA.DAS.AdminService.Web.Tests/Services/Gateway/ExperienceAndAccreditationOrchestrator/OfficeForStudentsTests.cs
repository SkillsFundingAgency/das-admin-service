using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.ExperienceAndAccreditationOrchestrator
{
    [TestFixture]
    public class OfficeForStudentsTests
    {
        private GatewayExperienceAndAccreditationOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpExperienceAndAccreditationApiClient> _experienceAndAccreditationApiClient;
        private Mock<ILogger<GatewayExperienceAndAccreditationOrchestrator>> _logger;
        
        private static string ukprn => "12344321";
        private static string UKRLPLegalName => "Mark's workshop";
        private static string UserName = "GatewayUser";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _experienceAndAccreditationApiClient = new Mock<IRoatpExperienceAndAccreditationApiClient>();
            _logger = new Mock<ILogger<GatewayExperienceAndAccreditationOrchestrator>>();
            _orchestrator = new GatewayExperienceAndAccreditationOrchestrator(_applyApiClient.Object, _experienceAndAccreditationApiClient.Object, _logger.Object);
        }

        [TestCase("Yes", true)]
        [TestCase("No", false)]
        public void check_office_for_students_details_are_returned(string returnedAnswer, bool expectedResult)
        {
            var applicationId = Guid.NewGuid();

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                CheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn,
                GatewayReviewStatus = "RevStatus",
                OptionFailText = "fail",
                OptionInProgressText = "inprog",
                OptionPassText = "Pass",
                Status = "Status"
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(applicationId, GatewayPageIds.OfficeForStudents, UserName)).ReturnsAsync(commonDetails);

            _experienceAndAccreditationApiClient.Setup(x => x.GetOfficeForStudents(applicationId)).ReturnsAsync(returnedAnswer);

            var request = new GetOfficeForStudentsRequest(applicationId, UserName);
            var response = _orchestrator.GetOfficeForStudentsViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(GatewayPageIds.OfficeForStudents, viewModel.PageId);
            Assert.AreEqual(applicationId, viewModel.ApplicationId);
            Assert.AreEqual(commonDetails.GatewayReviewStatus, viewModel.GatewayReviewStatus);
            Assert.AreEqual(commonDetails.OptionFailText, viewModel.OptionFailText);
            Assert.AreEqual(commonDetails.OptionInProgressText, viewModel.OptionInProgressText);
            Assert.AreEqual(commonDetails.OptionPassText, viewModel.OptionPassText);
            Assert.AreEqual(commonDetails.Status, viewModel.Status);
            Assert.AreEqual(commonDetails.Ukprn, viewModel.Ukprn);
            Assert.AreEqual(commonDetails.LegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(expectedResult, viewModel.IsOrganisationFundedByOfficeForStudents);
        }
    }
}
