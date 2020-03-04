using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using ApplicationStatus = SFA.DAS.AssessorService.ApplyTypes.Roatp.ApplicationStatus;
using FinancialReviewStatus = SFA.DAS.AssessorService.ApplyTypes.Roatp.FinancialReviewStatus;

namespace SFA.DAS.AdminService.Web.Tests.Services
{
    [TestFixture]
    public class RoatpApplicationApprovalServiceTests
    {
        private RoatpApplicationApprovalService _service;
        private List<RoatpApplySequence> _sequences;
        private Mock<IQnaApiClient> _qnaApiClient;
        private Mock<IRoatpApplicationApiClient> _roatpApplicationApiClient;
        private Mock<IRoatpApiClient> _roatpApiClient;
        private List<ProviderType> _providerTypes;
        private List<OrganisationType> _organisationTypes;

        [SetUp]
        public void Before_each_test()
        {
            _qnaApiClient = new Mock<IQnaApiClient>();
            _roatpApplicationApiClient = new Mock<IRoatpApplicationApiClient>();
            _roatpApiClient = new Mock<IRoatpApiClient>();

            _providerTypes = new List<ProviderType> 
            {
                new ProviderType
                {
                    Id = 1,
                    Type = "Main"
                },
                new ProviderType
                {
                    Id = 2,
                    Type = "Employer"
                },
                new ProviderType
                {
                    Id = 3,
                    Type = "Supporting"
                }
            };
            _roatpApiClient.Setup(x => x.GetProviderTypes()).ReturnsAsync(_providerTypes);

            _service = new RoatpApplicationApprovalService(_qnaApiClient.Object, _roatpApplicationApiClient.Object, _roatpApiClient.Object);

            _sequences = new List<RoatpApplySequence>
            {
                new RoatpApplySequence
                {
                    NotRequired = false,
                    Status = SequenceReviewStatus.Evaluated,
                    Sections = new List<RoatpApplySection>
                    {
                        new RoatpApplySection
                        {
                            SectionNo = 1,
                            Status = AssessorReviewStatus.Approved
                        }
                    }
                }
            };

            _organisationTypes = new List<OrganisationType>
            {
                new OrganisationType
                {
                    Id = 1,
                    Type = "School"
                },
                new OrganisationType
                {
                    Id = 2,
                    Type = "General Further Education College"
                },
                new OrganisationType
                {
                    Id = 3,
                    Type = "National College"
                },
                new OrganisationType
                {
                    Id = 4,
                    Type = "Sixth Form College"
                },
                new OrganisationType
                {
                    Id = 5,
                    Type = "Further Education Institute"
                },
                new OrganisationType
                {
                    Id = 6,
                    Type = "Higher Education Institute"
                },
                new OrganisationType
                {
                    Id = 7,
                    Type = "Academy"
                },
                new OrganisationType
                {
                    Id = 8,
                    Type = "Multi-Academy Trust"
                },
                new OrganisationType
                {
                    Id = 9,
                    Type = "NHS Trust"
                },
                new OrganisationType
                {
                    Id = 10,
                    Type = "Police"
                },
                new OrganisationType
                {
                    Id = 11,
                    Type = "Fire authority"
                },
                new OrganisationType
                {
                    Id = 12,
                    Type = "Local authority"
                },
                new OrganisationType
                {
                    Id = 13,
                    Type = "Government department"
                },
                new OrganisationType
                {
                    Id = 14,
                    Type = "Non departmental public body (NDPB)"
                },
                new OrganisationType
                {
                    Id = 15,
                    Type = "Executive agency"
                },
                new OrganisationType
                {
                    Id = 16,
                    Type = "An Independent Training Provider"
                },
                new OrganisationType
                {
                    Id = 17,
                    Type = "An Apprenticeship Training Agency"
                },
                new OrganisationType
                {
                    Id = 18,
                    Type = "A Group Training Association"
                },
                new OrganisationType
                {
                    Id = 19,
                    Type = "An employer training apprentices in other organisations"
                },
                new OrganisationType
                {
                    Id = 20,
                    Type = "None of the above"
                }
            };
            _roatpApiClient.Setup(x => x.GetOrganisationTypes(It.IsAny<int>())).ReturnsAsync(_organisationTypes);

        }

