﻿using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Apply;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Apply
{
    public class ApplicationControllerTests
    {
        private ApplicationController _controller;

        private Mock<IApiClient> _apiClient;
        private Mock<IApplicationApiClient> _applyApiClient;
        private Mock<IQnaApiClient> _qnaApiClient;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<IAnswerService> _answerService;
        private Mock<IAnswerInjectionService> _answerInjectionService;
        private Mock<ILogger<ApplicationController>> _logger;

        [SetUp]
        public void Setup()
        {
            _apiClient = new Mock<IApiClient>();
            _applyApiClient = new Mock<IApplicationApiClient>();
            _qnaApiClient = new Mock<IQnaApiClient>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _answerService = new Mock<IAnswerService>();
            _answerInjectionService = new Mock<IAnswerInjectionService>();
            _logger = new Mock<ILogger<ApplicationController>>();

            var identity = new GenericIdentity("test user");
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "JOHN"));
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "DUNHILL"));

            _httpContextAccessor.Setup(a => a.HttpContext.User.Identities)
               .Returns(new List<ClaimsIdentity>() { identity });

            _controller = new ApplicationController(_apiClient.Object, _applyApiClient.Object, _qnaApiClient.Object, _httpContextAccessor.Object, _answerService.Object, _answerInjectionService.Object, _logger.Object);
        }

        [Test]
        public async Task When_RequestingWithdrawalDateCheckPage_Then_WithdrawalDateCheckViewIsReturned()
        {
            // Arrange

            ArrangeMocksWithIrrelevantData();
            var applicationId = Guid.NewGuid();
            var sequenceNumber = 0;
            var backModel = new BackViewModel();
            var currentVersionIndex = 0;

            // Act

            ViewResult viewResult = await _controller.WithdrawalDateCheck(applicationId, sequenceNumber, backModel, currentVersionIndex) as ViewResult;

            // Assert

            viewResult.ViewName.Should().Be("WithdrawalDateCheck");
        }

        [Test]
        public async Task When_RequestingWithdrawalDateChangePageForStandardWithdrawal_Then_RedirectToConfirmation()
        {
            // Arrange

            ArrangeMocksWithIrrelevantData();
            var applicationId = Guid.NewGuid();
            var sequenceNumber = 0;
            var backModel = new BackViewModel();
            var effectiveToDay = "01";
            var effectiveToMonth = "01";
            var effectiveToYear = "2021";
            var currentVersionIndex = 0;

            // Act

            var viewResult = await _controller.WithdrawalDateChange(applicationId, sequenceNumber, backModel, effectiveToDay, effectiveToMonth, effectiveToYear, currentVersionIndex);

            // Assert

            viewResult.Should().NotBeNull();
            viewResult.Should().BeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)viewResult).ActionName.Should().Be("Assessment");  // Confirmation page is generated by the Assessment action
        }

        [Test]
        public async Task When_PostingWithdrawalDateCheckSaveForOrganisationWithdrawal_Then_RedirectToConfirmation()
        {
            // Arrange
            ArrangeMocksWithIrrelevantData();
            var withdrawalDate = new DateTime(2021, 10, 1);
            var applicationId = Guid.NewGuid();
            var sequenceNumber = 0;
            var backModel = new BackViewModel();
            var dateApproved = "YES";
            var currentVersionIndex = 0;

            _qnaApiClient.Setup(x => x.GetSequence(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new QnA.Api.Types.Sequence()
                {
                    SequenceNo = ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO,
                });

            _qnaApiClient.Setup(x => x.GetSections(It.IsAny<Guid>(), It.IsAny<Guid>()))
               .ReturnsAsync(new List<QnA.Api.Types.Section>()
               {
                    new QnA.Api.Types.Section()
                    {
                        SequenceNo = ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO,
                        QnAData = new QnA.Api.Types.Page.QnAData()
                        {
                            Pages = new List<QnA.Api.Types.Page.Page>()
                            {
                                new QnA.Api.Types.Page.Page()
                                {
                                    LinkTitle = "WITHDRAWAL DATE",
                                    PageOfAnswers = new List<QnA.Api.Types.Page.PageOfAnswers>()
                                    {
                                        new QnA.Api.Types.Page.PageOfAnswers()
                                        {
                                            Answers = new List<QnA.Api.Types.Page.Answer>()
                                            {
                                                new QnA.Api.Types.Page.Answer()
                                                {
                                                    Value = withdrawalDate.ToString()
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
               });

            // Act
            var viewResult = await _controller.WithdrawalDateCheckSave(applicationId, sequenceNumber, backModel, dateApproved, currentVersionIndex);

            // Assert
            _apiClient.Verify(m => m.WithdrawOrganisation(It.Is<WithdrawOrganisationRequest>(x => x.WithdrawalDate == withdrawalDate &&
                                                                                                   x.UpdatedBy == "JOHN DUNHILL")));
        }

        private void ArrangeMocksWithIrrelevantData()
        {
            _applyApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>()))
                .ReturnsAsync(new ApplicationResponse()
                {
                    ApplyData = new AssessorService.ApplyTypes.ApplyData()
                    {
                        Apply = new AssessorService.ApplyTypes.Apply()
                        {
                            ReferenceNumber = "123456",
                        },
                        Sequences = new List<ApplySequence>
                        {
                            new ApplySequence
                            {
                                SequenceNo = 5,
                                IsActive = true,
                                NotRequired = false,
                                Sections = new List<ApplySection>()
                                {
                                    new ApplySection()
                                    {
                                        NotRequired = false,
                                    }
                                }
                            }
                        }
                    }
                });

            _apiClient.Setup(x => x.GetOrganisation(It.IsAny<Guid>()))
                .ReturnsAsync(new Organisation()
                {
                    OrganisationData = new OrganisationData()
                    {
                        LegalName = "Test Organisation",
                    }
                });

            _qnaApiClient.Setup(x => x.GetSequence(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new QnA.Api.Types.Sequence()
                {

                });

            _qnaApiClient.Setup(x => x.GetSections(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<QnA.Api.Types.Section>()
                {
                    new QnA.Api.Types.Section()
                    {
                    }
                });
        }
    }
}