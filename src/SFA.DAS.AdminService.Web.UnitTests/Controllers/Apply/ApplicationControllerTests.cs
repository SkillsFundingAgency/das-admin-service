using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AdminService.Web.Controllers.Apply;
using SFA.DAS.AdminService.Web.Domain.Apply;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Apply
{
    public class ApplicationControllerTests
    {
        private ApplicationController _controller;

        private Mock<IApplicationApiClient> _applyApiClient;
        private Mock<IOrganisationsApiClient> _organisationsApiClient;
        private Mock<IQnaApiClient> _qnaApiClient;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<IAnswerService> _answerService;
        private Mock<IAnswerInjectionService> _answerInjectionService;
        private Mock<ILogger<ApplicationController>> _logger;

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IApplicationApiClient>();
            _organisationsApiClient = new Mock<IOrganisationsApiClient>();
            _qnaApiClient = new Mock<IQnaApiClient>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _answerService = new Mock<IAnswerService>();
            _answerInjectionService = new Mock<IAnswerInjectionService>();
            _logger = new Mock<ILogger<ApplicationController>>();

            var identity = new GenericIdentity("UserName");
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "User"));
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "Name"));

            _httpContextAccessor.Setup(a => a.HttpContext.User.Identities)
               .Returns(new List<ClaimsIdentity>() { identity });

            _controller = new ApplicationController(_applyApiClient.Object, _organisationsApiClient.Object, _qnaApiClient.Object, _httpContextAccessor.Object, _answerService.Object, _answerInjectionService.Object, _logger.Object);
        }

        [Test]
        public async Task When_RequestingWithdrawalDateCheck_ThenReturnCorrectView()
        {
            // Arrange
            var id = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var sequenceNumber = ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO;
            var backModel = new BackViewModel();
            
            ArrangeMocksWithQnAData(id, applicationId, sequenceNumber, null, ApplicationSectionStatus.Evaluated);

            _qnaApiClient.Setup(x => x.GetApplicationDataDictionary(It.IsAny<Guid>()))
                .ReturnsAsync(new Dictionary<string, object> { { nameof(ApplicationData.ConfirmedWithdrawalDate), null } });

            // Act
            var viewResult = await _controller.WithdrawalDateCheck(applicationId, sequenceNumber, backModel);

            // Assert
            viewResult.Should().NotBeNull();
            viewResult.Should().BeOfType<ViewResult>();
            ((ViewResult)viewResult).ViewName.Should().Be(nameof(ApplicationController.WithdrawalDateCheck));
        }

        [Test]
        public async Task When_PostingWithdrawalDateChange_Then_UpdateApplicationDataForChangedDate()
        {
            // Arrange
            var id = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var sequenceNumber = ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO;
            var backModel = new BackViewModel();
            var effectiveTo = new DateTime(2021, 01, 01);

            ArrangeMocksWithQnAData(id, applicationId, sequenceNumber, null, ApplicationSectionStatus.Evaluated);

            _qnaApiClient.Setup(x => x.GetApplicationDataDictionary(It.IsAny<Guid>()))
                .ReturnsAsync(new Dictionary<string, object> { { nameof(ApplicationData.ConfirmedWithdrawalDate), null } });

            // Act
            var viewResult = await _controller.WithdrawalDateChange(applicationId, sequenceNumber, backModel, effectiveTo.Day.ToString(), effectiveTo.Month.ToString(), effectiveTo.Year.ToString());

            // Assert
            string specificKey = nameof(ApplicationData.ConfirmedWithdrawalDate);
            object expectedValue = effectiveTo;

            _qnaApiClient.Verify(m => m.UpdateApplicationDataDictionary(applicationId, It.Is<Dictionary<string, object>>(x => x.ContainsKey(specificKey) && x[specificKey].Equals(expectedValue))));
        }

        [Test]
        public async Task When_PostingWithdrawalDateChange_Then_RedirectToConfirmation()
        {
            // Arrange
            var id = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var sequenceNumber = ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO;
            var backModel = new BackViewModel();
            var effectiveTo = new DateTime(2021, 01, 01);

            ArrangeMocksWithQnAData(id, applicationId, sequenceNumber, null, ApplicationSectionStatus.Evaluated);

            _qnaApiClient.Setup(x => x.GetApplicationDataDictionary(It.IsAny<Guid>()))
                .ReturnsAsync(new Dictionary<string, object> { { nameof(ApplicationData.ConfirmedWithdrawalDate), null } });

            // Act
            var viewResult = await _controller.WithdrawalDateChange(applicationId, sequenceNumber, backModel, effectiveTo.Day.ToString(), effectiveTo.Month.ToString(), effectiveTo.Year.ToString());

            // Assert
            viewResult.Should().NotBeNull();
            viewResult.Should().BeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)viewResult).ActionName.Should().Be("Assessment");  // Confirmation page is generated by the Assessment action
        }

        [Test]
        public async Task When_PostingWithdrawalDateCheckSave_Then_UpdateDataDictionaryWithChangedDate()
        {
            // Arrange
            var id = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var sequenceNumber = ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO;
            var withdrawalDate = new DateTime(2021, 10, 1);
            var backModel = new BackViewModel();
            var dateApproved = "YES";

            ArrangeMocksWithQnAData(id, applicationId, sequenceNumber, null, ApplicationSectionStatus.Evaluated);

            _qnaApiClient.Setup(x => x.GetApplicationDataDictionary(It.IsAny<Guid>()))
                .ReturnsAsync(new Dictionary<string, object> { { nameof(ApplicationData.ConfirmedWithdrawalDate), null } });

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
            var viewResult = await _controller.WithdrawalDateCheckSave(applicationId, sequenceNumber, backModel, dateApproved);

            // Assert
            string specificKey = nameof(ApplicationData.ConfirmedWithdrawalDate);
            object expectedValue = withdrawalDate;

            _qnaApiClient.Verify(m => m.UpdateApplicationDataDictionary(applicationId, It.Is<Dictionary<string, object>>(x => x.ContainsKey(specificKey) && x[specificKey].Equals(expectedValue))));
        }

        [Test]
        public async Task When_PostingWithdrawalDateCheckSave_Then_RedirectToConfirmation()
        {
            // Arrange
            var id = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var sequenceNumber = ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO;
            var withdrawalDate = new DateTime(2021, 10, 1);
            var backModel = new BackViewModel();
            var dateApproved = "YES";

            ArrangeMocksWithQnAData(id, applicationId, sequenceNumber, null, ApplicationSectionStatus.Evaluated);

            _qnaApiClient.Setup(x => x.GetApplicationDataDictionary(It.IsAny<Guid>()))
                .ReturnsAsync(new Dictionary<string, object> { { nameof(ApplicationData.ConfirmedWithdrawalDate), null } });

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
            var viewResult = await _controller.WithdrawalDateCheckSave(applicationId, sequenceNumber, backModel, dateApproved);

            // Assert
            viewResult.Should().NotBeNull();
            viewResult.Should().BeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)viewResult).ActionName.Should().Be("Assessment");
        }

        [Test]
        public async Task When_PostingReturnForWithdrawOrganisationSequence_Then_CallsWithdrawOrganisationAndReturnApplication()
        {
            // Arrange
            var id = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var sequenceNumber = ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO;
            var backModel = new BackViewModel();

            var withdrawOrganisationRequest = new WithdrawOrganisationRequest
            {
                ApplicationId = id,
                EndPointAssessorOrganisationId = "EPA0001",
                WithdrawalDate = DateTime.Now.Date.AddMonths(6),
                UpdatedBy = "User Name"
            };

            _answerService.Setup(x => x.GatherAnswersForWithdrawOrganisationForApplication(id, "User Name"))
                .ReturnsAsync(withdrawOrganisationRequest);

            ArrangeMocksWithQnAData(id, applicationId, sequenceNumber, null, ApplicationSectionStatus.Evaluated);

            // Act
            var viewResult = await _controller.Return(applicationId, sequenceNumber, ReturnTypes.Approve, backModel);

            // Assert
            _organisationsApiClient.Verify(x => x.WithdrawOrganisation(withdrawOrganisationRequest), Times.Once);
            _applyApiClient.Verify(x => x.ReturnApplicationSequence(id, sequenceNumber, ReturnTypes.Approve, "User Name"));
        }

        [Test]
        public async Task When_PostingReturnForWithdrawOrganisationSequence_Then_ShowsReturnedView()
        {
            // Arrange
            var id = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var sequenceNumber = ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO;
            var backModel = new BackViewModel();

            var withdrawOrganisationRequest = new WithdrawOrganisationRequest
            {
                ApplicationId = id,
                EndPointAssessorOrganisationId = "EPA0001",
                WithdrawalDate = DateTime.Now.Date.AddMonths(6),
                UpdatedBy = "User Name"
            };

            _answerService.Setup(x => x.GatherAnswersForWithdrawOrganisationForApplication(id, "User Name"))
                .ReturnsAsync(withdrawOrganisationRequest);

            ArrangeMocksWithQnAData(id, applicationId, sequenceNumber, null, ApplicationSectionStatus.Evaluated);

            // Act
            var viewResult = await _controller.Return(applicationId, sequenceNumber, ReturnTypes.Approve, backModel);

            // Assert
            _organisationsApiClient.Verify(x => x.WithdrawOrganisation(withdrawOrganisationRequest), Times.Once);
            _applyApiClient.Verify(x => x.ReturnApplicationSequence(id, sequenceNumber, ReturnTypes.Approve, "User Name"));

            viewResult.Should().NotBeNull();
            viewResult.Should().BeOfType<ViewResult>();
            ((ViewResult)viewResult).ViewName.Should().Be("Returned");
        }

        [Test]
        public async Task When_PostingReturnForWithdrawStandardSequence_Then_CallsWithdrawStandardAndReturnApplication()
        {
            // Arrange
            var id = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var sequenceNumber = ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO;
            var backModel = new BackViewModel();
            var standardCode = 123;

            var withdrawStandardRequest = new WithdrawStandardRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardCode = standardCode,
                WithdrawalDate = DateTime.Now.Date.AddMonths(6)
            };

            _answerService.Setup(x => x.GatherAnswersForWithdrawStandardForApplication(id))
                .ReturnsAsync(withdrawStandardRequest);

            ArrangeMocksWithQnAData(id, applicationId, sequenceNumber, standardCode, ApplicationSectionStatus.Evaluated);

            // Act
            var viewResult = await _controller.Return(applicationId, sequenceNumber, ReturnTypes.Approve, backModel);

            // Assert
            _organisationsApiClient.Verify(x => x.WithdrawStandard(withdrawStandardRequest), Times.Once);
            _applyApiClient.Verify(x => x.ReturnApplicationSequence(id, sequenceNumber, ReturnTypes.Approve, "User Name"));
        }

        [Test]
        public async Task When_PostingReturnForWithdrawStandardSequence_Then_ShowsReturnedView()
        {
            // Arrange
            var id = Guid.NewGuid();
            var applicationId = Guid.NewGuid();
            var sequenceNumber = ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO;
            var backModel = new BackViewModel();
            var standardCode = 123;

            var withdrawStandardRequest = new WithdrawStandardRequest
            {
                EndPointAssessorOrganisationId = "EPA0001",
                StandardCode = standardCode,
                WithdrawalDate = DateTime.Now.Date.AddMonths(6)
            };

            _answerService.Setup(x => x.GatherAnswersForWithdrawStandardForApplication(id))
                .ReturnsAsync(withdrawStandardRequest);

            ArrangeMocksWithQnAData(id, applicationId, sequenceNumber, standardCode, ApplicationSectionStatus.Evaluated);

            // Act
            var viewResult = await _controller.Return(applicationId, sequenceNumber, ReturnTypes.Approve, backModel);

            // Assert
            viewResult.Should().NotBeNull();
            viewResult.Should().BeOfType<ViewResult>();
            ((ViewResult)viewResult).ViewName.Should().Be("Returned");
        }

        private void ArrangeMocksWithQnAData(Guid id, Guid applicationId, int sequenceNumber, int? standardCode, string applicationSectionStatus)
        {
            _applyApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>()))
                .ReturnsAsync(new ApplicationResponse()
                {
                    Id = id,
                    ApplicationId = applicationId,
                    ApplyData = new ApplyData()
                    {
                        Apply = new ApplyInfo()
                        {
                            ReferenceNumber = "123456",
                            StandardCode = standardCode
                        },
                        Sequences = new List<ApplySequence>
                        {
                            new ApplySequence
                            {
                                SequenceNo = sequenceNumber,
                                IsActive = true,
                                NotRequired = false,
                                Sections = new List<ApplySection>()
                                {
                                    new ApplySection()
                                    {
                                        NotRequired = false,
                                        Status = applicationSectionStatus
                                    }
                                }
                            }
                        }
                    }
                });

            _organisationsApiClient.Setup(x => x.Get(It.IsAny<Guid>()))
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