using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using Answer = SFA.DAS.QnA.Api.Types.Page.Answer;
using PageOfAnswers = SFA.DAS.QnA.Api.Types.Page.PageOfAnswers;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway.OrganisationChecks.Orchestrator
{
    [TestFixture]
    public class TradingNameTests
    {
        private GatewayOrganisationChecksOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOrganisationChecksOrchestrator>> _logger;
        private Mock<IQnaApiClient> _qnaApiClient;

        private static string PageId => "1-20";
        private static string ukprn => "12344321";
        private static string UKRLPTradingName => "Mark's workshop";
        private static string LegalName => "My Legal Name";

        private static string UserName = "GatewayUser";
        private static string ApplyTradingName = "My Trading Name";

        private static string QuestionId => "PRE-46";
        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _qnaApiClient = new Mock<IQnaApiClient>();
            _logger = new Mock<ILogger<GatewayOrganisationChecksOrchestrator>>();
            _orchestrator = new GatewayOrganisationChecksOrchestrator(_applyApiClient.Object, _qnaApiClient.Object, _logger.Object);
        }

        [Test]
        public void check_trading_name_orchestrator_builds_with_expected_details()
        {
            var applicationId = Guid.NewGuid();

            var commonDetails = new GatewayCommonDetails
            {
                ApplicationSubmittedOn = DateTime.Now.AddDays(-3),
                CheckedOn = DateTime.Now,
                LegalName = LegalName,
                Ukprn = ukprn
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            var ukrlpDetails = new ProviderDetails
            {
                ProviderAliases =  new List<ProviderAlias>()
                {
                    new ProviderAlias{Alias = UKRLPTradingName}

                }
            };

            var applyPage = new QnA.Api.Types.Page.Page
            {
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Answers = new List<Answer>
                        {
                            new Answer { QuestionId = QuestionId, Value = ApplyTradingName}
                        }
                    }
                }
            };


            _applyApiClient.Setup(x => x.GetUkrlpDetails(It.IsAny<Guid>())).ReturnsAsync(ukrlpDetails);
            _qnaApiClient.Setup(x =>
                    x.GetPageBySectionNo(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(applyPage);
        

            var request = new GetTradingNameRequest(applicationId, UserName);

            var response = _orchestrator.GetTradingNameViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(UKRLPTradingName, viewModel.UkrlpTradingName);
            Assert.AreEqual(ApplyTradingName, viewModel.ApplyTradingName);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
        }


    }
}
