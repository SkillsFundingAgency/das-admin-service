using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Handlers.Gateway;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Handlers
{
    [TestFixture]
    public class GetCriminalComplianceCheckHandlerTests
    {
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IQnaApiClient> _qnaApiClient;
        private Mock<ICriminalComplianceChecksQuestionLookupService> _lookupService;
        private Mock<ILogger<GetCriminalComplianceCheckHandler>> _logger;
        private GetCriminalComplianceCheckHandler _handler;

        [SetUp]
        public void Before_each_test()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _qnaApiClient = new Mock<IQnaApiClient>();
            _lookupService = new Mock<ICriminalComplianceChecksQuestionLookupService>();
            _logger = new Mock<ILogger<GetCriminalComplianceCheckHandler>>();

            _handler = new GetCriminalComplianceCheckHandler(_applyApiClient.Object, _qnaApiClient.Object, _lookupService.Object, _logger.Object);
        }

        [Test]
        public void Handler_generates_view_model_if_not_already_stored_in_database()
        {
            var pageId = "1234";
            var questionId = "CC-11";
            var furtherQuestionId = "CC-11.1";

            GatewayPageAnswer gatewayPageAnswer = null;

            _applyApiClient.Setup(x => x.GetGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(gatewayPageAnswer);

            var application = new RoatpApplicationResponse
            {
                GatewayReviewStatus = GatewayReviewStatus.New
            };

            _applyApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);

            var ukprnAnswer = new QnA.Api.Types.Page.Answer { Value = "10001234" };
            _qnaApiClient.Setup(x => x.GetQuestionTag(It.IsAny<Guid>(), RoatpQnaConstants.QnaQuestionTags.Ukprn)).ReturnsAsync(ukprnAnswer.Value);

            var legalNameAnswer = new QnA.Api.Types.Page.Answer { Value = "Legal Name" };
            _qnaApiClient.Setup(x => x.GetQuestionTag(It.IsAny<Guid>(), RoatpQnaConstants.QnaQuestionTags.UKRLPLegalName)).ReturnsAsync(legalNameAnswer.Value);

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

            var viewModel = _handler.Handle(new GetCriminalComplianceCheckRequest(Guid.NewGuid(), "1234", "user"), new System.Threading.CancellationToken()).GetAwaiter().GetResult();

            viewModel.PageId.Should().Be(pageId);
            viewModel.QuestionText.Should().Be("Question text");
            viewModel.Ukprn.Should().Be(ukprnAnswer.Value);
            viewModel.ApplyLegalName.Should().Be(legalNameAnswer.Value);
            viewModel.ComplianceCheckQuestionId.Should().Be(questionId);
            viewModel.ComplianceCheckAnswer.Should().Be("Yes");
            viewModel.FurtherInformationQuestionId.Should().Be(furtherQuestionId);
            viewModel.FurtherInformationAnswer.Should().Be("Lorem ipsum");
        }

        [Test]
        public void Handler_retrieves_view_model_from_database_if_page_already_visited_previously()
        {
            var model = new OrganisationCriminalCompliancePageViewModel
            {
                ComplianceCheckQuestionId = "CC-11",
                ComplianceCheckAnswer = "No"
            };
            var jsonData = JsonConvert.SerializeObject(model);

            var gatewayPageAnswer = new GatewayPageAnswer
            {
                ApplicationId = Guid.NewGuid(),
                PageId = "1234",
                GatewayPageData = jsonData
            };

            _applyApiClient.Setup(x => x.GetGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(gatewayPageAnswer);

            var applicationDetails = new RoatpApplicationResponse
            {
                GatewayReviewStatus = GatewayReviewStatus.InProgress
            };

            _applyApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(applicationDetails);

            var viewModel = _handler.Handle(new GetCriminalComplianceCheckRequest(Guid.NewGuid(), "1234", "user"), new System.Threading.CancellationToken()).GetAwaiter().GetResult();

            viewModel.ComplianceCheckQuestionId.Should().Be(model.ComplianceCheckQuestionId);
            viewModel.ComplianceCheckAnswer.Should().Be(model.ComplianceCheckAnswer);
            viewModel.FurtherInformationQuestionId.Should().BeNull();
            viewModel.FurtherInformationAnswer.Should().BeNull();

        }
    }
}
