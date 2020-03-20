using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;

namespace SFA.DAS.AdminService.Web.Tests.Services
{
    [TestFixture]
    public class GatewayCriminalComplianceChecksOrchestratorTests
    {
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpGatewayCriminalComplianceChecksApiClient> _criminalChecksApiClient;
        private Mock<ILogger<GatewayCriminalComplianceChecksOrchestrator>> _logger;
        private GatewayCriminalComplianceChecksOrchestrator _orchestrator;

        [SetUp]
        public void Before_each_test()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _criminalChecksApiClient = new Mock<IRoatpGatewayCriminalComplianceChecksApiClient>();
            _logger = new Mock<ILogger<GatewayCriminalComplianceChecksOrchestrator>>();

            _orchestrator = new GatewayCriminalComplianceChecksOrchestrator(_applyApiClient.Object, _criminalChecksApiClient.Object, _logger.Object);
        }

        [Test]
        public void Orchestrator_builds_view_model_from_api()
        {
            var pageId = "1234";
            var questionId = "CC-11";
            var furtherQuestionId = "CC-11.1";

            var applicationDetails = new RoatpApplicationResponse { GatewayReviewStatus = ApplicationReviewStatus.InProgress };

            var commonDetails = new GatewayCommonDetails
            {
                GatewayReviewStatus = GatewayReviewStatus.InProgress,
                LegalName = "Legal Eagle",
                Status = "Pass",
                OptionPassText = "No comment",
                Ukprn = "10001234",
                CheckedOn = DateTime.Now.AddHours(-2),
                ApplicationSubmittedOn = DateTime.Now.AddMonths(-1)
            };
            _applyApiClient.Setup(x => x.GetPageCommonDetails(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(commonDetails);

            var criminalComplianceDetails = new CriminalComplianceCheckDetails
            {
                QuestionText = "What is the question",
                Answer = "Yes",
                QuestionId = questionId,
                PageId = pageId,
                FurtherAnswer = "Lorem ipsum",
                FurtherQuestionId = furtherQuestionId
            };

            _criminalChecksApiClient.Setup(x => x.GetCriminalComplianceQuestionDetails(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(criminalComplianceDetails);
            
            var viewModel = _orchestrator.GetCriminalComplianceCheckViewModel(new GetCriminalComplianceCheckRequest(Guid.NewGuid(), "1234", "user")).GetAwaiter().GetResult();
            
            viewModel.PageId.Should().Be(pageId);
            viewModel.QuestionText.Should().Be(criminalComplianceDetails.QuestionText);
            viewModel.Ukprn.Should().Be(commonDetails.Ukprn);
            viewModel.ApplyLegalName.Should().Be(commonDetails.LegalName);
            viewModel.ComplianceCheckQuestionId.Should().Be(questionId);
            viewModel.ComplianceCheckAnswer.Should().Be("Yes");
            viewModel.FurtherInformationQuestionId.Should().Be(furtherQuestionId);
            viewModel.FurtherInformationAnswer.Should().Be("Lorem ipsum");
        }
    }
}
