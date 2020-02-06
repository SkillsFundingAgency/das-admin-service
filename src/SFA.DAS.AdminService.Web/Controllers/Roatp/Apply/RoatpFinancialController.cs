using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ApplyData = SFA.DAS.AssessorService.ApplyTypes.Roatp.ApplyData;
using FinancialEvidence = SFA.DAS.AssessorService.ApplyTypes.Roatp.FinancialEvidence;
using FinancialGrade = SFA.DAS.AssessorService.ApplyTypes.FinancialGrade;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.ProviderRiskAssuranceTeam + "," + Roles.CertificationTeam)]
    public class RoatpFinancialController : Controller
    {
        private readonly IRoatpOrganisationApiClient _apiClient;
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private const int FinancialHealthSequenceNo = 2;

        public RoatpFinancialController(IRoatpOrganisationApiClient apiClient, IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IHttpContextAccessor contextAccessor)
        {
            _apiClient = apiClient;
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
            _qnaApiClient = qnaApiClient;
        }

        [HttpGet("/Roatp/Financial/Open")]
        public async Task<IActionResult> OpenApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetOpenFinancialApplications();

            var paginatedApplications = new PaginatedList<RoatpFinancialSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpFinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Financial/OpenApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Financial/Rejected")]
        public async Task<IActionResult> RejectedApplications(int page = 1)
        {
            // NOTE: Rejected actually means Feedback Added or it was graded as Inadequate
            var applications = await _applyApiClient.GetFeedbackAddedFinancialApplications();

            var paginatedApplications = new PaginatedList<RoatpFinancialSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpFinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Financial/RejectedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Financial/Closed")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetClosedFinancialApplications();

            var paginatedApplications = new PaginatedList<RoatpFinancialSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new RoatpFinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Roatp/Apply/Financial/ClosedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Roatp/Financial/{Id}")]
        public async Task<IActionResult> ViewApplication(Guid Id)
        {
            var application = await _applyApiClient.GetApplication(Id);
            if (application is null)
            {
                return RedirectToAction(nameof(OpenApplications));
            }

            await _applyApiClient.StartFinancialReview(application.ApplicationId, _contextAccessor.HttpContext.User.UserDisplayName());

            var vm = await CreateFinancialApplicationViewModel(application);

            return View("~/Views/Roatp/Apply/Financial/Application.cshtml", vm);
        }

        [HttpGet("/Roatp/Financial/{Id}/Graded")]
        public async Task<IActionResult> ViewGradedApplication(Guid Id)
        {
            var application = await _applyApiClient.GetApplication(Id);
            if (application is null)
            {
                return RedirectToAction(nameof(OpenApplications));
            }

            var vm = await CreateFinancialApplicationViewModel(application);

            return View("~/Views/Roatp/Apply/Financial/Application_ReadOnly.cshtml", vm);
        }

        [HttpGet("/Roatp/Financial/Download/Application/{ApplicationId}/Section/{sectionId}")]
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

        [HttpPost("/Roatp/Financial")]
        public async Task<IActionResult> Grade(FinancialApplicationViewModel vm)
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
                    SelectedGrade = vm.Grade.SelectedGrade,
                    FinancialDueDate = GetFinancialDueDate(vm),
                    FinancialEvidences = await GetFinancialEvidence(vm.ApplicationId),
                    Comments = vm.Grade.InadequateMoreInformation
                };

                await _applyApiClient.ReturnFinancialReview(vm.ApplicationId, financialReviewDetails);
                return RedirectToAction(nameof(Evaluated), new { vm.Id });
            }
            else
            {
                var newvm = await CreateFinancialApplicationViewModel(application);
                return View("~/Views/Roatp/Apply/Financial/Application.cshtml", newvm);
            }
        }

        [HttpGet("/Roatp/Financial/{Id}/Evaluated")]
        public async Task<IActionResult> Evaluated(Guid Id)
        {
            var application = await _applyApiClient.GetApplication(Id);
            if (application?.financialGrade is null)
            {
                return RedirectToAction(nameof(OpenApplications));
            }

            return View("~/Views/Roatp/Apply/Financial/Graded.cshtml", application.financialGrade);
        }

        private async Task<RoatpFinancialApplicationViewModel> CreateFinancialApplicationViewModel(RoatpApplicationResponse applicationFromAssessor)
        {
            if (applicationFromAssessor is null)
            {
                return new RoatpFinancialApplicationViewModel();
            }

            var roatpSequences = await _applyApiClient.GetRoatpSequences();
            var financialSequences = roatpSequences.Where(x => x.Roles.Contains(Roles.ProviderRiskAssuranceTeam));
            var financialSections = new List<Section>();
            foreach(var sequence in financialSequences)
            {
                var qnaSequence = await _qnaApiClient.GetSequenceBySequenceNo(applicationFromAssessor.ApplicationId, sequence.Id);
                var sections = await _qnaApiClient.GetSections(applicationFromAssessor.ApplicationId, qnaSequence.Id);
                foreach(var section in sections)
                {
                    var roatpSequence = roatpSequences.FirstOrDefault(x => x.Id == sequence.Id);

                    if (!roatpSequence.ExcludeSections.Contains(section.SectionNo.ToString()))
                    {
                        financialSections.Add(section);
                    }
                }
            }

            var orgId = applicationFromAssessor.OrganisationId;
            var organisation = await _apiClient.GetOrganisation(orgId);

            var application = new AssessorService.ApplyTypes.Roatp.Apply
            {
                ApplicationStatus = applicationFromAssessor.ApplicationStatus,
                ApplyData = Mapper.Map<ApplyData>(applicationFromAssessor.ApplyData)
            };

            return new RoatpFinancialApplicationViewModel(applicationFromAssessor, financialSections);
        }

        private async Task<List<FinancialEvidence>> GetFinancialEvidence(Guid applicationId)
        {
            var listOfEvidence = new List<FinancialEvidence>();

            var financialSequence = await _qnaApiClient.GetSequenceBySequenceNo(applicationId, FinancialHealthSequenceNo);
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

        private static DateTime? GetFinancialDueDate(FinancialApplicationViewModel vm)
        {
            if(vm is null)
            {
                return null;
            }

            switch (vm?.Grade?.SelectedGrade)
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
    }
}