using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers.Apply
{
    [Authorize(Roles = Roles.ProviderRiskAssuranceTeam + "," + Roles.CertificationTeam)]
    public class FinancialController : Controller
    {
        private readonly IApiClient _apiClient;
        private readonly IApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public FinancialController(IApiClient apiClient, IApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IHttpContextAccessor contextAccessor)
        {
            _apiClient = apiClient;
            _applyApiClient = applyApiClient;
            _contextAccessor = contextAccessor;
            _qnaApiClient = qnaApiClient;
        }

        [HttpGet("/Financial/Open")]
        public async Task<IActionResult> OpenApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetOpenFinancialApplications();

            var paginatedApplications = new PaginatedList<FinancialApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new FinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Financial/OpenApplications.cshtml", viewmodel);
        }

        [HttpGet("/Financial/Rejected")]
        public async Task<IActionResult> RejectedApplications(int page = 1)
        {
            // NOTE: Rejected actually means Feedback Added or it was graded as Inadequate
            var applications = await _applyApiClient.GetFeedbackAddedFinancialApplications();

            var paginatedApplications = new PaginatedList<FinancialApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new FinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Financial/RejectedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Financial/Closed")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            var applications = await _applyApiClient.GetClosedFinancialApplications();

            var paginatedApplications = new PaginatedList<FinancialApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new FinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Financial/ClosedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Financial/Download/Organisation/{OrgId}/Application/{ApplicationId}")]
        public async Task<IActionResult> DownloadFiles(Guid orgId, Guid applicationId)
        {
            // NOTE: Using the QnA applicationId is somewhat dubious! We're using the Assessor applicationId nearly everywhere else.
            var financialSection = await _qnaApiClient.GetSectionBySectionNo(applicationId, ApplyConst.FINANCIAL_SEQUENCE_NO, ApplyConst.FINANCIAL_DETAILS_SECTION_NO);
            var organisation = await _apiClient.GetOrganisation(orgId);

            if (financialSection != null && organisation != null)
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

                    return File(zipStream.ToArray(), "application/zip", $"FinancialDocuments_{organisation.EndPointAssessorName}.zip");
                }
            }

            return new NotFoundResult();
        }


        [HttpGet("/Financial/{Id}/Evaluated")]
        public async Task<IActionResult> Evaluated(Guid Id)
        {
            var application = await _applyApiClient.GetApplication(Id);
            if (application?.FinancialGrade is null)
            {
                return RedirectToAction(nameof(OpenApplications));
            }

            return View("~/Views/Apply/Financial/Graded.cshtml", application.FinancialGrade);
        }


        private static List<FinancialEvidence> GetFinancialEvidence(QnA.Api.Types.Sequence financialSequence, QnA.Api.Types.Section financialSection)
        {
            var listOfEvidence = new List<FinancialEvidence>();

            if (financialSequence != null && financialSection != null)
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

            return listOfEvidence;
        }
    }
}