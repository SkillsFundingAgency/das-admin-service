using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure.Apply;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Services.Gateway;
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


        const string PersonInControlName = "Bob";
        const string PersonInControlDob = "Jan 1990";
        const string SourceExternal = "-external";
        const string SourceSubmitted = "-submitted";
        const string DirectorsPostfix = "-directors";
        const string PscPostfix = "-psc";
        const string TrusteesPostfix = "-trustee";
        const string WhosInControlPostfix = "-wic";

        readonly Guid _applicationId = Guid.NewGuid();
        private GatewayCommonDetails _commonDetails;

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _organisationSummaryApiClient = new Mock<IRoatpOrganisationSummaryApiClient>();
            _logger = new Mock<ILogger<PeopleInControlOrchestrator>>();
            _orchestrator = new PeopleInControlOrchestrator(_applyApiClient.Object, _organisationSummaryApiClient.Object, _logger.Object);
    
            _commonDetails = new GatewayCommonDetails
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
            _applyApiClient.Setup(x => x.GetPageCommonDetails(_applicationId, GatewayPageIds.PeopleInControl, UserName)).ReturnsAsync(_commonDetails);
        }

        [Test]
        public void check_people_in_control_common_details_are_returned()
        {

            var request = new GetPeopleInControlRequest(_applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(GatewayPageIds.PeopleInControl, viewModel.PageId);
            Assert.AreEqual(_applicationId, viewModel.ApplicationId);
            Assert.AreEqual(_commonDetails.OptionFailText, viewModel.OptionFailText);
            Assert.AreEqual(_commonDetails.OptionInProgressText, viewModel.OptionInProgressText);
            Assert.AreEqual(_commonDetails.OptionPassText, viewModel.OptionPassText);
            Assert.AreEqual(_commonDetails.Status, viewModel.Status);
            Assert.AreEqual(_commonDetails.Ukprn, viewModel.Ukprn);
            Assert.AreEqual(_commonDetails.LegalName, viewModel.ApplyLegalName);
        }

        [Test]
        public void check_people_in_control_director_details_from_companies_house_are_returned()
        {
            var directorsFromCompaniesHouse = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName + DirectorsPostfix + SourceExternal,
                    DateOfBirth = PersonInControlDob + DirectorsPostfix + SourceExternal
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetDirectorsFromCompaniesHouse(_applicationId))
                .ReturnsAsync(directorsFromCompaniesHouse);

            var request = new GetPeopleInControlRequest(_applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(1, viewModel.CompanyDirectorsData.FromExternalSource.Count);

            Assert.AreEqual(viewModel.CompanyDirectorsData.FromExternalSource.First().Name,
                directorsFromCompaniesHouse.First().Name);
            Assert.AreEqual(viewModel.CompanyDirectorsData.FromExternalSource.First().DateOfBirth,
                directorsFromCompaniesHouse.First().DateOfBirth);
        }

        [Test]
        public void check_people_in_control_director_details_from_submitted_are_returned()
        {

            var directorsFromSubmitted = new List<PersonInControl>{
                new PersonInControl{
                    Name =PersonInControlName + DirectorsPostfix + SourceSubmitted,
                    DateOfBirth = PersonInControlDob + DirectorsPostfix + SourceSubmitted
                } };
            _organisationSummaryApiClient.Setup(x => x.GetDirectorsFromSubmitted(_applicationId)).ReturnsAsync(directorsFromSubmitted);

            var request = new GetPeopleInControlRequest(_applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(1, viewModel.CompanyDirectorsData.FromSubmittedApplication.Count);
            Assert.AreEqual(viewModel.CompanyDirectorsData.FromSubmittedApplication.First().Name, directorsFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.CompanyDirectorsData.FromSubmittedApplication.First().DateOfBirth, directorsFromSubmitted.First().DateOfBirth);
        }


        [Test]
        public void check_people_in_control_psc_details_from_companies_house_are_returned()
        {

            var pscsFromCompaniesHouse = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+PscPostfix + SourceExternal,
                    DateOfBirth = PersonInControlDob + PscPostfix + SourceExternal
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetPscsFromCompaniesHouse(_applicationId)).ReturnsAsync(pscsFromCompaniesHouse);

            var request = new GetPeopleInControlRequest(_applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(1, viewModel.PscData.FromExternalSource.Count);

            Assert.AreEqual(viewModel.PscData.FromExternalSource.First().Name, pscsFromCompaniesHouse.First().Name);
            Assert.AreEqual(viewModel.PscData.FromExternalSource.First().DateOfBirth, pscsFromCompaniesHouse.First().DateOfBirth);
        }

        [Test]
        public void check_people_in_control_psc_details_from_submitted_are_returned()
        {

            var pcsFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+PscPostfix + SourceSubmitted,
                    DateOfBirth = PersonInControlDob + PscPostfix + SourceSubmitted
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetPscsFromSubmitted(_applicationId)).ReturnsAsync(pcsFromSubmitted);

            var request = new GetPeopleInControlRequest(_applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(1, viewModel.PscData.FromSubmittedApplication.Count);
            Assert.AreEqual(viewModel.PscData.FromSubmittedApplication.First().Name, pcsFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.PscData.FromSubmittedApplication.First().DateOfBirth, pcsFromSubmitted.First().DateOfBirth);
        }

        [Test]
        public void check_people_in_control_trustee_details_from_charity_commission_are_returned()
        {
            var trusteesFromCharityCommission = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+ TrusteesPostfix + SourceExternal
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetTrusteesFromCharityCommission(_applicationId)).ReturnsAsync(trusteesFromCharityCommission);


            var request = new GetPeopleInControlRequest(_applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;
            Assert.AreEqual(1, viewModel.TrusteeData.FromExternalSource.Count);
            Assert.AreEqual(viewModel.TrusteeData.FromExternalSource.First().Name, trusteesFromCharityCommission.First().Name);
            Assert.AreEqual(viewModel.TrusteeData.FromExternalSource.First().DateOfBirth, trusteesFromCharityCommission.First().DateOfBirth);

        }

        [Test]
        public void check_people_in_control_trustee_details_from_submitted_are_returned()
        {
            var trusteesFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+ TrusteesPostfix + SourceExternal,
                    DateOfBirth = PersonInControlDob + TrusteesPostfix + SourceExternal
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetTrusteesFromSubmitted(_applicationId)).ReturnsAsync(trusteesFromSubmitted);

            var request = new GetPeopleInControlRequest(_applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;
            Assert.AreEqual(1, viewModel.TrusteeData.FromSubmittedApplication.Count);

            Assert.AreEqual(viewModel.TrusteeData.FromSubmittedApplication.First().Name, trusteesFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.TrusteeData.FromSubmittedApplication.First().DateOfBirth, trusteesFromSubmitted.First().DateOfBirth);
        }

        [Test]
        public void check_people_in_control_whos_in_control_details_from_submitted_are_returned()
        {
            var whosInControlFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+ WhosInControlPostfix + SourceSubmitted,
                    DateOfBirth = PersonInControlDob + WhosInControlPostfix + SourceSubmitted
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetWhosInControlFromSubmitted(_applicationId)).ReturnsAsync(whosInControlFromSubmitted);

            var request = new GetPeopleInControlRequest(_applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(1, viewModel.WhosInControlData.FromSubmittedApplication.Count);
            Assert.AreEqual(viewModel.WhosInControlData.FromSubmittedApplication.First().Name, whosInControlFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.WhosInControlData.FromSubmittedApplication.First().DateOfBirth, whosInControlFromSubmitted.First().DateOfBirth);
        }
    }
}
