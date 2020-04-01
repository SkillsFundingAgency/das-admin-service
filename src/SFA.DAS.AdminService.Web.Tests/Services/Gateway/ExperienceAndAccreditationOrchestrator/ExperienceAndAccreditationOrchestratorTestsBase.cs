using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.ExperienceAndAccreditationOrchestrator
{
    public abstract class ExperienceAndAccreditationOrchestratorTestsBase
    {
        protected GatewayExperienceAndAccreditationOrchestrator Orchestrator;
        protected Mock<IRoatpApplicationApiClient> ApplyApiClient;
        protected Mock<IRoatpExperienceAndAccreditationApiClient> ExperienceAndAccreditationApiClient;
        protected Mock<ILogger<GatewayExperienceAndAccreditationOrchestrator>> Logger;
        protected abstract string GatewayPageId { get; }

        protected static string Ukprn => "12344321";
        protected static string UKRLPLegalName => "Mark's workshop";
        protected static string UserName = "GatewayUser";
        protected GatewayCommonDetails CommonDetails;
        protected Guid ApplicationId;

        [SetUp]
        public void Setup()
        {
            ApplyApiClient = new Mock<IRoatpApplicationApiClient>();
            ExperienceAndAccreditationApiClient = new Mock<IRoatpExperienceAndAccreditationApiClient>();
            Logger = new Mock<ILogger<GatewayExperienceAndAccreditationOrchestrator>>();
            Orchestrator = new GatewayExperienceAndAccreditationOrchestrator(ApplyApiClient.Object, ExperienceAndAccreditationApiClient.Object, Logger.Object);

            ApplicationId = Guid.NewGuid();

            CommonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                CheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = Ukprn,
                GatewayReviewStatus = "RevStatus",
                OptionFailText = "fail",
                OptionInProgressText = "inprog",
                OptionPassText = "Pass",
                Status = "Status"
            };
            ApplyApiClient.Setup(x => x.GetPageCommonDetails(ApplicationId, GatewayPageId, UserName)).ReturnsAsync(CommonDetails);
        }

        protected void AssertCommonDetails(RoatpGatewayPageViewModel viewModel)
        {
            Assert.AreEqual(ApplicationId, viewModel.ApplicationId);
            Assert.AreEqual(CommonDetails.GatewayReviewStatus, viewModel.GatewayReviewStatus);
            Assert.AreEqual(CommonDetails.OptionFailText, viewModel.OptionFailText);
            Assert.AreEqual(CommonDetails.OptionInProgressText, viewModel.OptionInProgressText);
            Assert.AreEqual(CommonDetails.OptionPassText, viewModel.OptionPassText);
            Assert.AreEqual(CommonDetails.Status, viewModel.Status);
            Assert.AreEqual(CommonDetails.Ukprn, viewModel.Ukprn);
            Assert.AreEqual(CommonDetails.LegalName, viewModel.ApplyLegalName);
        }
    }
}
