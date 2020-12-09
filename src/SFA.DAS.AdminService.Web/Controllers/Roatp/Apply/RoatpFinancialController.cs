using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.FeatureToggles;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Common.Validation;
using SFA.DAS.AdminService.Web.Validators.Roatp.Applications;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Financial;
using FinancialApplicationSelectedGrade = SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply.FinancialApplicationSelectedGrade;
using FinancialReviewStatus = SFA.DAS.AssessorService.ApplyTypes.Roatp.FinancialReviewStatus;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpFinancialAssessorTeam)]
    [FeatureToggle(FeatureToggles.EnableRoatpFinancialReview, "Dashboard", "Index")]
    public class RoatpFinancialController : Controller
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRoatpFinancialClarificationViewModelValidator _clarificationValidator;
        public RoatpFinancialController(IRoatpOrganisationApiClient apiClient, IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IHttpContextAccessor contextAccessor, IRoatpFinancialClarificationViewModelValidator clarificationValidator)
        {
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
            _clarificationValidator = clarificationValidator;
            _qnaApiClient = qnaApiClient;
        }

        [HttpGet("/Roatp/Financial/Current")]
        public async Task<IActionResult> OpenApplications(int page = 1)
        {
            var statusCounts = await _applyApiClient.GetFinancialApplicationsStatusCounts();

            var applications = await _applyApiClient.GetOpenFinancialApplications();

            var paginatedApplications = new PaginatedList<RoatpFinancialSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpFinancialDashboardViewModel { Applications = paginatedApplications, StatusCounts = statusCounts };

            return View("~/Views/Roatp/Apply/Financial/OpenApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Financial/Clarification")]
        public async Task<IActionResult> ClarificationApplications(int page = 1)
        {
            var statusCounts = await _applyApiClient.GetFinancialApplicationsStatusCounts();

            var applications = await _applyApiClient.GetClarificationFinancialApplications();

            var paginatedApplications = new PaginatedList<RoatpFinancialSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpFinancialDashboardViewModel { Applications = paginatedApplications, StatusCounts = statusCounts };

            return View("~/Views/Roatp/Apply/Financial/ClarificationApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Financial/Outcome")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            var statusCounts = await _applyApiClient.GetFinancialApplicationsStatusCounts();

            var applications = await _applyApiClient.GetClosedFinancialApplications();

            var paginatedApplications = new PaginatedList<RoatpFinancialSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpFinancialDashboardViewModel { Applications = paginatedApplications, StatusCounts = statusCounts };

            return View("~/Views/Roatp/Apply/Financial/ClosedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Financial/{applicationId}")]
        public async Task<IActionResult> ViewApplication(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            if (application is null)
            {
                return RedirectToAction(nameof(OpenApplications));
            }

            var vm = await CreateRoatpFinancialApplicationViewModel(application);

            var contact = await _applyApiClient.GetContactForApplication(applicationId);
            vm.ApplicantEmailAddress = contact.Email;

            var activeFinancialReviewStatuses = new List<string> { FinancialReviewStatus.New, FinancialReviewStatus.InProgress };

            if (activeFinancialReviewStatuses.Contains(application.FinancialReviewStatus))
            {
                await _applyApiClient.StartFinancialReview(application.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName());
                return View("~/Views/Roatp/Apply/Financial/Application.cshtml", vm);
            }
            
            if (application.FinancialReviewStatus == FinancialReviewStatus.ClarificationSent)
            {
                var clarificationVm = ConvertFinancialApplicationToFinancialClarificationViewModel(vm, vm.ClarificationComments);
                return View("~/Views/Roatp/Apply/Financial/Application_Clarification.cshtml", clarificationVm);
            }

            return View("~/Views/Roatp/Apply/Financial/Application_ReadOnly.cshtml", vm);
        }

        [HttpPost("/Roatp/Financial/{applicationId}")]
        public async Task<IActionResult> GradeApplication(Guid applicationId, [FromForm] RoatpFinancialApplicationViewModel vm)
        {
            var application = await _applyApiClient.GetApplication(vm.ApplicationId);
            if (application is null)
            {
                return RedirectToAction(nameof(OpenApplications));
            }

            if (ModelState.IsValid)
            {
                var financialReviewDetails = new FinancialReviewDetails
                {
                    GradedBy = _contextAccessor.HttpContext.User.UserDisplayName(),
                    GradedDateTime = DateTime.UtcNow,
                    SelectedGrade = vm.FinancialReviewDetails.SelectedGrade,
                    FinancialDueDate = GetFinancialDueDate(vm),
                    Comments = GetFinancialReviewComments(vm)
                };

                await _applyApiClient.ReturnFinancialReview(vm.ApplicationId, financialReviewDetails);
                return RedirectToAction(nameof(Graded), new { vm.ApplicationId });
            }
            else
            {
                var newvm = await CreateRoatpFinancialApplicationViewModel(application);
                newvm.ApplicantEmailAddress = vm.ApplicantEmailAddress;
                newvm.ClarificationComments = vm.ClarificationComments;
                
                // For now, only replace selected grade with whatever was selected
                if (vm.FinancialReviewDetails != null)
                {
                    newvm.FinancialReviewDetails.SelectedGrade = vm.FinancialReviewDetails.SelectedGrade;
                }

                return View("~/Views/Roatp/Apply/Financial/Application.cshtml", newvm);
            }
        }

        [HttpPost("/Roatp/Financial/Clarification/{applicationId}")]
        public async Task<IActionResult> SubmitClarification(Guid applicationId, RoatpFinancialClarificationViewModel vm)
        {
            var removeClarificationFileName = string.Empty;
            var application = await _applyApiClient.GetApplication(vm.ApplicationId);
            if (application is null)
            {
                return RedirectToAction(nameof(OpenApplications));
            }
            var isClarificationFilesUpdate = HttpContext.Request.Form["submitClarificationFiles"].Count != 0;
            var isClarificationOutcome = HttpContext.Request.Form["submitClarificationOutcome"].Count == 1;
            if (!isClarificationFilesUpdate && !isClarificationOutcome &&
                HttpContext.Request.Form["removeClarificationFile"].Count == 1)
                removeClarificationFileName = HttpContext.Request.Form["removeClarificationFile"].ToString();

            vm.FinancialReviewDetails.ClarificationFiles = application.FinancialGrade.ClarificationFiles;
            vm.FilesToUpload = HttpContext.Request.Form.Files;
            
            var validationResponse = _clarificationValidator.Validate(vm, isClarificationFilesUpdate, isClarificationOutcome);

            if (validationResponse.Errors !=null && validationResponse.Errors.Count>0)
            {
                var newClarificationViewModel = await ProcessErrorMessagesAndRebuildModel(vm, application, validationResponse);
                return View("~/Views/Roatp/Apply/Financial/Application_Clarification.cshtml", newClarificationViewModel);
            }

            if (isClarificationFilesUpdate)
            {
                var newClarificationVm = await ProcessUploadedFilesAndRebuildViewModel(applicationId, vm, application);
                return View("~/Views/Roatp/Apply/Financial/Application_Clarification.cshtml", newClarificationVm);
            }


            if (!string.IsNullOrEmpty(removeClarificationFileName))
            {
                var newClarificationVm = await RemoveUploadedFileAndRebuildViewModel(applicationId, vm, removeClarificationFileName, application);
                return View("~/Views/Roatp/Apply/Financial/Application_Clarification.cshtml", newClarificationVm);
            }

            var financialReviewDetails = RebuildFinancialReviewDetailsForSubmission(vm);

            await _applyApiClient.ReturnFinancialReview(vm.ApplicationId, financialReviewDetails);
            return RedirectToAction(nameof(Graded), new { vm.ApplicationId });
        }

        private FinancialReviewDetails RebuildFinancialReviewDetailsForSubmission(RoatpFinancialClarificationViewModel vm)
        {
            var comments = vm.Comments;
            if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate)
                comments = vm.InadequateComments;

            var financialReviewDetails = new FinancialReviewDetails
            {
                GradedBy = _contextAccessor.HttpContext.User.UserDisplayName(),
                GradedDateTime = DateTime.UtcNow,
                SelectedGrade = vm.FinancialReviewDetails.SelectedGrade,
                FinancialDueDate = GetFinancialDueDate(vm),
                Comments = comments,
                ClarificationResponse = vm.ClarificationResponse,
                ClarificationRequestedOn = vm.FinancialReviewDetails.ClarificationRequestedOn,
                ClarificationFiles = vm.FinancialReviewDetails.ClarificationFiles
            };
            return financialReviewDetails;
        }

        [HttpGet("/Roatp/Financial/Clarification/{applicationId}/Download/{filename}")]
        public async Task<IActionResult> DownloadClarificationFile(Guid applicationId, string filename)
        {
            var response = await _applyApiClient.DownloadClarificationFile(applicationId, filename);
            var fileStream = await response.Content.ReadAsStreamAsync();
            return File(fileStream, response.Content.Headers.ContentType.MediaType, filename);
        }

        [HttpGet("/Roatp/Financial/Download/Application/{applicationId}/Section/{sectionId}")]
        public async Task<IActionResult> DownloadFiles(Guid applicationId, Guid sectionId)
        {
            var financialSection = await _qnaApiClient.GetSection(applicationId, sectionId);
            
            var application = await _applyApiClient.GetApplication(applicationId);

            if (financialSection != null)
            {
                using (var zipStream = new MemoryStream())
                {
                    using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                    {
                        var pagesContainingQuestionsWithFileupload = financialSection.QnAData.Pages.Where(x => x.Questions.Any(y => y.Input.Type == "FileUpload")).ToList();
                        foreach (var uploadPage in pagesContainingQuestionsWithFileupload)
                        {
                            foreach (var uploadQuestion in uploadPage.Questions)
                            {
                                foreach (var answer in financialSection.QnAData.Pages.SelectMany(p => p.PageOfAnswers).SelectMany(a => a.Answers).Where(a => a.QuestionId == uploadQuestion.QuestionId))
                                {
                                    if (string.IsNullOrWhiteSpace(answer.ToString())) continue;

                                    var fileDownloadName = answer.Value;

                                    var downloadedFile = await _qnaApiClient.DownloadFile(applicationId, financialSection.Id, uploadPage.PageId, uploadQuestion.QuestionId, fileDownloadName);

                                    if (downloadedFile.IsSuccessStatusCode)
                                    {
                                        var zipEntry = zipArchive.CreateEntry(fileDownloadName);
                                        using (var entryStream = zipEntry.Open())
                                        {
                                            var fileStream = await downloadedFile.Content.ReadAsStreamAsync();
                                            fileStream.CopyTo(entryStream);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    zipStream.Position = 0;

                    return File(zipStream.ToArray(), "application/zip", $"FinancialDocuments_{application.ApplyData.ApplyDetails.OrganisationName}.zip");
                }
            }

            return new NotFoundResult();
        }

        [HttpGet("/Roatp/Financial/{applicationId}/Graded")]
        public async Task<IActionResult> Graded(Guid applicationId)
        {
            var application = await _applyApiClient.GetApplication(applicationId);
            if (application?.FinancialGrade is null)
            {
                return RedirectToAction(nameof(OpenApplications));
            }

            return View("~/Views/Roatp/Apply/Financial/Graded.cshtml", application.FinancialGrade);
        }

        private async Task<RoatpFinancialClarificationViewModel> RemoveUploadedFileAndRebuildViewModel(Guid applicationId, RoatpFinancialClarificationViewModel vm,
         string removeClarificationFileName, RoatpApplicationResponse application)
        {
            var fileRemoved = await _applyApiClient.RemoveClarificationFile(applicationId,
                _contextAccessor.HttpContext.User.UserId(), removeClarificationFileName);


            var financialReviewDets = vm.FinancialReviewDetails;
            if (fileRemoved)
            {
                var clarificationFiles = financialReviewDets.ClarificationFiles;
                var newClarificationFiles = new List<ClarificationFile>();
                if (clarificationFiles != null)
                {
                    foreach (var file in clarificationFiles)
                    {
                        if (file.Filename != removeClarificationFileName)
                            newClarificationFiles.Add(file);
                    }

                    financialReviewDets.ClarificationFiles = newClarificationFiles;
                }
            }

            var applicationVm = await RebuildApplicationViewModel(vm, application, financialReviewDets);

            var newClarificationVm =
                ConvertFinancialApplicationToFinancialClarificationViewModel(applicationVm, vm.InternalComments);
            return newClarificationVm;
        }

        private async Task<RoatpFinancialClarificationViewModel> ProcessErrorMessagesAndRebuildModel(RoatpFinancialClarificationViewModel vm,
            RoatpApplicationResponse application, ValidationResponse validationResponse)
        {
            var applicationViewModel = await RebuildApplicationViewModel(vm, application, vm.FinancialReviewDetails);
            var newClarificationViewModel =
                ConvertFinancialApplicationToFinancialClarificationViewModel(applicationViewModel, vm.InternalComments);
            newClarificationViewModel.ErrorMessages = validationResponse.Errors;
            return newClarificationViewModel;
        }

        private async Task<RoatpFinancialClarificationViewModel> ProcessUploadedFilesAndRebuildViewModel(Guid applicationId, RoatpFinancialClarificationViewModel vm,
            RoatpApplicationResponse application)
        {
            var financialReviewDets = vm.FinancialReviewDetails;

            if (vm.FilesToUpload != null && vm.FilesToUpload.Count > 0)
            {
                var fileToUpload = vm.FilesToUpload[0].FileName;
                if (!FileAlreadyInClarifications(financialReviewDets.ClarificationFiles, fileToUpload))
                {
                    var fileUploadedSuccessfully = await _applyApiClient.UploadClarificationFile(applicationId,
                        _contextAccessor.HttpContext.User.UserId(), vm.FilesToUpload);


                    if (fileUploadedSuccessfully)
                    {
                        if (financialReviewDets.ClarificationFiles == null)
                            financialReviewDets.ClarificationFiles = new List<ClarificationFile>();

                        financialReviewDets.ClarificationFiles.Add(new ClarificationFile
                        { Filename = fileToUpload });
                    }
                }
            }

            var clarificationVm = await RebuildApplicationViewModel(vm, application, financialReviewDets);

            var newClarificationVm =
                ConvertFinancialApplicationToFinancialClarificationViewModel(clarificationVm, vm.InternalComments);
            return newClarificationVm;
        }

        private static bool FileAlreadyInClarifications(List<ClarificationFile> clarificationFiles, string fileToUpload)
        {
            return clarificationFiles != null && clarificationFiles.Count > 0 &&
                   clarificationFiles.Any(file => file.Filename == fileToUpload);
        }

        private async Task<RoatpFinancialApplicationViewModel> RebuildApplicationViewModel(RoatpFinancialClarificationViewModel vm,
            RoatpApplicationResponse application, FinancialReviewDetails financialReviewDets)
        {
            var clarificationVm = await CreateRoatpFinancialApplicationViewModel(application);
            clarificationVm.ApplicantEmailAddress = vm.ApplicantEmailAddress;
            clarificationVm.ClarificationComments = vm.ClarificationComments;
            clarificationVm.FinancialReviewDetails = financialReviewDets;
            clarificationVm.OutstandingFinancialDueDate = vm.OutstandingFinancialDueDate;
            clarificationVm.GoodFinancialDueDate = vm.GoodFinancialDueDate;
            clarificationVm.SatisfactoryFinancialDueDate = vm.SatisfactoryFinancialDueDate;
            clarificationVm.InadequateComments = vm.InadequateComments;
            return clarificationVm;
        }
        private async Task<RoatpFinancialApplicationViewModel> CreateRoatpFinancialApplicationViewModel(RoatpApplicationResponse applicationFromAssessor)
        {
            if (applicationFromAssessor is null)
            {
                return new RoatpFinancialApplicationViewModel();
            }

            var parentCompanySection = await GetParentCompanySection(applicationFromAssessor.ApplicationId);
            var activelyTradingSection = await GetActivelyTradingSection(applicationFromAssessor.ApplicationId);
            var organisationTypeSection = await GetOrganisationTypeSection(applicationFromAssessor.ApplicationId);
            var financialSections = await GetFinancialSections(applicationFromAssessor);

            return new RoatpFinancialApplicationViewModel(applicationFromAssessor, parentCompanySection, activelyTradingSection, organisationTypeSection, financialSections);
        }

        private async Task<Section> GetParentCompanySection(Guid applicationId)
        {
            const string ParentCompanySectionTitle = "UK ultimate parent company";

            Section parentCompanySection = null;

            var hasParentCompanyTagValue = await _qnaApiClient.GetQuestionTag(applicationId, RoatpQnaConstants.QnaQuestionTags.HasParentCompany);

            if ("Yes".Equals(hasParentCompanyTagValue, StringComparison.OrdinalIgnoreCase))
            {                
                parentCompanySection = await _qnaApiClient.GetSectionBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.YourOrganisation, RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails);
                parentCompanySection.LinkTitle = ParentCompanySectionTitle;
                parentCompanySection.Title = ParentCompanySectionTitle;
                parentCompanySection.QnAData.Pages = parentCompanySection.QnAData.Pages?.Where(page => page.PageId == RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.ParentCompanyCheck 
                                                                                                    || page.PageId == RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.ParentCompanyDetails).ToList();
            }

            return parentCompanySection;
        }

        private async Task<Section> GetActivelyTradingSection(Guid applicationId)
        {
            const string ActivelyTradingSectionTitle = "Actively trading";

            Section activelyTradingSection = await _qnaApiClient.GetSectionBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.YourOrganisation, RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails);
            activelyTradingSection.LinkTitle = ActivelyTradingSectionTitle;
            activelyTradingSection.Title = ActivelyTradingSectionTitle;
            activelyTradingSection.QnAData.Pages = activelyTradingSection.QnAData.Pages?.Where(page => page.PageId == RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.TradingForMain
                                                                                                    || page.PageId == RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.TradingForEmployer
                                                                                                    || page.PageId == RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.TradingForSupporting).ToList();

            return activelyTradingSection;
        }

        private async Task<Section> GetOrganisationTypeSection(Guid applicationId)
        {
            const string OrganisationTypeSectionTitle = "Organisation type";

            Section organisationTypeSection = await _qnaApiClient.GetSectionBySectionNo(applicationId, RoatpQnaConstants.RoatpSequences.YourOrganisation, RoatpQnaConstants.RoatpSections.YourOrganisation.DescribeYourOrganisation);
            organisationTypeSection.LinkTitle = OrganisationTypeSectionTitle;
            organisationTypeSection.Title = OrganisationTypeSectionTitle;
            organisationTypeSection.QnAData.Pages = organisationTypeSection.QnAData.Pages?.Where(page => page.PageId != RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.HowDescribeYourOrganisation).ToList();

            return organisationTypeSection;
        }

        private async Task<List<Section>> GetFinancialSections(RoatpApplicationResponse applicationFromAssessor)
        {
            const string CompanyFhaSectionTitle = "Organisation's financial health";
            const string ParentCompanyFhaSectionTitle = "UK ultimate parent company's financial health";

            var financialSections = new List<Section>();

            var applicationId = applicationFromAssessor.ApplicationId;
            var applicationSequences = applicationFromAssessor.ApplyData?.Sequences;

            var roatpSequences = await _applyApiClient.GetRoatpSequences();
            var financialSequences = roatpSequences.Where(x => x.Roles.Contains(Roles.ProviderRiskAssuranceTeam));
           
            foreach (var sequence in financialSequences)
            {
                var qnaSequence = await _qnaApiClient.GetSequenceBySequenceNo(applicationId, sequence.Id);
                var qnaSections = await _qnaApiClient.GetSections(applicationId, qnaSequence.Id);

                foreach (var section in qnaSections)
                {
                    var roatpSequence = roatpSequences.FirstOrDefault(x => x.Id == sequence.Id);

                    if (!roatpSequence.ExcludeSections.Contains(section.SectionNo.ToString()) && IsRequiredSection(applicationSequences, section))
                    {
                        // Ensure at least one active page is answered
                        if (section.QnAData.Pages.Any(p => p.Active && p.Complete))
                        {
                            // APR-1477 - adjust title for Assessor
                            if(section.SequenceNo == RoatpQnaConstants.RoatpSequences.FinancialEvidence && section.SectionNo == RoatpQnaConstants.RoatpSections.FinancialEvidence.YourOrganisationsFinancialEvidence)
                            {
                                section.LinkTitle = CompanyFhaSectionTitle;
                                section.Title = CompanyFhaSectionTitle;
                            }
                            else if (section.SequenceNo == RoatpQnaConstants.RoatpSequences.FinancialEvidence && section.SectionNo == RoatpQnaConstants.RoatpSections.FinancialEvidence.YourUkUltimateParentCompanysFinancialEvidence)
                            {
                                section.LinkTitle = ParentCompanyFhaSectionTitle;
                                section.Title = ParentCompanyFhaSectionTitle;
                            }

                            // APR-1606 - Display in PageId order
                            section.QnAData.Pages = section.QnAData.Pages.OrderBy(p => p.PageId).ToList();

                            financialSections.Add(section);
                        }
                    }
                }
            }

            return financialSections;
        }

        private static RoatpFinancialClarificationViewModel ConvertFinancialApplicationToFinancialClarificationViewModel(RoatpFinancialApplicationViewModel vm, string internalComments)
        {
            var viewModel = new RoatpFinancialClarificationViewModel
            {
                ClarificationComments = vm.ClarificationComments,
                InadequateComments =  vm.InadequateComments,
                ApplicantEmailAddress = vm.ApplicantEmailAddress,
                FinancialReviewDetails = vm.FinancialReviewDetails,
                ApplicationId = vm.ApplicationId,
                ApplicationReference = vm.ApplicationReference,
                ApplicationRoute = vm.ApplicationRoute,
                DeclaredInApplication = vm.DeclaredInApplication,
                Sections = vm.Sections,
                OutstandingFinancialDueDate = vm.OutstandingFinancialDueDate,
                GoodFinancialDueDate = vm.GoodFinancialDueDate,
                SatisfactoryFinancialDueDate = vm.SatisfactoryFinancialDueDate,
                InternalComments = internalComments,
                Ukprn = vm.Ukprn,
                OrganisationName = vm.OrganisationName,
                SubmittedDate = vm.SubmittedDate
            };

            return viewModel;
        }
        private static bool IsRequiredSection(List<RoatpApplySequence> applicationSequences, Section section)
        {
            var appSequence = applicationSequences?.SingleOrDefault(appSeq => appSeq.SequenceNo == section.SequenceNo);

            var notRequired = appSequence is null
                                || appSequence.NotRequired
                                || appSequence.Sections is null
                                || appSequence.Sections.Any(appSec => appSec.SectionNo == section.SectionNo && appSec.NotRequired);

            return !notRequired;
        }

        private static DateTime? GetFinancialDueDate(RoatpFinancialApplicationViewModel vm)
        {
            if(vm is null)
            {
                return null;
            }

            switch (vm?.FinancialReviewDetails?.SelectedGrade)
            {
                case FinancialApplicationSelectedGrade.Outstanding:
                    return vm.OutstandingFinancialDueDate?.ToDateTime();
                case FinancialApplicationSelectedGrade.Good:
                    return vm.GoodFinancialDueDate?.ToDateTime();
                case FinancialApplicationSelectedGrade.Satisfactory:
                    return vm.SatisfactoryFinancialDueDate?.ToDateTime();
                default:
                    return null;
            }
        }

        private static string GetFinancialReviewComments(RoatpFinancialApplicationViewModel vm)
        {
            if (vm is null)
            {
                return null;
            }

            switch (vm?.FinancialReviewDetails?.SelectedGrade)
            {
                case FinancialApplicationSelectedGrade.Clarification:
                    return vm.ClarificationComments;
                case FinancialApplicationSelectedGrade.Inadequate:
                    return vm.InadequateComments;
                default:
                    return null;
            }
        }
    }
}