        [TestCase(ApplicationStatus.Submitted)]
        [TestCase(ApplicationStatus.Resubmitted)]
        [TestCase(ApplicationStatus.Rejected)]
        [TestCase(ApplicationStatus.New)]
        [TestCase(ApplicationStatus.InProgress)]
        [TestCase(ApplicationStatus.Cancelled)]
        [TestCase(ApplicationStatus.FeedbackAdded)]
        public void Organisiation_not_eligible_if_not_passed_gateway_checks(string gatewayStatus)
        {
            var eligible = _service.IsEligibleForRegister(gatewayStatus, FinancialReviewStatus.Approved, _sequences);

            eligible.Should().BeFalse();
        }

        [TestCase(FinancialReviewStatus.Declined)]
        [TestCase(FinancialReviewStatus.InProgress)]
        [TestCase(FinancialReviewStatus.New)]
        [TestCase(FinancialReviewStatus.Draft)]
        public void Organisation_not_eligible_if_not_passed_financial_health_checks(string financialHealthStatus)
        {
            var eligible = _service.IsEligibleForRegister(ApplicationStatus.GatewayAssessed, financialHealthStatus, _sequences);

            eligible.Should().BeFalse();
        }

        [TestCase(FinancialReviewStatus.Approved, AssessorReviewStatus.Declined)]
        [TestCase(FinancialReviewStatus.Exempt, AssessorReviewStatus.Declined)]
        [TestCase(FinancialReviewStatus.Approved, AssessorReviewStatus.Draft)]
        [TestCase(FinancialReviewStatus.Exempt, AssessorReviewStatus.Draft)]
        [TestCase(FinancialReviewStatus.Approved, AssessorReviewStatus.InProgress)]
        [TestCase(FinancialReviewStatus.Exempt, AssessorReviewStatus.InProgress)]
        [TestCase(FinancialReviewStatus.Approved, AssessorReviewStatus.HasFeedback)]
        [TestCase(FinancialReviewStatus.Exempt, AssessorReviewStatus.HasFeedback)]
        [TestCase(FinancialReviewStatus.Approved, AssessorReviewStatus.New)]
        [TestCase(FinancialReviewStatus.Exempt, AssessorReviewStatus.New)]
        public void Organisation_not_eligible_if_not_passed_all_assessor_sequence_checks(string financialHealthStatus, string assessorReviewStatus)
        {
            var sequences = new List<RoatpApplySequence>
            {
                new RoatpApplySequence
                {
                    SequenceNo = 1,
                    Status = SequenceReviewStatus.Evaluated
                },
                new RoatpApplySequence
                {
                    SequenceNo = 2,
                    Status = assessorReviewStatus
                }
            };

            var eligible = _service.IsEligibleForRegister(ApplicationStatus.GatewayAssessed, financialHealthStatus, sequences);

            eligible.Should().BeFalse();
        }

        [TestCase(ApplicationStatus.Approved, FinancialReviewStatus.Approved, SequenceReviewStatus.Evaluated)]
        [TestCase(ApplicationStatus.Approved, FinancialReviewStatus.Exempt, SequenceReviewStatus.Evaluated)]
        [TestCase(ApplicationStatus.Approved, FinancialReviewStatus.Approved, SequenceReviewStatus.NotRequired)]
        [TestCase(ApplicationStatus.Approved, FinancialReviewStatus.Exempt, SequenceReviewStatus.NotRequired)]
        public void Organisation_is_eligible_if_all_statuses_are_valid(string gatewayStatus, string financialHealthStatus, string assessorReviewStatus)
        {
            _sequences[0].Status = assessorReviewStatus;
            var eligible = _service.IsEligibleForRegister(gatewayStatus, financialHealthStatus, _sequences);

            eligible.Should().BeTrue();
        }

        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        public void Organisation_approval_model_is_built_with_correct_provider_route_description(int providerTypeId, int index)
        {
            var applicationId = Guid.NewGuid();

            var application = new RoatpApplicationResponse
            {
                ApplicationId = applicationId,
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails
                    {
                        ProviderRoute = providerTypeId
                    }
                }
            };
            _roatpApplicationApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);

