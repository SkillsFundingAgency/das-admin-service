using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpFinancialAssessorTeam)]
    [FeatureToggle(FeatureToggles.EnableRoatpFinancialReview, "Dashboard", "Index")]
    public class RoatpFinancialController : Controller
    {
        private readonly IRoatpOrganisationApiClient _apiClient;
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public RoatpFinancialController(IRoatpOrganisationApiClient apiClient, IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IHttpContextAccessor contextAccessor)
        {
            _apiClient = apiClient;
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
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

            var activeFinancialReviewStatuses = new List<string> { FinancialReviewStatus.New, FinancialReviewStatus.InProgress };

            if (activeFinancialReviewStatuses.Contains(application.FinancialReviewStatus))
            {
                await _applyApiClient.StartFinancialReview(application.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName());
                return View("~/Views/Roatp/Apply/Financial/Application.cshtml", vm);
            }
            else
            {
                return View("~/Views/Roatp/Apply/Financial/Application_ReadOnly.cshtml", vm);
            }
        }

        [HttpPost("/Roatp/Financial/{applicationId}")]
        public async Task<IActionResult> GradeApplication(Guid applicationId, RoatpFinancialApplicationViewModel vm)
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
                    FinancialEvidences = await GetFinancialEvidence(vm.ApplicationId),
                    Comments = GetFinancialReviewComments(vm)
                };

                await _applyApiClient.ReturnFinancialReview(vm.ApplicationId, financialReviewDetails);
                return RedirectToAction(nameof(Graded), new { vm.ApplicationId });
            }
            else
            {
                var newvm = await CreateRoatpFinancialApplicationViewModel(application);

                // For now, only replace selected grade with whatever was selected
                newvm.FinancialReviewDetails.SelectedGrade = vm.FinancialReviewDetails.SelectedGrade;

                return View("~/Views/Roatp/Apply/Financial/Application.cshtml", newvm);
            }
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

        private async Task<RoatpFinancialApplicationViewModel> CreateRoatpFinancialApplicationViewModel(RoatpApplicationResponse applicationFromAssessor)
        {
            if (applicationFromAssessor is null)
            {
                return new RoatpFinancialApplicationViewModel();
            }

            var parentCompanySection = await GetParentCompanySection(applicationFromAssessor.ApplicationId);
            var activelyTradingSection = await GetActivelyTradingSection(applicationFromAssessor.ApplicationId);
            var organisationTypeSection = await GetOrganisationTypeSection(applicationFromAssessor.ApplicationId);
            var financialSections = await GetFinancialSections(applicationFromAssessor.ApplicationId);

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
            organisationTypeSection.QnAData.Pages = organisationTypeSection.QnAData.Pages?.Where(page => page.PageId != RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.HowTrainItsApprentices
                                                                                                      && page.PageId != RoatpQnaConstants.RoatpSections.YourOrganisation.PageIds.HowDescribeYourOrganisation).ToList();

            return organisationTypeSection;
        }

        private async Task<List<Section>> GetFinancialSections(Guid applicationId)
        {
            const string CompanyFhaSectionTitle = "Organisation's financial health";
            const string ParentCompanyFhaSectionTitle = "UK ultimate parent company's financial health";

            var financialSections = new List<Section>();

            var roatpSequences = await _applyApiClient.GetRoatpSequences();
            var financialSequences = roatpSequences.Where(x => x.Roles.Contains(Roles.ProviderRiskAssuranceTeam));
           
            foreach (var sequence in financialSequences)
            {
                var qnaSequence = await _qnaApiClient.GetSequenceBySequenceNo(applicationId, sequence.Id);
                var sections = await _qnaApiClient.GetSections(applicationId, qnaSequence.Id);
                foreach (var section in sections)
                {
                    var roatpSequence = roatpSequences.FirstOrDefault(x => x.Id == sequence.Id);

                    if (!roatpSequence.ExcludeSections.Contains(section.SectionNo.ToString()))
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

                            financialSections.Add(section);
                        }
                    }
                }
            }

            return financialSections;
        }

        private async Task<List<FinancialEvidence>> GetFinancialEvidence(Guid applicationId)
        {
            var listOfEvidence = new List<FinancialEvidence>();

            var financialSequence = await _qnaApiClient.GetSequenceBySequenceNo(applicationId, RoatpQnaConstants.RoatpSequences.FinancialEvidence);
            var financialSections = await _qnaApiClient.GetSections(applicationId, financialSequence.Id);

            foreach(var financialSection in financialSections)
            {
                if (financialSection != null)
                {
                    var pagesContainingQuestionsWithFileupload = financialSection.QnAData.Pages.Where(x => x.Questions.Any(y => y.Input.Type == "FileUpload")).ToList();
                    foreach (var uploadPage in pagesContainingQuestionsWithFileupload)
                    {
                        foreach (var uploadQuestion in uploadPage.Questions)
                        {
                            foreach (var answer in financialSection.QnAData.Pages.SelectMany(p => p.PageOfAnswers).SelectMany(a => a.Answers).Where(a => a.QuestionId == uploadQuestion.QuestionId))
                            {
                                if (string.IsNullOrWhiteSpace(answer.ToString())) continue;
                                var filenameWithFullPath = $"{financialSequence.ApplicationId}/{financialSequence.Id}/{financialSection.Id}/{uploadPage.PageId}/{uploadQuestion.QuestionId}/{answer.Value}";
                                listOfEvidence.Add(new FinancialEvidence { Filename = filenameWithFullPath });
                            }
                        }
                    }
                }
            }

            return listOfEvidence;
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