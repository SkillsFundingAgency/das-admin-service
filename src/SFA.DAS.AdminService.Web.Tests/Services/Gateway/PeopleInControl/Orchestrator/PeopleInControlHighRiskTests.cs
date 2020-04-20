﻿using System;
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
    public class PeopleInControlHighRiskTests
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
            _applyApiClient.Setup(x => x.GetPageCommonDetails(_applicationId, GatewayPageIds.PeopleInControlRisk, UserName)).ReturnsAsync(_commonDetails);
        }

        [Test]
        public void check_people_in_control_high_risk_common_details_are_returned()
        {

            var request = new GetPeopleInControlHighRiskRequest(_applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlHighRiskViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(GatewayPageIds.PeopleInControlRisk, viewModel.PageId);
            Assert.AreEqual(_applicationId, viewModel.ApplicationId);
            Assert.AreEqual(_commonDetails.OptionFailText, viewModel.OptionFailText);
            Assert.AreEqual(_commonDetails.OptionInProgressText, viewModel.OptionInProgressText);
            Assert.AreEqual(_commonDetails.OptionPassText, viewModel.OptionPassText);
            Assert.AreEqual(_commonDetails.Status, viewModel.Status);
            Assert.AreEqual(_commonDetails.Ukprn, viewModel.Ukprn);
            Assert.AreEqual(_commonDetails.LegalName, viewModel.ApplyLegalName);
        }


        [Test]
        public void check_people_in_control_high_risk_director_details_from_cached_data_are_returned()
        {

            var directorsFromSubmitted = new List<PersonInControl>{
                new PersonInControl{
                    Name =PersonInControlName + DirectorsPostfix,
                    MonthYearOfBirth = PersonInControlDob + DirectorsPostfix ,
                } };
            _organisationSummaryApiClient.Setup(x => x.GetDirectorsFromCompaniesHouse(_applicationId)).ReturnsAsync(directorsFromSubmitted);

            var request = new GetPeopleInControlHighRiskRequest(_applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlHighRiskViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(1, viewModel.CompanyDirectorsData.PeopleInControl.Count);
            Assert.AreEqual(viewModel.CompanyDirectorsData.PeopleInControl.First().Name, directorsFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.CompanyDirectorsData.PeopleInControl.First().MonthYearOfBirth, directorsFromSubmitted.First().MonthYearOfBirth);
        }

        [Test]
        public void check_people_in_control_high_risk_psc_details_from_submitted_are_returned()
        {

            var pcsFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+PscPostfix,
                    MonthYearOfBirth = PersonInControlDob + PscPostfix
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetPscsFromCompaniesHouse(_applicationId)).ReturnsAsync(pcsFromSubmitted);

            var request = new GetPeopleInControlHighRiskRequest(_applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlHighRiskViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(1, viewModel.PscData.PeopleInControl.Count);
            Assert.AreEqual(viewModel.PscData.PeopleInControl.First().Name, pcsFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.PscData.PeopleInControl.First().MonthYearOfBirth, pcsFromSubmitted.First().MonthYearOfBirth);
        }

        [Test]
        public void check_people_in_control_trustee_details_from_submitted_are_returned()
        {
            var trusteesFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+ TrusteesPostfix,
                    MonthYearOfBirth = PersonInControlDob + TrusteesPostfix
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetTrusteesFromCharityCommission(_applicationId)).ReturnsAsync(trusteesFromSubmitted);

            var request = new GetPeopleInControlHighRiskRequest(_applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlHighRiskViewModel(request);

            var viewModel = response.Result;
            Assert.AreEqual(1, viewModel.TrusteeData.PeopleInControl.Count);

            Assert.AreEqual(viewModel.TrusteeData.PeopleInControl.First().Name, trusteesFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.TrusteeData.PeopleInControl.First().MonthYearOfBirth, trusteesFromSubmitted.First().MonthYearOfBirth);
        }

        [Test]
        public void check_people_in_control_whos_in_control_details_from_submitted_are_returned()
        {
            var whosInControlFromSubmitted = new List<PersonInControl>
            {
                new PersonInControl
                {
                    Name = PersonInControlName+ WhosInControlPostfix,
                    MonthYearOfBirth = PersonInControlDob + WhosInControlPostfix
                }
            };
            _organisationSummaryApiClient.Setup(x => x.GetWhosInControlFromSubmitted(_applicationId)).ReturnsAsync(whosInControlFromSubmitted);

            var request = new GetPeopleInControlHighRiskRequest(_applicationId, UserName);
            var response = _orchestrator.GetPeopleInControlHighRiskViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(1, viewModel.WhosInControlData.PeopleInControl.Count);
            Assert.AreEqual(viewModel.WhosInControlData.PeopleInControl.First().Name, whosInControlFromSubmitted.First().Name);
            Assert.AreEqual(viewModel.WhosInControlData.PeopleInControl.First().MonthYearOfBirth, whosInControlFromSubmitted.First().MonthYearOfBirth);
        }
    }
}
