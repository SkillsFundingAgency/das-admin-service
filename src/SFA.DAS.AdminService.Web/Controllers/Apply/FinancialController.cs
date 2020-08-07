using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using FinancialGrade = SFA.DAS.AssessorService.ApplyTypes.FinancialGrade;

namespace SFA.DAS.AdminService.Web.Controllers.Apply
{
    [Authorize(Roles = Roles.ProviderRiskAssuranceTeam + "," + Roles.CertificationTeam)]
    public class FinancialController : Controller
    {
        private const int FINANCIAL_SEQUENCE_NO = 1;
        private const int FINANCIAL_SECTION_NO = 3;

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

        [HttpGet("/Financial/{Id}")]
        public async Task<IActionResult> ViewApplication(Guid Id)
        {
            var application = await _applyApiClient.GetApplication(Id);
            if (application is null)
            {
                return RedirectToAction(nameof(OpenApplications));
            }

            await _applyApiClient.StartFinancialReview(application.Id, _contextAccessor.HttpContext.User.UserDisplayName());

            var vm = await CreateFinancialApplicationViewModel(application, null);

            return View("~/Views/Apply/Financial/Application.cshtml", vm);
        }

        [HttpGet("/Financial/{Id}/Graded")]
        public async Task<IActionResult> ViewGradedApplication(Guid Id)
        {
            var application = await _applyApiClient.GetApplication(Id);
            if (application is null)
            {
                return RedirectToAction(nameof(OpenApplications));
            }

            var vm = await CreateFinancialApplicationViewModel(application, null);

            return View("~/Views/Apply/Financial/Application_ReadOnly.cshtml", vm);
        }

        [HttpGet("/Financial/Download/Organisation/{OrgId}/Application/{ApplicationId}")]
        public async Task<IActionResult> DownloadFiles(Guid orgId, Guid applicationId)
        {
            // NOTE: Using the QnA applicationId is somewhat dubious! We're using the Assessor applicationId nearly everywhere else.
            var financialSection = await _qnaApiClient.GetSectionBySectionNo(applicationId, FINANCIAL_SEQUENCE_NO, FINANCIAL_SECTION_NO);
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

        [HttpPost("/Financial")]
        public async Task<IActionResult> Grade(FinancialApplicationViewModel vm)
        {
            var application = await _applyApiClient.GetApplication(vm.Id);
            if (application is null)
            {
                return RedirectToAction(nameof(OpenApplications));
            }

            if (ModelState.IsValid)
            {
                var financialSequence = await _qnaApiClient.GetSequenceBySequenceNo(application.ApplicationId, FINANCIAL_SEQUENCE_NO);
                var financialSection = await _qnaApiClient.GetSectionBySectionNo(application.ApplicationId, FINANCIAL_SEQUENCE_NO, FINANCIAL_SECTION_NO);

                var grade = new FinancialGrade
                {
                    ApplicationReference = application.ApplyData.Apply.ReferenceNumber,
                    GradedBy = _contextAccessor.HttpContext.User.UserDisplayName(),
                    GradedDateTime = DateTime.UtcNow,
                    SelectedGrade = vm.Grade.SelectedGrade,
                    FinancialDueDate = GetFinancialDueDate(vm),
                    FinancialEvidences = GetFinancialEvidence(financialSequence, financialSection),
                    InadequateMoreInformation = vm.Grade.InadequateMoreInformation
                };

                await _applyApiClient.ReturnFinancialReview(vm.Id, grade);
                return RedirectToAction(nameof(Evaluated), new { vm.Id });
            }
            else
            {
                var newvm = await CreateFinancialApplicationViewModel(application, vm.Grade);
                return View("~/Views/Apply/Financial/Application.cshtml", newvm);
            }
        }

        [HttpGet("/Financial/{Id}/Evaluated")]
        public async Task<IActionResult> Evaluated(Guid Id)
        {
            var application = await _applyApiClient.GetApplication(Id);
            if (application?.financialGrade is null)
            {
                return RedirectToAction(nameof(OpenApplications));
            }

            return View("~/Views/Apply/Financial/Graded.cshtml", application.financialGrade);
        }

        private async Task<FinancialApplicationViewModel> CreateFinancialApplicationViewModel(ApplicationResponse applicationFromAssessor, FinancialGrade grade)
        {
            if (applicationFromAssessor is null)
            {
                return new FinancialApplicationViewModel();
            }
            else if (grade is null)
            {
                grade = applicationFromAssessor.financialGrade;
            }

            var financialSection = await _qnaApiClient.GetSectionBySectionNo(applicationFromAssessor.ApplicationId, FINANCIAL_SEQUENCE_NO, FINANCIAL_SECTION_NO);

            var orgId = applicationFromAssessor.OrganisationId;
            var organisation = await _apiClient.GetOrganisation(orgId);

            var application = new AssessorService.ApplyTypes.Application
            {
                ApplicationData = new ApplicationData
                {
                    ReferenceNumber = applicationFromAssessor.ApplyData.Apply.ReferenceNumber
                },
                ApplyingOrganisation = organisation,
                ApplyingOrganisationId = orgId,
                ApplicationStatus = applicationFromAssessor.ApplicationStatus
            };

            return new FinancialApplicationViewModel(applicationFromAssessor.Id, applicationFromAssessor.ApplicationId, financialSection, grade, application);
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
                case FinancialApplicationSelectedGrade.Monitoring:
                    return vm.MonitoringFinancialDueDate?.ToDateTime();
                default:
                    return null;
            }
        }
    }
}