            var companyNumber = "12345678";
            var preamblePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.Preamble.PageId,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UkrlpVerificationCompany,
                                Value = "TRUE"
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.CompanyNumber,
                                Value = companyNumber
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.LegalName,
                                Value = "Legal name"
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UKPRN,
                                Value = "10001234"
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.Preamble,
                                                          RoatpQnaConstants.RoatpSections.Preamble.SectionId, RoatpQnaConstants.RoatpSections.Preamble.PageId))
                                                         .ReturnsAsync(preamblePage);

            var organisationDetailsEmployerPage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeEmployer,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.OrganisationTypeEmployer,
                                Value = "An Independent Training Provider"
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(It.IsAny<Guid>(), RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeEmployer))
                                                         .ReturnsAsync(organisationDetailsEmployerPage);

            var organisationDetailsMainSupportingPage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeMainSupporting,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.OrganisationTypeMainSupporting,
                                Value = "An Independent Training Provider"
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(It.IsAny<Guid>(), RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeMainSupporting))
                                                         .ReturnsAsync(organisationDetailsMainSupportingPage);

            var model = _service.BuildApplicationApprovalViewModel(applicationId).GetAwaiter().GetResult();

            model.ProviderTypeId.Should().Be(application.ApplyData.ApplyDetails.ProviderRoute);
            model.ApplicationRoute.Should().Be(_providerTypes[index].Type);
        }

        [Test]
        public void Organisation_approval_model_built_for_companies_house_verification()
        {
            var applicationId = Guid.NewGuid();

            var application = new RoatpApplicationResponse
            {
                ApplicationId = applicationId,
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails
                    {
                        ProviderRoute = 3
                    }
                }
            };
            _roatpApplicationApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);

            var companyNumber = "12345678";
            var preamblePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.Preamble.PageId,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UkrlpVerificationCompany,
                                Value = "TRUE"
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.CompanyNumber,
                                Value = companyNumber
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.LegalName,
                                Value = "Legal name"
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UKPRN,
                                Value = "10001234"
                            }
                        }
                    }
                }
            };


            _qnaApiClient.Setup(x => x.GetPageBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.Preamble,
                                                          RoatpQnaConstants.RoatpSections.Preamble.SectionId, RoatpQnaConstants.RoatpSections.Preamble.PageId))
                                                         .ReturnsAsync(preamblePage);

            var organisationDetailsPage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeMainSupporting,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.OrganisationTypeMainSupporting,
                                Value = "An Independent Training Provider"
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeMainSupporting))
                                                         .ReturnsAsync(organisationDetailsPage);

            var model = _service.BuildApplicationApprovalViewModel(applicationId).GetAwaiter().GetResult();

            model.CompanyNumber.Should().Be(companyNumber);
        }

        [Test]
        public void Organisation_approval_model_built_for_charity_commission_verification()
        {
            var applicationId = Guid.NewGuid();

            var application = new RoatpApplicationResponse
            {
                ApplicationId = applicationId,
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails
                    {
                        ProviderRoute = 1
                    }
                }
            };
            _roatpApplicationApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);

            var charityNumber = "12345678";

            var preamblePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.Preamble.PageId,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UkrlpVerificationCharity,
                                Value = "TRUE"
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.CharityNumber,
                                Value = charityNumber
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.LegalName,
                                Value = "Legal name"
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.TradingName,
                                Value = "Trading name"
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UKPRN,
                                Value = "10001234"
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.Preamble,
                                                          RoatpQnaConstants.RoatpSections.Preamble.SectionId, RoatpQnaConstants.RoatpSections.Preamble.PageId))
                                                         .ReturnsAsync(preamblePage);
            var organisationDetailsPage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeMainSupporting,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.OrganisationTypeMainSupporting,
                                Value = "An Independent Training Provider"
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeMainSupporting))
                                                         .ReturnsAsync(organisationDetailsPage);

            var model = _service.BuildApplicationApprovalViewModel(applicationId).GetAwaiter().GetResult();

            model.CharityNumber.Should().Be(charityNumber);
        }

        [Test]
        public void Organisation_approval_model_built_for_non_company_or_charity()
        {
            var applicationId = Guid.NewGuid();

            var application = new RoatpApplicationResponse
            {
                ApplicationId = applicationId,
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails
                    {
                        ProviderRoute = 2
                    }
                }
            };
            _roatpApplicationApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);

            var ukprn = "10002000";
            var legalName = "Legal Name";
            var tradingName = "Trading Name";
            
            var preamblePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.Preamble.PageId,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UkrlpVerificationCompany,
                                Value = ""
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UkrlpVerificationCharity,
                                Value = ""
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UKPRN,
                                Value = ukprn
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.LegalName,
                                Value = legalName
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.TradingName,
                                Value = tradingName
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.Preamble,
                                                          RoatpQnaConstants.RoatpSections.Preamble.SectionId, RoatpQnaConstants.RoatpSections.Preamble.PageId))
                                                         .ReturnsAsync(preamblePage);
            
            var organisationDetailsPage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeEmployer,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.OrganisationTypeEmployer,
                                Value = "An Independent Training Provider"
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeEmployer))
                                                         .ReturnsAsync(organisationDetailsPage);

            var model = _service.BuildApplicationApprovalViewModel(applicationId).GetAwaiter().GetResult();

            model.UKPRN.Should().Be(ukprn);
            model.LegalName.Should().Be(legalName);
            model.TradingName.Should().Be(tradingName);
        }

        [TestCase("School", 1, 1)]
        [TestCase("General Further Education College", 2, 1)]
        [TestCase("National College", 3, 1)]
        [TestCase("Sixth Form College", 4, 1)]
        [TestCase("Further Education Institute", 5, 1)]
        [TestCase("Higher Education Institute", 6, 1)]
        [TestCase("Academy", 7, 1)]
        [TestCase("Multi-Academy Trust", 8, 1)]
        [TestCase("School", 1, 3)]
        [TestCase("General Further Education College", 2, 3)]
        [TestCase("National College", 3, 3)]
        [TestCase("Sixth Form College", 4, 3)]
        [TestCase("Further Education Institute", 5, 3)]
        [TestCase("Higher Education Institute", 6, 3)]
        [TestCase("Academy", 7, 3)]
        [TestCase("Multi-Academy Trust", 8, 3)]
        public void Organisation_approval_model_maps_organisation_type_for_educational_institute_main_supporting_provider(string organisationType, int organisationTypeId, int providerTypeId)
        {
            var applicationId = Guid.NewGuid();

            var application = new RoatpApplicationResponse
            {
                ApplicationId = applicationId,
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails
                    {
                        ProviderRoute = providerTypeId
                    }
                }
            };
            _roatpApplicationApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);

            SetupPreamblePage();

            var organisationTypePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeMainSupporting,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.OrganisationTypeMainSupporting,
                                Value = "An educational institute"
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(It.IsAny<Guid>(), RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails, RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeMainSupporting))
                                                         .ReturnsAsync(organisationTypePage);

            var educationalInstituteTypePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.EducationalInstituteTypeMainSupporting,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.EducationalInstituteTypeMainSupporting,
                                Value = organisationType
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(It.IsAny<Guid>(), RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails, RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.EducationalInstituteTypeMainSupporting))
                                                         .ReturnsAsync(educationalInstituteTypePage);

            var model = _service.BuildApplicationApprovalViewModel(applicationId).GetAwaiter().GetResult();
            model.OrganisationTypeId.Should().Be(organisationTypeId);
        }

        [TestCase("School", 1)]
        [TestCase("General Further Education College", 2)]
        [TestCase("National College", 3)]
        [TestCase("Sixth Form College", 4)]
        [TestCase("Further Education Institute", 5)]
        [TestCase("Higher Education Institute", 6)]
        [TestCase("Academy", 7)]
        [TestCase("Multi-Academy Trust", 8)]
        public void Organisation_approval_model_maps_organisation_type_for_educational_institute_supporting_provider(string organisationType, int organisationTypeId)
        {
            var applicationId = Guid.NewGuid();

            var application = new RoatpApplicationResponse
            {
                ApplicationId = applicationId,
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails
                    {
                        ProviderRoute = 2
                    }
                }
            };
            _roatpApplicationApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);

            SetupPreamblePage();

            var organisationTypePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeEmployer,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.OrganisationTypeEmployer,
                                Value = "An educational institute"
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(It.IsAny<Guid>(), RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails, RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeEmployer))
                                                         .ReturnsAsync(organisationTypePage);

            var educationalInstituteTypePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.EducationalInstituteTypeEmployer,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.EducationalInstituteTypeEmployer,
                                Value = organisationType
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(It.IsAny<Guid>(), RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails, RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.EducationalInstituteTypeEmployer))
                                                         .ReturnsAsync(educationalInstituteTypePage);

            var model = _service.BuildApplicationApprovalViewModel(applicationId).GetAwaiter().GetResult();
            model.OrganisationTypeId.Should().Be(organisationTypeId);
        }

        [TestCase("NHS Trust", 9, 1)]
        [TestCase("Police", 10, 1)]
        [TestCase("Fire authority", 11, 1)]
        [TestCase("Local authority", 12, 1)]
        [TestCase("Government department", 13, 1)]
        [TestCase("Non departmental public body (NDPB)", 14, 1)]
        [TestCase("Executive agency", 15, 1)]
        [TestCase("NHS Trust", 9, 3)]
        [TestCase("Police", 10, 3)]
        [TestCase("Fire authority", 11, 3)]
        [TestCase("Local authority", 12, 3)]
        [TestCase("Government department", 13, 3)]
        [TestCase("Non departmental public body (NDPB)", 14, 3)]
        [TestCase("Executive agency", 15, 3)]
        public void Organisation_approval_model_maps_organisation_type_for_public_body_main_supporting_provider(string organisationType, int organisationTypeId, int providerTypeId)
        {
            var applicationId = Guid.NewGuid();

            var application = new RoatpApplicationResponse
            {
                ApplicationId = applicationId,
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails
                    {
                        ProviderRoute = providerTypeId
                    }
                }
            };
            _roatpApplicationApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);

            SetupPreamblePage();

            var organisationTypePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeMainSupporting,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.OrganisationTypeMainSupporting,
                                Value = "A public body"
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(It.IsAny<Guid>(), RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails, RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeMainSupporting))
                                                         .ReturnsAsync(organisationTypePage);

            var publicBodyTypePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.PublicBodyTypeMainSupporting,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.PublicBodyTypeMainSupporting,
                                Value = organisationType
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(It.IsAny<Guid>(), RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails, RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.PublicBodyTypeMainSupporting))
                                                         .ReturnsAsync(publicBodyTypePage);

            var model = _service.BuildApplicationApprovalViewModel(applicationId).GetAwaiter().GetResult();
            model.OrganisationTypeId.Should().Be(organisationTypeId);
        }

        [TestCase("NHS Trust", 9)]
        [TestCase("Police", 10)]
        [TestCase("Fire authority", 11)]
        [TestCase("Local authority", 12)]
        [TestCase("Government department", 13)]
        [TestCase("Non departmental public body (NDPB)", 14)]
        [TestCase("Executive agency", 15)]
        public void Organisation_approval_model_maps_organisation_type_for_public_body_employer_provider(string organisationType, int organisationTypeId)
        {
            var applicationId = Guid.NewGuid();

            var application = new RoatpApplicationResponse
            {
                ApplicationId = applicationId,
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails
                    {
                        ProviderRoute = 2
                    }
                }
            };
            _roatpApplicationApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);
            
            SetupPreamblePage();

            var organisationTypePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeEmployer,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.OrganisationTypeEmployer,
                                Value = "A public body"
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(It.IsAny<Guid>(), RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails, RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeEmployer))
                                                         .ReturnsAsync(organisationTypePage);

            var publicBodyTypePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.PublicBodyTypeEmployer,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.PublicBodyTypeEmployer,
                                Value = organisationType
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(It.IsAny<Guid>(), RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails, RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.PublicBodyTypeEmployer))
                                                         .ReturnsAsync(publicBodyTypePage);

            var model = _service.BuildApplicationApprovalViewModel(applicationId).GetAwaiter().GetResult();
            model.OrganisationTypeId.Should().Be(organisationTypeId);
        }

        [TestCase("An Independent Training Provider", 16, 1)]
        [TestCase("An Apprenticeship Training Agency", 17, 1)]
        [TestCase("A Group Training Association", 18, 1)]
        [TestCase("An employer training apprentices in other organisations", 19, 1)]
        [TestCase("An Independent Training Provider", 16, 3)]
        [TestCase("An Apprenticeship Training Agency", 17, 3)]
        [TestCase("A Group Training Association", 18, 3)]
        [TestCase("An employer training apprentices in other organisations", 19, 3)]
        public void Organisation_approval_model_maps_organisation_type_for_other_organisation_types(string organisationType, int organisationTypeId, int providerTypeId)
        {
            var applicationId = Guid.NewGuid();

            var application = new RoatpApplicationResponse
            {
                ApplicationId = applicationId,
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails
                    {
                        ProviderRoute = providerTypeId
                    }
                }
            };
            _roatpApplicationApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(application);

            SetupPreamblePage();

            var organisationTypePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeMainSupporting,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.YourOrganisation.QuestionIds.OrganisationTypeMainSupporting,
                                Value = organisationType
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(It.IsAny<Guid>(), RoatpQnaConstants.RoatpSequences.YourOrganisation,
                                                          RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails, RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.OrganisationTypeMainSupporting))
                                                         .ReturnsAsync(organisationTypePage);

            var model = _service.BuildApplicationApprovalViewModel(applicationId).GetAwaiter().GetResult();
            model.OrganisationTypeId.Should().Be(organisationTypeId);
        }

        private void SetupPreamblePage()
        {
            var preamblePage = new Page
            {
                PageId = RoatpQnaConstants.RoatpSections.Preamble.PageId,
                PageOfAnswers = new List<PageOfAnswers>
                {
                    new PageOfAnswers
                    {
                        Id = Guid.NewGuid(),
                        Answers = new List<Answer>
                        {
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.LegalName,
                                Value = "Legal name"
                            },
                            new Answer
                            {
                                QuestionId = RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.UKPRN,
                                Value = "10001234"
                            }
                        }
                    }
                }
            };

            _qnaApiClient.Setup(x => x.GetPageBySectionNo(It.IsAny<Guid>(), RoatpQnaConstants.RoatpSequences.Preamble,
                                                          RoatpQnaConstants.RoatpSections.Preamble.SectionId, RoatpQnaConstants.RoatpSections.Preamble.PageId))
                                                         .ReturnsAsync(preamblePage);
        }
    }
}
