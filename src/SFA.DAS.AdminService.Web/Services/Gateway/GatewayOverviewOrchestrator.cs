using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewayOverviewOrchestrator: IGatewayOverviewOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;

        private readonly ILogger<GatewayOverviewOrchestrator> _logger;

        public GatewayOverviewOrchestrator(IRoatpApplicationApiClient applyApiClient, ILogger<GatewayOverviewOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
        }

        public async Task<RoatpGatewayApplicationViewModel> GetOverviewViewModel(GetApplicationOverviewRequest request)
        {
            var application = await _applyApiClient.GetApplication(request.ApplicationId);
            if (application is null)
            {
                return null;
            }

            // Setting Application Data => TODO: To be stored in session.
            var applicationData = new AssessorService.ApplyTypes.Roatp.Apply
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

            var viewmodel = new RoatpGatewayApplicationViewModel(applicationData);

            // APR-1467 Code Stubbing Data - TODO: Store it somewhere 
            #region Sequences Stubbed Data
            viewmodel.Sequences = new List<GatewaySequence>
            {
                new GatewaySequence
                {
                    SequenceNumber = 1,
                    SequenceTitle = "Organisation checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "LegalName",  LinkTitle = "Legal name", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "TradingName", LinkTitle = "Trading name", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 3, PageId = "OrganisationStatus", LinkTitle = "Organisation status", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 4, PageId = "Address", LinkTitle = "Address", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 5, PageId = "IcoNumber", LinkTitle = "ICO registration number", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 6, PageId = "WebsiteAddress",  LinkTitle = "Website address", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 7, PageId = "OrganisationRisk",  LinkTitle = "Organisation high risk", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 2,
                    SequenceTitle = "People in control checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "PeopleInControl", LinkTitle = "People in control", HiddenText = "for people in control checks", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "PeopleInControlRisk",   LinkTitle = "People in control high risk", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 3,
                    SequenceTitle = "Register checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "Roatp", LinkTitle = "RoATP", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "Repao",  LinkTitle = "Register of end-point assessment organisations", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 4,
                    SequenceTitle = "Experience and accreditation checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "OfficeForStudents",  LinkTitle = "Office for Student (OfS)", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "InitialTeacherTraining", LinkTitle = "Initial teacher training (ITT)", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 3, PageId = "Ofsted",  LinkTitle = "Ofsted", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 4, PageId = "SubcontractorDeclaration", LinkTitle = "Subcontractor declaration", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 5,
                    SequenceTitle = "Organisation’s criminal and compliance checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "CompositionWithCreditors",  LinkTitle = "Composition with creditors", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "Payback", LinkTitle = "Failed to pay back funds", HiddenText = "for the organisation", Status = "" },
                        new GatewaySection { SectionNumber = 3, PageId = "ContractTerm",  LinkTitle = "Contract terminated early by a public body", HiddenText = "for the organisation", Status = "" },
                        new GatewaySection { SectionNumber = 4, PageId = "Withdrawn",LinkTitle = "Withdrawn from a contract with a public body", HiddenText = "for the organisation", Status = "" },
                        new GatewaySection { SectionNumber = 5, PageId = "Roto", LinkTitle = "Register of Training Organisations (RoTO)", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 6, PageId = "FundingRemoved", LinkTitle = "Funding removed from any education bodies", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 7, PageId = "RemovedProfessionalRegister", LinkTitle = "Removed from any professional or trade registers", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 8, PageId = "IttAccredication",  LinkTitle = "Initial Teacher Training accreditation", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 9, PageId = "RemovedCharityRegister", LinkTitle = "Removed from any charity register", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 10, PageId = "Safeguarding",  LinkTitle = "Investigated due to safeguarding issues", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 11, PageId = "Whistleblowing",  LinkTitle = "Investigated due to whistleblowing issues", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 12, PageId = "Insolvency", LinkTitle = "Insolvency or winding up proceedings", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 6,
                    SequenceTitle = "People in control’s criminal and compliance checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "UnspentCriminalConviction",  LinkTitle = "Unspent criminal convictions", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "FailedtoPayBack", LinkTitle = "Failed to pay back funds", HiddenText = "for the people in control", Status = "" },
                        new GatewaySection { SectionNumber = 3, PageId = "FraudIrregularities", LinkTitle = "Investigated for fraud or irregularities", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 4, PageId = "OngoingInvestigation",  LinkTitle = "Ongoing investigations for fraud or irregularities", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 5, PageId = "ContractTerminated", LinkTitle = "Contract terminated early by a public body", HiddenText = "for the people in control", Status = "" },
                        new GatewaySection { SectionNumber = 6, PageId = "WithdrawnFromContract",  LinkTitle = "Withdrawn from a contract with a public body", HiddenText = "for the people in control", Status = "" },
                        new GatewaySection { SectionNumber = 7, PageId = "BreachedPayments",  LinkTitle = "Breached tax payments or social security contributions", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 8, PageId = "RegisterOfRemovedTrustees", LinkTitle = "Register of Removed Trustees", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 9, PageId = "Bankrupt",  LinkTitle = "Been made bankrupt", HiddenText = "", Status = "" }
                    }
                }
            };
            #endregion

            if (application.GatewayReviewStatus.Equals(GatewayReviewStatus.New))
            { 
                // TradingName
                var tradingName = await _applyApiClient.GetTradingName(request.ApplicationId);

                if (string.IsNullOrEmpty(tradingName))
                {
                    var page = viewmodel?.Sequences?.SelectMany(seq => seq.Sections)
                        .Where(sec => sec.PageId == GatewayPageIds.TradingName)?.FirstOrDefault();
                      
                   if (page!=null)
                        page.Status = SectionReviewStatus.NotRequired;

                    _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{request.ApplicationId}' - PageId '{GatewayPageIds.TradingName}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{request.UserName}' - PageData = 'null'");
                    await _applyApiClient.SubmitGatewayPageAnswer(request.ApplicationId, GatewayPageIds.TradingName, SectionReviewStatus.NotRequired, request.UserName, null);
                }

                // WebsiteAddress
                var websiteAddressUkrlp = await _applyApiClient.GetWebsiteAddressSourcedFromUkrlp(request.ApplicationId);
                var websiteAddressApply =
                    await _applyApiClient.GetWebsiteAddressManuallyEntered(request.ApplicationId);
                var websiteAddressStatus = string.IsNullOrWhiteSpace(websiteAddressUkrlp) && string.IsNullOrWhiteSpace(websiteAddressApply) ? SectionReviewStatus.NotRequired : string.Empty; ;
                if (websiteAddressStatus.Equals(SectionReviewStatus.NotRequired))
                {
                    var page = viewmodel?.Sequences?.SelectMany(seq => seq.Sections)
                        .Where(sec => sec.PageId == GatewayPageIds.WebsiteAddress)?.FirstOrDefault();

                    if (page != null)
                        page.Status = SectionReviewStatus.NotRequired;
                    _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{request.ApplicationId}' - PageId '{GatewayPageIds.WebsiteAddress}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{request.UserName}' - PageData = 'null'");
                    await _applyApiClient.SubmitGatewayPageAnswer(request.ApplicationId, GatewayPageIds.WebsiteAddress, SectionReviewStatus.NotRequired, request.UserName, null);
                }

                var providerRoute = application.ApplyData.ApplyDetails.ProviderRoute;

                // OfficeForStudent 
                var officeForStudentStatus = SectionReviewStatus.NotRequired;

                if (providerRoute.Equals(ProviderTypes.Main) || providerRoute.Equals(ProviderTypes.Employer))
                {
                    var officeForStudent = await _applyApiClient.GetOfficeForStudents(request.ApplicationId);
                    if (officeForStudent != null && officeForStudent.Equals("Yes", StringComparison.InvariantCultureIgnoreCase)) officeForStudentStatus = string.Empty;
                }
                if (officeForStudentStatus.Equals(SectionReviewStatus.NotRequired))
                {

                    var page = viewmodel?.Sequences?.SelectMany(seq => seq.Sections)
                        .Where(sec => sec.PageId == GatewayPageIds.OfficeForStudent)?.FirstOrDefault();
                    if (page != null)
                        page.Status = SectionReviewStatus.NotRequired;
                    _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{request.ApplicationId}' - PageId '{GatewayPageIds.OfficeForStudent}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{request.UserName}' - PageData = 'null'");
                    await _applyApiClient.SubmitGatewayPageAnswer(request.ApplicationId, GatewayPageIds.OfficeForStudent, SectionReviewStatus.NotRequired, request.UserName, null);
                }
                

                // InitialTeacherTraining
                var initialTeacherTrainingStatus = SectionReviewStatus.NotRequired;

                if (providerRoute.Equals(ProviderTypes.Main) || providerRoute.Equals(ProviderTypes.Employer))
                {
                    var initialTeacherTraining = await _applyApiClient.GetInitialTeacherTraining(request.ApplicationId);
                    if (initialTeacherTraining != null && initialTeacherTraining.Equals("Yes", StringComparison.InvariantCultureIgnoreCase)) initialTeacherTrainingStatus = string.Empty;
                }

                if (initialTeacherTrainingStatus.Equals(SectionReviewStatus.NotRequired))
                {
                    var page = viewmodel?.Sequences?.SelectMany(seq => seq.Sections)
                        .Where(sec => sec.PageId == GatewayPageIds.InitialTeacherTraining)?.FirstOrDefault();
                    if (page != null)
                        page.Status = SectionReviewStatus.NotRequired;

                    _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{request.ApplicationId}' - PageId '{GatewayPageIds.InitialTeacherTraining}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{request.UserName}' - PageData = 'null'");
                    await _applyApiClient.SubmitGatewayPageAnswer(request.ApplicationId, GatewayPageIds.InitialTeacherTraining, SectionReviewStatus.NotRequired, request.UserName, null);
                }

                // Ofsted
                var OfstedStatus = providerRoute.Equals( ProviderTypes.Supporting) ? SectionReviewStatus.NotRequired : string.Empty;
                if (OfstedStatus.Equals(SectionReviewStatus.NotRequired))
                {
                    var page = viewmodel?.Sequences?.SelectMany(seq => seq.Sections)
                        .Where(sec => sec.PageId == GatewayPageIds.Ofsted)?.FirstOrDefault();
                    if (page != null)
                        page.Status = SectionReviewStatus.NotRequired;

                    _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{request.ApplicationId}' - PageId '{GatewayPageIds.Ofsted}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{request.UserName}' - PageData = 'null'");
                    await _applyApiClient.SubmitGatewayPageAnswer(request.ApplicationId, GatewayPageIds.Ofsted, SectionReviewStatus.NotRequired, request.UserName, null);
                }

                // SubcontractorDeclaration
                var SubcontractorDeclarationStatus = providerRoute.Equals(ProviderTypes.Supporting) ? string.Empty : SectionReviewStatus.NotRequired;
                if (SubcontractorDeclarationStatus.Equals(SectionReviewStatus.NotRequired))
                {
                    var page = viewmodel?.Sequences?.SelectMany(seq => seq.Sections)
                        .Where(sec => sec.PageId == GatewayPageIds.SubcontractorDeclaration)?.FirstOrDefault();
                    if (page != null)
                        page.Status = SectionReviewStatus.NotRequired;

                    _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{request.ApplicationId}' - PageId '{GatewayPageIds.SubcontractorDeclaration}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{request.UserName}' - PageData = 'null'");
                    await _applyApiClient.SubmitGatewayPageAnswer(request.ApplicationId, GatewayPageIds.SubcontractorDeclaration, SectionReviewStatus.NotRequired, request.UserName, null);
                }
            }
            else
            {
                var savedStatuses = await _applyApiClient.GetGatewayPageAnswers(request.ApplicationId);
                foreach (var currentStatus in savedStatuses)
                {
                    // Inject the statuses into viewmodel
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).FirstOrDefault(sec => sec.PageId == currentStatus.PageId).Status = currentStatus.Status;
                }
            }

            viewmodel.ReadyToConfirm = CheckIsItReadyToConfirm(viewmodel);

            return viewmodel;
        }

        public bool CheckIsItReadyToConfirm(RoatpGatewayApplicationViewModel viewmodel)
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
    }
}

