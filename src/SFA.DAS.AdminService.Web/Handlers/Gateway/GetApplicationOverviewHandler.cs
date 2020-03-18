using MediatR;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Domain;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetApplicationOverviewHandler : IRequestHandler<GetApplicationOverviewRequest, RoatpGatewayApplicationViewModel>
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;

        private readonly ILogger<GetApplicationOverviewHandler> _logger;

        public GetApplicationOverviewHandler(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, ILogger<GetApplicationOverviewHandler> logger)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _logger = logger;
        }
        public async Task<RoatpGatewayApplicationViewModel> Handle(GetApplicationOverviewRequest request, CancellationToken cancellationToken)
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
                        new GatewaySection { SectionNumber = 1, PageId = "1-10", LinkTitle = "Legal name", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "1-20", LinkTitle = "Trading name", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 3, PageId = "1-30", LinkTitle = "Organisation status", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 4, PageId = "1-40", LinkTitle = "Address", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 5, PageId = "1-50", LinkTitle = "ICO registration number", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 6, PageId = "1-60", LinkTitle = "Website address", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 7, PageId = "1-70", LinkTitle = "Organisation high risk", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 2,
                    SequenceTitle = "People in control checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "2-10", LinkTitle = "People in control", HiddenText = "for people in control checks", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "2-20", LinkTitle = "People in control high risk", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 3,
                    SequenceTitle = "Register checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "3-10", LinkTitle = "RoATP", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "3-20", LinkTitle = "Register of end-point assessment organisations", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 4,
                    SequenceTitle = "Experience and accreditation checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "4-10", LinkTitle = "Office for Student (OfS)", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "4-20", LinkTitle = "Initial teacher training (ITT)", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 3, PageId = "4-30", LinkTitle = "Ofsted", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 4, PageId = "4-40", LinkTitle = "Subcontractor declaration", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 5,
                    SequenceTitle = "Organisation’s criminal and compliance checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "5-10", LinkTitle = "Composition with creditors", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "5-20", LinkTitle = "Failed to pay back funds", HiddenText = "for the organisation", Status = "" },
                        new GatewaySection { SectionNumber = 3, PageId = "5-30", LinkTitle = "Contract terminated early by a public body", HiddenText = "for the organisation", Status = "" },
                        new GatewaySection { SectionNumber = 4, PageId = "5-40", LinkTitle = "Withdrawn from a contract with a public body", HiddenText = "for the organisation", Status = "" },
                        new GatewaySection { SectionNumber = 5, PageId = "5-50", LinkTitle = "Register of Training Organisations (RoTO)", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 6, PageId = "5-60", LinkTitle = "Funding removed from any education bodies", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 7, PageId = "5-70", LinkTitle = "Removed from any professional or trade registers", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 8, PageId = "5-80", LinkTitle = "Initial Teacher Training accreditation", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 9, PageId = "5-90", LinkTitle = "Removed from any charity register", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 10, PageId = "5-100", LinkTitle = "Investigated due to safeguarding issues", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 11, PageId = "5-110", LinkTitle = "Investigated due to whistleblowing issues", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 12, PageId = "5-120", LinkTitle = "Insolvency or winding up proceedings", HiddenText = "", Status = "" }
                    }
                },

                new GatewaySequence
                {
                    SequenceNumber = 6,
                    SequenceTitle = "People in control’s criminal and compliance checks",
                    Sections = new List<GatewaySection>
                    {
                        new GatewaySection { SectionNumber = 1, PageId = "6-10", LinkTitle = "Unspent criminal convictions", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 2, PageId = "6-20", LinkTitle = "Failed to pay back funds", HiddenText = "for the people in control", Status = "" },
                        new GatewaySection { SectionNumber = 3, PageId = "6-30", LinkTitle = "Investigated for fraud or irregularities", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 4, PageId = "6-40", LinkTitle = "Ongoing investigations for fraud or irregularities", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 5, PageId = "6-50", LinkTitle = "Contract terminated early by a public body", HiddenText = "for the people in control", Status = "" },
                        new GatewaySection { SectionNumber = 6, PageId = "6-60", LinkTitle = "Withdrawn from a contract with a public body", HiddenText = "for the people in control", Status = "" },
                        new GatewaySection { SectionNumber = 7, PageId = "6-70", LinkTitle = "Breached tax payments or social security contributions", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 8, PageId = "6-80", LinkTitle = "Register of Removed Trustees", HiddenText = "", Status = "" },
                        new GatewaySection { SectionNumber = 9, PageId = "6-90", LinkTitle = "Been made bankrupt", HiddenText = "", Status = "" }
                    }
                }
            };
            #endregion


            // TODO:
            if (application.GatewayReviewStatus.Equals(GatewayReviewStatus.New))
            {
                // NotRequired checks
                var TradingNameAndWebsitePage = await _qnaApiClient.GetPageBySectionNo(request.ApplicationId, 0, 1, RoatpQnaConstants.RoatpSections.Preamble.SectionId.ToString());
                // TradingName
                var TradingName = TradingNameAndWebsitePage.PageOfAnswers.SelectMany(a => a.Answers).Where(a => a.QuestionId == RoatpQnaConstants.RoatpSections.Preamble.QuestionIds.TradingName).FirstOrDefault().Value;
                var TradingNameStatus = string.IsNullOrWhiteSpace(TradingName) ? SectionReviewStatus.NotRequired : string.Empty;
                if (TradingNameStatus.Equals(SectionReviewStatus.NotRequired))
                {
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).Where(sec => sec.PageId == GatewayPageIds.TradingName).FirstOrDefault().Status = SectionReviewStatus.NotRequired;
                    _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{request.ApplicationId}' - PageId '{GatewayPageIds.TradingName}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{request.UserName}' - PageData = 'null'");
                    await _applyApiClient.SubmitGatewayPageAnswer(request.ApplicationId, GatewayPageIds.TradingName, SectionReviewStatus.NotRequired, request.UserName, null);
                }

                // WebsiteAddress
                var WebsiteAddressUkrlp = TradingNameAndWebsitePage.PageOfAnswers.SelectMany(a => a.Answers).Where(a => a.QuestionId == "PRE-30").FirstOrDefault().Value;

                var WebsiteAddressApplyPage = await _qnaApiClient.GetPageBySectionNo(request.ApplicationId, 1, 2, "40");
                var WebsiteAddressApply = WebsiteAddressApplyPage.PageOfAnswers.SelectMany(a => a.Answers).Where(a => a.QuestionId == "YO-41").FirstOrDefault()?.Value;

                var WebsiteAddressStatus = string.IsNullOrWhiteSpace(WebsiteAddressUkrlp) && string.IsNullOrWhiteSpace(WebsiteAddressApply) ? SectionReviewStatus.NotRequired : string.Empty; ;
                if (WebsiteAddressStatus.Equals(SectionReviewStatus.NotRequired))
                {
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).Where(sec => sec.PageId == GatewayPageIds.WebsiteAddress).FirstOrDefault().Status = SectionReviewStatus.NotRequired;
                    _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{request.ApplicationId}' - PageId '{GatewayPageIds.WebsiteAddress}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{request.UserName}' - PageData = 'null'");
                    await _applyApiClient.SubmitGatewayPageAnswer(request.ApplicationId, GatewayPageIds.WebsiteAddress, SectionReviewStatus.NotRequired, request.UserName, null);
                }

                var ProviderRoute = application.ApplyData.ApplyDetails.ProviderRoute;

                // OfficeForStudent & InitialTeacherTraining
                var OfficeForStudentStatus = SectionReviewStatus.NotRequired;
                var InitialTeacherTrainingStatus = SectionReviewStatus.NotRequired;

                if (ProviderRoute.Equals(1) || ProviderRoute.Equals(2))
                {
                    var OfficeForStudentPage = await _qnaApiClient.GetPageBySectionNo(request.ApplicationId, 1, 5, "235");
                    var OfficeForStudent = OfficeForStudentPage.PageOfAnswers.SelectMany(a => a.Answers).Where(a => a.QuestionId == "YO-235").FirstOrDefault()?.Value;
                    if (OfficeForStudent!=null && OfficeForStudent.Equals("Yes", StringComparison.InvariantCultureIgnoreCase)) OfficeForStudentStatus = string.Empty;

                    var InitialTeacherTrainingPage = await _qnaApiClient.GetPageBySectionNo(request.ApplicationId, 1, 5, "240");
                    var InitialTeacherTraining = InitialTeacherTrainingPage.PageOfAnswers.SelectMany(a => a.Answers).Where(a => a.QuestionId == "YO-240").FirstOrDefault()?.Value;
                    if (InitialTeacherTraining!=null && InitialTeacherTraining.Equals("Yes", StringComparison.InvariantCultureIgnoreCase)) InitialTeacherTrainingStatus = string.Empty;
                }
                if (OfficeForStudentStatus.Equals(SectionReviewStatus.NotRequired))
                {
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).Where(sec => sec.PageId == GatewayPageIds.OfficeForStudent).FirstOrDefault().Status = SectionReviewStatus.NotRequired;
                    _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{request.ApplicationId}' - PageId '{GatewayPageIds.OfficeForStudent}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{request.UserName}' - PageData = 'null'");
                    await _applyApiClient.SubmitGatewayPageAnswer(request.ApplicationId, GatewayPageIds.OfficeForStudent, SectionReviewStatus.NotRequired, request.UserName, null);
                }
                if (InitialTeacherTrainingStatus.Equals(SectionReviewStatus.NotRequired))
                {
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).Where(sec => sec.PageId == GatewayPageIds.InitialTeacherTraining).FirstOrDefault().Status = SectionReviewStatus.NotRequired;
                    _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{request.ApplicationId}' - PageId '{GatewayPageIds.InitialTeacherTraining}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{request.UserName}' - PageData = 'null'");
                    await _applyApiClient.SubmitGatewayPageAnswer(request.ApplicationId, GatewayPageIds.InitialTeacherTraining, SectionReviewStatus.NotRequired, request.UserName, null);
                }

                // Ofsted
                var OfstedStatus = ProviderRoute.Equals(3) ? SectionReviewStatus.NotRequired : string.Empty;
                if (OfstedStatus.Equals(SectionReviewStatus.NotRequired))
                {
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).Where(sec => sec.PageId == GatewayPageIds.Ofsted).FirstOrDefault().Status = SectionReviewStatus.NotRequired;
                    _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{request.ApplicationId}' - PageId '{GatewayPageIds.Ofsted}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{request.UserName}' - PageData = 'null'");
                    await _applyApiClient.SubmitGatewayPageAnswer(request.ApplicationId, GatewayPageIds.Ofsted, SectionReviewStatus.NotRequired, request.UserName, null);
                }

                // SubcontractorDeclaration
                var SubcontractorDeclarationStatus = ProviderRoute.Equals(3) ? string.Empty : SectionReviewStatus.NotRequired;
                if (SubcontractorDeclarationStatus.Equals(SectionReviewStatus.NotRequired))
                {
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).Where(sec => sec.PageId == GatewayPageIds.SubcontractorDeclaration).FirstOrDefault().Status = SectionReviewStatus.NotRequired;
                    _logger.LogInformation($"GetApplicationOverviewHandler-SubmitGatewayPageAnswer - ApplicationId '{request.ApplicationId}' - PageId '{GatewayPageIds.SubcontractorDeclaration}' - Status '{SectionReviewStatus.NotRequired}' - UserName '{request.UserName}' - PageData = 'null'");
                    await _applyApiClient.SubmitGatewayPageAnswer(request.ApplicationId, GatewayPageIds.SubcontractorDeclaration, SectionReviewStatus.NotRequired, request.UserName, null);
                }       
            }
            else
            {
                var savedStatuses = await _applyApiClient.GetGatewayPageAnswers(request.ApplicationId);
                foreach(var currentStatus in savedStatuses)
                {
                    // Inject the statuses into viewmodel
                    viewmodel.Sequences.SelectMany(seq => seq.Sections).Where(sec => sec.PageId == currentStatus.PageId).FirstOrDefault().Status = currentStatus.Status;
                }
            }

            // Check whether the Gateway Application is Ready to Confirm gateway outcome
            viewmodel.ReadyToConfirm = CheckIsItReadyToConfirm(viewmodel); 

            return viewmodel;
        }
    
        public bool CheckIsItReadyToConfirm(RoatpGatewayApplicationViewModel viewmodel)
        {
            bool IsReadyToConfirm = true;

            foreach(var sequence in viewmodel.Sequences)
            {
                foreach(var section in sequence.Sections)
                {
                    if(section.Status == null || (!section.Status.Equals(SectionReviewStatus.Pass) && !section.Status.Equals(SectionReviewStatus.Fail) && !section.Status.Equals(SectionReviewStatus.NotRequired)))
                    {
                        IsReadyToConfirm = false;
                        break;
                    } 
                }
            }

            return IsReadyToConfirm;
        }
    }
}
