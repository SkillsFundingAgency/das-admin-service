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
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
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
