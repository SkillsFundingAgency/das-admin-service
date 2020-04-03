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

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.ExperienceAndAccreditation
{
    [TestFixture]
    public class SubcontractorDeclarationTests
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

        [Test]
        public void check_subcontractor_declaration_details_are_returned()
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
            _applyApiClient.Setup(x => x.GetPageCommonDetails(applicationId, GatewayPageIds.SubcontractorDeclaration, UserName)).ReturnsAsync(commonDetails);

            var subcontractorDeclaration = new SubcontractorDeclaration { HasDeliveredTrainingAsSubcontractor = true, ContractFileName = "fileName" };
            _experienceAndAccreditationApiClient.Setup(x => x.GetSubcontractorDeclaration(applicationId)).ReturnsAsync(subcontractorDeclaration);

            var request = new GetSubcontractorDeclarationRequest(applicationId, UserName);
            var response = _orchestrator.GetSubcontractorDeclarationViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(GatewayPageIds.SubcontractorDeclaration, viewModel.PageId);
            Assert.AreEqual(applicationId, viewModel.ApplicationId);
            Assert.AreEqual(commonDetails.OptionFailText, viewModel.OptionFailText);
            Assert.AreEqual(commonDetails.OptionInProgressText, viewModel.OptionInProgressText);
            Assert.AreEqual(commonDetails.OptionPassText, viewModel.OptionPassText);
            Assert.AreEqual(commonDetails.Status, viewModel.Status);
            Assert.AreEqual(commonDetails.Ukprn, viewModel.Ukprn);
            Assert.AreEqual(commonDetails.LegalName, viewModel.ApplyLegalName);
            Assert.AreEqual(subcontractorDeclaration.HasDeliveredTrainingAsSubcontractor, viewModel.HasDeliveredTrainingAsSubcontractor);
            Assert.AreEqual(subcontractorDeclaration.ContractFileName, viewModel.ContractFileName);
        }

        [Test]
        public void check_subcontractor_contract_file_is_returned()
        {
            var applicationId = Guid.NewGuid();
            var fileStreamResult = new FileStreamResult(new MemoryStream(), "application/pdf");
            _experienceAndAccreditationApiClient.Setup(x => x.GetSubcontractorDeclarationContractFile(applicationId)).ReturnsAsync(fileStreamResult);

            var request = new GetSubcontractorDeclarationContractFileRequest(applicationId);
            var response = _orchestrator.GetSubcontractorDeclarationContractFile(request);

            var result = response.Result;

            Assert.AreSame(fileStreamResult, result);
        }
    }
}
