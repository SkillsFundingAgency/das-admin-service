using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.Overview.Orchestrator
{
    [TestFixture]
    public class OverviewTests
    {
        private GatewayOverviewOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOverviewOrchestrator>> _logger;


        private static string ukprn => "12344321";
        private static string UKRLPLegalName => "Mark's workshop";

        //private static string CompanyNumber => "654321";
        //private static string CharityNumber => "123456";

        //private static string ProviderName => "Mark's other workshop";
        //private static string CompanyName => "Companies House Name";

        //private static string CharityName => "Charity commission Name";
        private static string ApplyTradingName = "My Trading Name";

        private static string UserName = "GatewayUser";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _logger = new Mock<ILogger<GatewayOverviewOrchestrator>>();
            _orchestrator = new GatewayOverviewOrchestrator(_applyApiClient.Object, _logger.Object);
        }

        [Test]
        public void check_overview_orchestrator_for_gateway_status_new()
        {
            var applicationId = Guid.NewGuid();

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                CheckedOn = DateTime.Now,
                LegalName = UKRLPLegalName,
                Ukprn = ukprn
            };

            var applicationResponse = new RoatpApplicationResponse();

           var applyData = new RoatpApplyData();
           var applyDetails = new RoatpApplyDetails();
           var gatewayReviewStatus = GatewayReviewStatus.New;   //branch here for other statuses

           applicationResponse.GatewayReviewStatus = gatewayReviewStatus;   



           applyData.ApplyDetails = applyDetails;
            applicationResponse.ApplyData = applyData;

            //var applicationData = new AssessorService.ApplyTypes.Roatp.Apply
            //{
            //    ApplyData = new RoatpApplyData
            //    {
            //        ApplyDetails = new RoatpApplyDetails
            //        {
            //            ReferenceNumber = application.ApplyData.ApplyDetails.ReferenceNumber,
            //            ProviderRoute = application.ApplyData.ApplyDetails.ProviderRoute,
            //            ProviderRouteName = application.ApplyData.ApplyDetails.ProviderRouteName,
            //            UKPRN = application.ApplyData.ApplyDetails.UKPRN,
            //            OrganisationName = application.ApplyData.ApplyDetails.OrganisationName,
            //            ApplicationSubmittedOn = application.ApplyData.ApplyDetails.ApplicationSubmittedOn
            //        }
            //    },
            //    Id = application.Id,
            //    ApplicationId = application.ApplicationId,
            //    OrganisationId = application.OrganisationId,
            //    ApplicationStatus = application.ApplicationStatus,
            //    GatewayReviewStatus = application.GatewayReviewStatus,
            //    AssessorReviewStatus = application.AssessorReviewStatus,
            //    FinancialReviewStatus = application.FinancialReviewStatus
            //};

            _applyApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(applicationResponse);
            _applyApiClient.Setup(x => x.GetPageCommonDetails(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            _applyApiClient.Setup(x => x.GetTradingName(It.IsAny<Guid>())).ReturnsAsync(ApplyTradingName);   // branch here for empty trading name, if present will call submit with trading name



            var request = new GetApplicationOverviewRequest(applicationId, UserName);

            var response = _orchestrator.GetOverviewViewModel(request);

            var viewModel = response.Result;


            // This is lifted from elsewhere, but you can see the principle------
            //Assert.AreEqual(UKRLPLegalName, viewModel.ApplyLegalName);
            //Assert.AreEqual(ProviderName, viewModel.UkrlpLegalName);
            //Assert.AreEqual(CompanyName, viewModel.CompaniesHouseLegalName);
            //Assert.AreEqual(CharityName, viewModel.CharityCommissionLegalName);
            //Assert.AreEqual(ukprn, viewModel.Ukprn);
        }


    }
}
