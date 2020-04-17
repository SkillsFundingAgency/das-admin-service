using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewayOverviewOrchestrator: IGatewayOverviewOrchestrator
    {
        //MFCMFC PARKING THIS TEST COVERAGE AS NEW STORY WILL BE CHANGING THE ORCHESTRATOR FLOW TO CHECK IF DETAILS ALREADY SET
        // WE WILL DO CHANGES AND COVERAGE WITHIN THAT STORY
        private readonly IRoatpApplicationApiClient _applyApiClient;

        private readonly ILogger<GatewayOverviewOrchestrator> _logger;
        private readonly IGatewaySectionsNotRequiredService _sectionsNotRequiredService;

        public GatewayOverviewOrchestrator(IRoatpApplicationApiClient applyApiClient, ILogger<GatewayOverviewOrchestrator> logger,
                                           IGatewaySectionsNotRequiredService sectionsNotRequiredService)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
            _sectionsNotRequiredService = sectionsNotRequiredService;
        }

        public async Task<RoatpGatewayApplicationViewModel> GetOverviewViewModel(GetApplicationOverviewRequest request)
        {
            var application = await _applyApiClient.GetApplication(request.ApplicationId);
            if (application is null)
            {
                return null;
            }

            var applicationData = GetApplicationData(application);

            var viewmodel = new RoatpGatewayApplicationViewModel(applicationData);
            viewmodel.Sequences = GetCoreGatewayApplicationViewModel();

            var savedStatuses = await _applyApiClient.GetGatewayPageAnswers(request.ApplicationId);
            if (savedStatuses != null && !savedStatuses.Any())
            {
                var providerRoute = application.ApplyData.ApplyDetails.ProviderRoute;
                await _sectionsNotRequiredService.SetupNotRequiredLinks(request.ApplicationId, request.UserName, viewmodel, providerRoute);
            }
            else
            {
                foreach (var currentStatus in savedStatuses)
                {
                    // Inject the statuses into viewmodel
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).FirstOrDefault(sec => sec.PageId == currentStatus.PageId).Status = currentStatus?.Status;
                }
            }

            viewmodel.ReadyToConfirm = CheckIsItReadyToConfirm(viewmodel);

            return viewmodel;
        }

        public async Task<RoatpGatewayApplicationViewModel> GetConfirmOverviewViewModel(GetApplicationOverviewRequest request)
        {
            var application = await _applyApiClient.GetApplication(request.ApplicationId);
            if (application is null)
            {
                return null;
            }

            var applicationData = GetApplicationData(application);

            var viewmodel = new RoatpGatewayApplicationViewModel(applicationData);
            viewmodel.Sequences = GetCoreGatewayApplicationViewModel();

            var savedStatuses = await _applyApiClient.GetGatewayPageAnswers(request.ApplicationId);
            if (savedStatuses != null && !savedStatuses.Any())
            {
                viewmodel.ReadyToConfirm = false;
                return viewmodel;
            }
            else
            {
                foreach (var currentStatus in savedStatuses)
                {
                    // Inject the statuses and comments into viewmodel
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).FirstOrDefault(sec => sec.PageId == currentStatus.PageId).Status = currentStatus?.Status;
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).FirstOrDefault(sec => sec.PageId == currentStatus.PageId).Comment = currentStatus?.Comments;
                }
            }

            viewmodel.ReadyToConfirm = CheckIsItReadyToConfirm(viewmodel);

            return viewmodel;
        }

        public void ProcessViewModelOnError(RoatpGatewayApplicationViewModel viewModelOnError, RoatpGatewayApplicationViewModel viewModel, ValidationResponse validationResponse)
        {
            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                viewModelOnError.IsInvalid = true;
                viewModelOnError.ErrorMessages = validationResponse.Errors;
                viewModelOnError.GatewayReviewStatus = viewModel.GatewayReviewStatus;
                viewModelOnError.OptionAskClarificationText = viewModel.OptionAskClarificationText;
                viewModelOnError.OptionDeclinedText = viewModel.OptionDeclinedText;
                viewModelOnError.OptionApprovedText = viewModel.OptionApprovedText;

                viewModelOnError.CssFormGroupError = HtmlAndCssElements.CssFormGroupErrorClass;
                viewModelOnError.RadioCheckedAskClarification = viewModelOnError.GatewayReviewStatus == GatewayReviewStatus.AskForClarification ? HtmlAndCssElements.CheckBoxChecked : string.Empty;
                viewModelOnError.RadioCheckedDeclined = viewModelOnError.GatewayReviewStatus == GatewayReviewStatus.Decline ? HtmlAndCssElements.CheckBoxChecked : string.Empty;
                viewModelOnError.RadioCheckedApproved = viewModelOnError.GatewayReviewStatus == GatewayReviewStatus.Pass ? HtmlAndCssElements.CheckBoxChecked : string.Empty;

                foreach(var error in viewModelOnError.ErrorMessages)
                {
                    if (error.Field.Equals(nameof(viewModelOnError.GatewayReviewStatus)))
                    {
                        viewModelOnError.ErrorTextGatewayReviewStatus = error.ErrorMessage;
                    }

                    if (error.Field.Equals(nameof(viewModelOnError.OptionAskClarificationText)))
                    {
                        viewModelOnError.ErrorTextAskClarification = error.ErrorMessage;
                        viewModelOnError.CssOnErrorAskClarification = HtmlAndCssElements.CssTextareaErrorOverrideClass;
                    }

                    if (error.Field.Equals(nameof(viewModelOnError.OptionDeclinedText)))
                    {
                        viewModelOnError.ErrorTextDeclined = error.ErrorMessage;
                        viewModelOnError.CssOnErrorDeclined = HtmlAndCssElements.CssTextareaErrorOverrideClass;
                    }

                    if (error.Field.Equals(nameof(viewModelOnError.OptionApprovedText)))
                    {
                        viewModelOnError.ErrorTextApproved = error.ErrorMessage;
                        viewModelOnError.CssOnErrorApproved = HtmlAndCssElements.CssTextareaErrorOverrideClass;
                    }
                }
            }
        }

        private bool CheckIsItReadyToConfirm(RoatpGatewayApplicationViewModel viewmodel)
        {
            var isReadyToConfirm = true;

            foreach (var sequence in viewmodel.Sequences)
            {
                foreach (var section in sequence.Sections)
                {
                    if (section.Status == null || (!section.Status.Equals(SectionReviewStatus.Pass) && !section.Status.Equals(SectionReviewStatus.Fail) && !section.Status.Equals(SectionReviewStatus.NotRequired)))
                    {
                        isReadyToConfirm = false;
                        break;
                    }
                }
            }

            return isReadyToConfirm;
        }

        private AssessorService.ApplyTypes.Roatp.Apply.Apply GetApplicationData(RoatpApplicationResponse application)
        {
            return new AssessorService.ApplyTypes.Roatp.Apply.Apply
            {
                ApplyData = new RoatpApplyData
                {
                    ApplyDetails = new RoatpApplyDetails
                    {
                        ReferenceNumber = application.ApplyData.ApplyDetails.ReferenceNumber,
                        ProviderRoute = application.ApplyData.ApplyDetails.ProviderRoute,
                        ProviderRouteName = application.ApplyData.ApplyDetails.ProviderRouteName,
                        UKPRN = application.ApplyData.ApplyDetails.UKPRN,
                        OrganisationName = application.ApplyData.ApplyDetails.OrganisationName,
                        ApplicationSubmittedOn = application.ApplyData.ApplyDetails.ApplicationSubmittedOn
                    }
                },
                Id = application.Id,
                ApplicationId = application.ApplicationId,
                OrganisationId = application.OrganisationId,
                ApplicationStatus = application.ApplicationStatus,
                GatewayReviewStatus = application.GatewayReviewStatus,
                AssessorReviewStatus = application.AssessorReviewStatus,
                FinancialReviewStatus = application.FinancialReviewStatus
            };
        }

        // APR-1467 Code Stubbed Data - TODO: Store it somewhere 
        private List<GatewaySequence> GetCoreGatewayApplicationViewModel()
        {
            return new List<GatewaySequence>
            {
                new GatewaySequence
                {
                    SequenceNumber = 1,
                    SequenceTitle = "Organisation checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = GatewayPageIds.LegalName,  LinkTitle = "Legal name", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = GatewayPageIds.TradingName, LinkTitle = "Trading name", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 3, PageId = GatewayPageIds.OrganisationStatus, LinkTitle = "Organisation status", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 4, PageId = GatewayPageIds.Address, LinkTitle = "Address", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 5, PageId = GatewayPageIds.IcoNumber, LinkTitle = "ICO registration number", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 6, PageId = GatewayPageIds.WebsiteAddress,  LinkTitle = "Website address", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 7, PageId = GatewayPageIds.OrganisationRisk,  LinkTitle = "Organisation high risk", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 2,
                    SequenceTitle = "People in control checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = GatewayPageIds.PeopleInControl, LinkTitle = "People in control", HiddenText = "for people in control checks", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = GatewayPageIds.PeopleInControlRisk,   LinkTitle = "People in control high risk", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 3,
                    SequenceTitle = "Register checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = GatewayPageIds.Roatp, LinkTitle = "RoATP", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = GatewayPageIds.Repao,  LinkTitle = "Register of end-point assessment organisations", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 4,
                    SequenceTitle = "Experience and accreditation checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = GatewayPageIds.OfficeForStudents,  LinkTitle = "Office for Student (OfS)", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = GatewayPageIds.InitialTeacherTraining, LinkTitle = "Initial teacher training (ITT)", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 3, PageId = GatewayPageIds.Ofsted,  LinkTitle = "Ofsted", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 4, PageId = GatewayPageIds.SubcontractorDeclaration, LinkTitle = "Subcontractor declaration", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 5,
                    SequenceTitle = "Organisation’s criminal and compliance checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.CompositionCreditors,  LinkTitle = "Composition with creditors", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.FailedToRepayFunds, LinkTitle = "Failed to pay back funds", HiddenText = "for the organisation", Status = "" },
                        new GatewaySection { SectionNumber = 3, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.ContractTermination,  LinkTitle = "Contract terminated early by a public body", HiddenText = "for the organisation", Status = "" },
                        new GatewaySection { SectionNumber = 4, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.ContractWithdrawnEarly, LinkTitle = "Withdrawn from a contract with a public body", HiddenText = "for the organisation", Status = "" },
                        new GatewaySection { SectionNumber = 5, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRoTO, LinkTitle = "Register of Training Organisations (RoTO)", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 6, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.FundingRemoved, LinkTitle = "Funding removed from any education bodies", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 7, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedRegister, LinkTitle = "Removed from any professional or trade registers", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 8, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.IttAccreditation,  LinkTitle = "Initial Teacher Training accreditation", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 9, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.RemovedCharityRegister, LinkTitle = "Removed from any charity register", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 10, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.Safeguarding,  LinkTitle = "Investigated due to safeguarding issues", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 11, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.Whistleblowing,  LinkTitle = "Investigated due to whistleblowing issues", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 12, PageId = GatewayPageIds.CriminalComplianceOrganisationChecks.Insolvency, LinkTitle = "Insolvency or winding up proceedings", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 6,
                    SequenceTitle = "People in control’s criminal and compliance checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.UnspentCriminalConvictions,  LinkTitle = "Unspent criminal convictions", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.FailedToRepayFunds, LinkTitle = "Failed to pay back funds", HiddenText = "for the people in control", Status = "" },
                        new GatewaySection { SectionNumber = 3, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.FraudIrregularities, LinkTitle = "Investigated for fraud or irregularities", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 4, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.OngoingInvestigation,  LinkTitle = "Ongoing investigations for fraud or irregularities", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 5, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.ContractTerminated, LinkTitle = "Contract terminated early by a public body", HiddenText = "for the people in control", Status = "" },
                        new GatewaySection { SectionNumber = 6, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.WithdrawnFromContract,  LinkTitle = "Withdrawn from a contract with a public body", HiddenText = "for the people in control", Status = "" },
                        new GatewaySection { SectionNumber = 7, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.BreachedPayments,  LinkTitle = "Breached tax payments or social security contributions", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 8, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.RegisterOfRemovedTrustees, LinkTitle = "Register of Removed Trustees", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 9, PageId = GatewayPageIds.CriminalComplianceWhosInControlChecks.Bankrupt,  LinkTitle = "Been made bankrupt", HiddenText = "", Status = "" }
                    }
                }
            };
        }
    }
}

