using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Handlers.Gateway;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Services
{
    [TestFixture]
    public class GatewayCriminalComplianceChecksOrchestratorTests
    {
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IQnaApiClient> _qnaApiClient;
        private Mock<ICriminalComplianceChecksQuestionLookupService> _lookupService;
        private Mock<ILogger<GatewayCriminalComplianceChecksOrchestrator>> _logger;
        private GatewayCriminalComplianceChecksOrchestrator _orchestrator;

        [SetUp]
        public void Before_each_test()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _qnaApiClient = new Mock<IQnaApiClient>();
            _lookupService = new Mock<ICriminalComplianceChecksQuestionLookupService>();
            _logger = new Mock<ILogger<GatewayCriminalComplianceChecksOrchestrator>>();

            _orchestrator = new GatewayCriminalComplianceChecksOrchestrator(_applyApiClient.Object, _qnaApiClient.Object, _lookupService.Object, _logger.Object);
        }

        [Test]
        public void Orchestrator_builds_view_model_from_ukrlp_and_qna_data_sources()
        {
            var pageId = "1234";
            var questionId = "CC-11";
            var furtherQuestionId = "CC-11.1";

            var applicationDetails = new RoatpApplicationResponse { GatewayReviewStatus = AssessorService.ApplyTypes.Roatp.ApplicationReviewStatus.InProgress };

            _applyApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(applicationDetails);

            var ukrlpDetails = new ProviderDetails
            {
                UKPRN = "10001212",
                ProviderName = "Provider name"
            };
            _applyApiClient.Setup(x => x.GetUkrlpDetails(It.IsAny<Guid>())).ReturnsAsync(ukrlpDetails);

            var pageDetails = new CriminalCompliancePageDetails
            {
                PageId = pageId,
                QuestionId = questionId,
                Title = "Page title"
            };

            _lookupService.Setup(x => x.GetPageDetailsForGatewayCheckPageId(It.IsAny<string>())).Returns(pageDetails);

            var criminalCompliancePage = new QnA.Api.Types.Page.Page
            {
                Questions = new List<QnA.Api.Types.Page.Question>
                {
                    new QnA.Api.Types.Page.Question
                    {
                        Label = "Question text",
                        QuestionId = questionId,
                        Input = new QnA.Api.Types.Page.Input
                        {
                            Options = new List<Option>
                            {
                                new Option
                                {
                                    Value = "Yes",
                                    FurtherQuestions = new List<QnA.Api.Types.Page.Question>
                                    {
                                        new QnA.Api.Types.Page.Question
                                        {
                                            QuestionId = furtherQuestionId
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                PageOfAnswers = new List<QnA.Api.Types.Page.PageOfAnswers>
                {
                    new QnA.Api.Types.Page.PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<QnA.Api.Types.Page.Answer>
                        {
                            new QnA.Api.Types.Page.Answer
                            {
                                QuestionId = questionId,
                                Value = "Yes"
                            },
                            new QnA.Api.Types.Page.Answer
                            {
                                QuestionId = furtherQuestionId,
                                Value = "Lorem ipsum"
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(criminalCompliancePage);
            
            var viewModel = _orchestrator.GetCriminalComplianceCheckViewModel(new GetCriminalComplianceCheckRequest(Guid.NewGuid(), "1234", "user")).GetAwaiter().GetResult();
            
            viewModel.PageId.Should().Be(pageId);
            viewModel.QuestionText.Should().Be("Question text");
            viewModel.Ukprn.Should().Be(ukrlpDetails.UKPRN);
            viewModel.ApplyLegalName.Should().Be(ukrlpDetails.ProviderName);
            viewModel.ComplianceCheckQuestionId.Should().Be(questionId);
            viewModel.ComplianceCheckAnswer.Should().Be("Yes");
            viewModel.FurtherInformationQuestionId.Should().Be(furtherQuestionId);
            viewModel.FurtherInformationAnswer.Should().Be("Lorem ipsum");
        }
    }
}
