using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure.Apply;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Services.Gateway;IRo
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.PeopleInControl.Orchestrator
{
    [TestFixture]
    public class PeopleInControlTests
    {
        private Web.Services.Gateway.PeopleInControlOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpOrganisationSummaryApiClient> _organisationSummaryApiClient;
        private Mock<ILogger<PeopleInControlOrchestrator>> _logger;

        private static string ukprn => "12344321";
        private static string UKRLPLegalName => "Mark's workshop";
        private static string UserName = "GatewayUser";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _organisationSummaryApiClient = new Mock<IRoatpOrganisationSummaryApiClient>();
            _logger = new Mock<ILogger<PeopleInControlOrchestrator>>();
            _orchestrator = new PeopleInControlOrchestrator(_applyApiClient.Object, _organisationSummaryApiClient.Object, _logger.Object);
        }

        [Test]
        public void check_people_in_control_details_are_returned()
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
            _applyApiClient.Setup(x => x.GetPageCommonDetails(applicationId, GatewayPageIds.PeopleInControl, UserName)).ReturnsAsync(commonDetails);

            const string personInControlName = "Bob";
            const string personInControlDob = "Jan 1990";
            const string sourceExternal = "-external";
            const string sourceSubmitted = "-submitted";
            const string directorsPostfix = "-directors";
            const string pscPostfix = "-psc";
            const string trusteesPostfix = "-trustee";
            const string whosInControlPostfix = "-wic";

            var directorsFromCompaniesHouse = new List<PersonInControl>{
                new PersonInControl{
                    Name =personInControlName + directorsPostfix + sourceExternal,
                    DateOfBirth = personInControlDob + directorsPostfix + sourceExternal
                    } };
            _organisationSummaryApiClient.Setup(x => x.GetDirectorsFromCompaniesHouse(applicationId)).ReturnsAsync(directorsFromCompaniesHouse);

            var directorsFromSubmitted = new List<PersonInControl>{
                new PersonInControl{
                    Name =personInControlName + directorsPostfix + sourceSubmitted,
                    DateOfBirth = personInControlDob + directorsPostfix + sourceSubmitted
                } };
            _organisationSummaryApiClient.Setup(x => x.GetDirectorsFromSubmitted(applicationId)).ReturnsAsync(directorsFromSubmitted);

            var pscsFromCompaniesHouse = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = personInControlName+pscPostfix + sourceExternal,
                    DateOfBirth = personInControlDob + pscPostfix + sourceExternal
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetPscsFromCompaniesHouse(applicationId)).ReturnsAsync(pscsFromCompaniesHouse);

            var pcsFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = personInControlName+pscPostfix + sourceSubmitted,
                    DateOfBirth = personInControlDob + pscPostfix + sourceSubmitted
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetPscsFromSubmitted(applicationId)).ReturnsAsync(pcsFromSubmitted);


            var trusteesFromCharityCommission = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = personInControlName+ trusteesPostfix + sourceExternal
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetTrusteesFromCharityCommission(applicationId)).ReturnsAsync(trusteesFromCharityCommission);

            var trusteesFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = personInControlName+ trusteesPostfix + sourceExternal,
                    DateOfBirth = personInControlDob + trusteesPostfix + sourceExternal
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetTrusteesFromSubmitted(applicationId)).ReturnsAsync(trusteesFromSubmitted);

            var whosInControlFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = personInControlName+ whosInControlPostfix + sourceSubmitted,
                    DateOfBirth = personInControlDob + whosInControlPostfix + sourceSubmitted
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetWhosInControlFromSubmitted(applicationId)).ReturnsAsync(whosInControlFromSubmitted);


            var request = new GetPeopleInControlRequest(applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(GatewayPageIds.PeopleInControl, viewModel.PageId);
            Assert.AreEqual(applicationId, viewModel.ApplicationId);
            Assert.AreEqual(commonDetails.OptionFailText, viewModel.OptionFailText);
            Assert.AreEqual(commonDetails.OptionInProgressText, viewModel.OptionInProgressText);
            Assert.AreEqual(commonDetails.OptionPassText, viewModel.OptionPassText);
            Assert.AreEqual(commonDetails.Status, viewModel.Status);
            Assert.AreEqual(commonDetails.Ukprn, viewModel.Ukprn);
            Assert.AreEqual(commonDetails.LegalName, viewModel.ApplyLegalName);
            
            // Director checks
            Assert.AreEqual(1,viewModel.CompanyDirectorsData.FromExternalSource.Count);
            Assert.AreEqual(1, viewModel.CompanyDirectorsData.FromSubmittedApplication.Count);
            Assert.AreEqual(viewModel.CompanyDirectorsData.FromExternalSource.First().Name, directorsFromCompaniesHouse.First().Name);
            Assert.AreEqual(viewModel.CompanyDirectorsData.FromExternalSource.First().DateOfBirth, directorsFromCompaniesHouse.First().DateOfBirth);
            Assert.AreEqual(viewModel.CompanyDirectorsData.FromSubmittedApplication.First().Name, directorsFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.CompanyDirectorsData.FromSubmittedApplication.First().DateOfBirth, directorsFromSubmitted.First().DateOfBirth);

            // Psc checks
            Assert.AreEqual(1, viewModel.PscData.FromExternalSource.Count);
            Assert.AreEqual(1, viewModel.PscData.FromSubmittedApplication.Count);
            Assert.AreEqual(viewModel.PscData.FromExternalSource.First().Name, pscsFromCompaniesHouse.First().Name);
            Assert.AreEqual(viewModel.PscData.FromExternalSource.First().DateOfBirth, pscsFromCompaniesHouse.First().DateOfBirth);
            Assert.AreEqual(viewModel.PscData.FromSubmittedApplication.First().Name, pcsFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.PscData.FromSubmittedApplication.First().DateOfBirth, pcsFromSubmitted.First().DateOfBirth);

            // Trustee checks
            Assert.AreEqual(1, viewModel.TrusteeData.FromExternalSource.Count);
            Assert.AreEqual(1, viewModel.TrusteeData.FromSubmittedApplication.Count);
            Assert.AreEqual(viewModel.TrusteeData.FromExternalSource.First().Name, trusteesFromCharityCommission.First().Name);
            Assert.AreEqual(viewModel.TrusteeData.FromExternalSource.First().DateOfBirth, trusteesFromCharityCommission.First().DateOfBirth);
            Assert.AreEqual(viewModel.TrusteeData.FromSubmittedApplication.First().Name, trusteesFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.TrusteeData.FromSubmittedApplication.First().DateOfBirth, trusteesFromSubmitted.First().DateOfBirth);

            // WhosInControl checks
            Assert.IsNull(viewModel.WhosInControlData.FromExternalSource);
            Assert.AreEqual(1, viewModel.CompanyDirectorsData.FromSubmittedApplication.Count);
            Assert.AreEqual(viewModel.WhosInControlData.FromSubmittedApplication.First().Name, whosInControlFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.WhosInControlData.FromSubmittedApplication.First().DateOfBirth, whosInControlFromSubmitted.First().DateOfBirth);

        }
    }
}
