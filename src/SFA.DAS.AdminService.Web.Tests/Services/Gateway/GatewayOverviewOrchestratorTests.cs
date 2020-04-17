using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AdminService.Web.Tests.Services.Gateway
{
    [TestFixture]
    public class GatewayOverviewOrchestratorTests
    {
        private GatewayOverviewOrchestrator _orchestrator;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<ILogger<GatewayOverviewOrchestrator>> _logger;
        private Mock<IGatewaySectionsNotRequiredService> _sectionsNotRequiredService;

        private static string UserName = "GatewayUser";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _logger = new Mock<ILogger<GatewayOverviewOrchestrator>>();
            _sectionsNotRequiredService = new Mock<IGatewaySectionsNotRequiredService>();
            _orchestrator = new GatewayOverviewOrchestrator(_applyApiClient.Object, _logger.Object, _sectionsNotRequiredService.Object);
        }

        [TestCase("12345678", "John Ltd.", SectionReviewStatus.Pass, "Very good.")]
        [TestCase("87654321", "Simon Ltd.", SectionReviewStatus.Fail, "Not so good.")]
        [TestCase("12344321", "Frank Ltd.", SectionReviewStatus.NotRequired, null)]
        public void GetConfirmOverviewViewModel_returns_model(string ukprn, string organisationName, string sectionReviewStatus, string comment)
        {
            var applicationId = Guid.NewGuid();
            var gatewayReviewStatus = GatewayReviewStatus.InProgress;

            var applyData = new RoatpApplyData
            {
                ApplyDetails = new RoatpApplyDetails
                {
                    UKPRN = ukprn,
                    OrganisationName = organisationName
                }
            };

            var returnedRoatpApplicationResponse = new RoatpApplicationResponse 
                                                        { 
                                                            ApplicationId = applicationId, 
                                                            ApplyData = applyData, 
                                                            GatewayReviewStatus = GatewayReviewStatus.InProgress 
                                                        };

            _applyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(returnedRoatpApplicationResponse);

            var returnedGatewayPageAnswers = new List<GatewayPageAnswerSummary>
            {
                new GatewayPageAnswerSummary
                { 
                    ApplicationId = applicationId, 
                    PageId = GatewayPageIds.OrganisationRisk, 
                    Status = sectionReviewStatus, 
                    Comments = comment 
                }
            };

            _applyApiClient.Setup(x => x.GetGatewayPageAnswers(applicationId)).ReturnsAsync(returnedGatewayPageAnswers);

            var request = new GetApplicationOverviewRequest(applicationId, UserName);

            var response = _orchestrator.GetConfirmOverviewViewModel(request);

            var viewModel = response.Result;

            Assert.AreEqual(applicationId, viewModel.ApplicationId);
            Assert.AreEqual(ukprn, viewModel.Ukprn);
            Assert.AreEqual(organisationName, viewModel.OrganisationName);
            Assert.AreEqual(gatewayReviewStatus, viewModel.GatewayReviewStatus);
            Assert.AreEqual(sectionReviewStatus, viewModel.Sequences.FirstOrDefault(seq => seq.SequenceNumber == 1).Sections.FirstOrDefault(sec => sec.PageId == GatewayPageIds.OrganisationRisk).Status);
            Assert.AreEqual(comment, viewModel.Sequences.FirstOrDefault(seq => seq.SequenceNumber == 1).Sections.FirstOrDefault(sec => sec.PageId == GatewayPageIds.OrganisationRisk).Comment);
        }

        [TestCase("12345678", "John Ltd.")]
        [TestCase("87654321", "Simon Ltd.")]
        [TestCase("12344321", "Frank Ltd.")]
        public void GetConfirmOverviewViewModel_returns_model_no_saved_statuses(string ukprn, string organisationName)
        {
            var applicationId = Guid.NewGuid();
            var gatewayReviewStatus = GatewayReviewStatus.InProgress;

            var applyData = new RoatpApplyData
            {
                ApplyDetails = new RoatpApplyDetails
                {
                    UKPRN = ukprn,
                    OrganisationName = organisationName
                }
            };

            var returnedRoatpApplicationResponse = new RoatpApplicationResponse
            {
                ApplicationId = applicationId,
                ApplyData = applyData,
                GatewayReviewStatus = GatewayReviewStatus.InProgress
            };

            _applyApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(returnedRoatpApplicationResponse);

            // No Saved Statusses
            var returnedGatewayPageAnswers = new List<GatewayPageAnswerSummary>();

            _applyApiClient.Setup(x => x.GetGatewayPageAnswers(applicationId)).ReturnsAsync(returnedGatewayPageAnswers);

            var request = new GetApplicationOverviewRequest(applicationId, UserName);

            var response = _orchestrator.GetConfirmOverviewViewModel(request);

            var viewModel = response.Result;

            viewModel.ApplicationId.Should().Be(applicationId);
            viewModel.Ukprn.Should().Be(ukprn);
            viewModel.OrganisationName.Should().Be(organisationName);
            viewModel.GatewayReviewStatus.Should().Be(gatewayReviewStatus);
            viewModel.ReadyToConfirm.Should().Be(false);
        }

        private RoatpGatewayApplicationViewModel ProcessViewModelOnError(Guid applicationId, string gatewayReviewStatus, string field, string errorMessage)
        {
            var viewModelOnError = new RoatpGatewayApplicationViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = gatewayReviewStatus
            };

            var viewModel = new RoatpGatewayApplicationViewModel
            {
                ApplicationId = applicationId,
                GatewayReviewStatus = gatewayReviewStatus
            };

            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>
                {
                    new ValidationErrorDetail
                    {
                        Field = field,
                        ErrorMessage = errorMessage
                    }
                }
            };

            _orchestrator.ProcessViewModelOnError(viewModelOnError, viewModel, validationResponse);

            return viewModelOnError;
        }

        [TestCase(GatewayReviewStatus.InProgress, "GatewayReviewStatus", "Error - GatewayReviewStatus")]
        public void ProcessViewModelOnError_process_view_model_correctly_GatewayReviewStatus(string gatewayReviewStatus, string field, string errorMessage)
        {
            var applicationId = Guid.NewGuid();
            var viewModelOnError = ProcessViewModelOnError(applicationId, gatewayReviewStatus, field, errorMessage);

            viewModelOnError.ApplicationId.Should().Be(applicationId);
            viewModelOnError.GatewayReviewStatus.Should().Be(gatewayReviewStatus);
            viewModelOnError.ErrorTextGatewayReviewStatus.Should().Be(errorMessage);
        }

        [TestCase(GatewayReviewStatus.AskForClarification, "OptionAskClarificationText", "Error - Ask forClarification")]
        public void ProcessViewModelOnError_process_view_model_correctly_OptionAskClarificationText(string gatewayReviewStatus, string field, string errorMessage)
        {
            var applicationId = Guid.NewGuid();
            var viewModelOnError = ProcessViewModelOnError(applicationId, gatewayReviewStatus, field, errorMessage);

            viewModelOnError.ApplicationId.Should().Be(applicationId);
            viewModelOnError.GatewayReviewStatus.Should().Be(gatewayReviewStatus);
            viewModelOnError.ErrorTextAskClarification.Should().Be(errorMessage);
        }

        [TestCase(GatewayReviewStatus.Decline, "OptionDeclinedText", "Error - Decline")]
        public void ProcessViewModelOnError_process_view_model_correctly_OptionDeclinedText(string gatewayReviewStatus, string field, string errorMessage)
        {
            var applicationId = Guid.NewGuid();
            var viewModelOnError = ProcessViewModelOnError(applicationId, gatewayReviewStatus, field, errorMessage);

            viewModelOnError.ApplicationId.Should().Be(applicationId);
            viewModelOnError.GatewayReviewStatus.Should().Be(gatewayReviewStatus);
            viewModelOnError.ErrorTextDeclined.Should().Be(errorMessage);
        }

        [TestCase(GatewayReviewStatus.Pass, "OptionApprovedText", "Error - Pass")]
        public void ProcessViewModelOnError_process_view_model_correctly_OptionApprovedText(string gatewayReviewStatus, string field, string errorMessage)
        {
            var applicationId = Guid.NewGuid();
            var viewModelOnError = ProcessViewModelOnError(applicationId, gatewayReviewStatus, field, errorMessage);

            viewModelOnError.ApplicationId.Should().Be(applicationId);
            viewModelOnError.GatewayReviewStatus.Should().Be(gatewayReviewStatus);
            viewModelOnError.ErrorTextApproved.Should().Be(errorMessage);
        }
    }
}
