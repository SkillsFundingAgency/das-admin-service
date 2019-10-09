using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using AutoMapper;
using FinancialGrade = SFA.DAS.AssessorService.ApplyTypes.FinancialGrade;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Controllers.Apply
{
    [Authorize(Roles = Roles.ProviderRiskAssuranceTeam + "," + Roles.CertificationTeam)]
    public class FinancialController : Controller
    {
        private const int FINANCIAL_SEQUENCE_NO = 1;
        private const int FINANCIAL_SECTION_NO = 3;

        private readonly IApiClient _apiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public FinancialController(IApiClient apiClient, IQnaApiClient qnaApiClient, IHttpContextAccessor contextAccessor)
        {
            _apiClient = apiClient;
            _contextAccessor = contextAccessor;
            _qnaApiClient = qnaApiClient;
        }
        
        [HttpGet("/Financial/Open")]
        public async Task<IActionResult> OpenApplications(int page = 1)
        {
            var applications = await _apiClient.GetOpenFinancialApplications();

            var paginatedApplications = new PaginatedList<FinancialApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new FinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Financial/OpenApplications.cshtml", viewmodel);
        }

        [HttpGet("/Financial/Rejected")]
        public async Task<IActionResult> RejectedApplications(int page = 1)
        {
            // NOTE: Rejected actually means Feedback Added or it was graded as Inadequate
            var applications = await _apiClient.GetFeedbackAddedFinancialApplications();

            var paginatedApplications = new PaginatedList<FinancialApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new FinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Financial/RejectedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Financial/Closed")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            var applications = await _apiClient.GetClosedFinancialApplications();

            var paginatedApplications = new PaginatedList<FinancialApplicationSummaryItem>(applications, applications.Count, page, int.MaxValue);

            var viewmodel = new FinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Financial/ClosedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Financial/{Id}")]
        public async Task<IActionResult> ViewApplication(Guid id)
        {
            var givenName = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")?.Value;
            var surname = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")?.Value;

            await _apiClient.StartFinancialReview(id, $"{givenName} {surname}");

            var vm = await CreateFinancialApplicationViewModel(id, null);

            return View("~/Views/Apply/Financial/Application.cshtml", vm);
        }


        [HttpGet("/Financial/{Id}/Graded")]
        public async Task<IActionResult> ViewGradedApplication(Guid id)
        {
            var vm = await CreateFinancialApplicationViewModel(id, null);

            return View("~/Views/Apply/Financial/Application_ReadOnly.cshtml", vm);
        }

        [HttpGet("/Financial/Download/Organisation/{OrgId}/Application/{ApplicationId}")]
        public async Task<IActionResult> DownloadFiles(Guid orgId, Guid applicationId)
        {
            var sequences = await _qnaApiClient.GetAllApplicationSequences(applicationId);
            var financialSequence = sequences.Single(x => x.SequenceNo == FINANCIAL_SEQUENCE_NO);
            var sections = await _qnaApiClient.GetSections(applicationId, financialSequence.Id);
            var financialSection = await _qnaApiClient.GetSection(applicationId, sections.SingleOrDefault(x => x.SectionNo == FINANCIAL_SECTION_NO).Id);

            var org = await _apiClient.GetOrganisation(orgId);

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

                zipStream.Position = 0;

                var compressedBytes = zipStream.ToArray();
                
                return File(compressedBytes, "application/zip", $"FinancialDocuments_{org.EndPointAssessorName}.zip");
            }
        }

        [HttpPost("/Financial")]
        public async Task<IActionResult> Grade(FinancialApplicationViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var givenName = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")?.Value;
                var surname = _contextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")?.Value;

                var applicationFromAssessor = await _apiClient.GetApplicationFromAssessor(vm.Id.ToString());
                var sequences = await _qnaApiClient.GetAllApplicationSequences(applicationFromAssessor.ApplicationId);
                var financialSequence = await _qnaApiClient.GetSequence(applicationFromAssessor.ApplicationId, sequences.Single(x => x.SequenceNo == FINANCIAL_SEQUENCE_NO).Id);
                var sections = await _qnaApiClient.GetSections(applicationFromAssessor.ApplicationId, financialSequence.Id);
                var financialSection = await _qnaApiClient.GetSection(applicationFromAssessor.ApplicationId, sections.Single(x => x.SectionNo == FINANCIAL_SECTION_NO).Id);

                if (vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate && !string.IsNullOrEmpty(vm.Grade.InadequateMoreInformation))
                {
                    var pageId = financialSection.QnAData.Pages.First(p => p.Active).PageId;
                    var feedback = new QnA.Api.Types.Page.Feedback { Message = vm.Grade.InadequateMoreInformation, From = "Staff member", Date = DateTime.UtcNow, IsNew = true };
                    await _qnaApiClient.UpdateFeedback(applicationFromAssessor.ApplicationId, financialSection.Id, pageId, feedback);
                }

                var grade = new FinancialGrade
                {
                    ApplicationReference = applicationFromAssessor.ApplyData.Apply.ReferenceNumber,
                    GradedBy = $"{givenName} {surname}",
                    GradedDateTime = DateTime.UtcNow,
                    SelectedGrade = vm.Grade.SelectedGrade,
                    FinancialDueDate = GetFinancialDueDate(vm),
                    FinancialEvidences = GetFinancialEvidence(financialSequence, financialSection),
                    InadequateMoreInformation = vm.Grade.InadequateMoreInformation
                };

                await _apiClient.ReturnFinancialReview(vm.Id, grade);

                return RedirectToAction("Evaluated", new {vm.Id});   
            }
            else
            {
                var newvm = await CreateFinancialApplicationViewModel(vm.Id, vm.Grade);
                return View("~/Views/Apply/Financial/Application.cshtml", newvm);
            }
        }


        [HttpGet("/Financial/{Id}/Evaluated")]
        public async Task<IActionResult> Evaluated(Guid id)
        {
            var applicationFromAssessor = await _apiClient.GetApplicationFromAssessor(id.ToString());
            return View("~/Views/Apply/Financial/Graded.cshtml", applicationFromAssessor.financialGrade);
        }

        private async  Task<FinancialApplicationViewModel> CreateFinancialApplicationViewModel(Guid id, FinancialGrade grade)
        {
            var applicationFromAssessor = await _apiClient.GetApplicationFromAssessor(id.ToString());
            if (grade is null)
            {
                grade = applicationFromAssessor?.financialGrade;
            }

            var sequences = await _qnaApiClient.GetAllApplicationSequences(applicationFromAssessor.ApplicationId);
            var financialSequence = await _qnaApiClient.GetSequence(applicationFromAssessor.ApplicationId, sequences.Single(x => x.SequenceNo == FINANCIAL_SEQUENCE_NO).Id);
            var sections = await _qnaApiClient.GetSections(applicationFromAssessor.ApplicationId, financialSequence.Id);
            var financialSection = await _qnaApiClient.GetSection(applicationFromAssessor.ApplicationId, sections.Single(x => x.SectionNo == FINANCIAL_SECTION_NO).Id);

            var orgId = applicationFromAssessor?.OrganisationId ?? Guid.Empty;
            var organisation = await _apiClient.GetOrganisation(orgId);

            var application = new AssessorService.ApplyTypes.Application 
            { 
                ApplicationData = new ApplicationData
                {
                    ReferenceNumber = applicationFromAssessor?.ApplyData.Apply.ReferenceNumber
                },
                ApplyingOrganisation = organisation,
                ApplyingOrganisationId = orgId,
                ApplicationStatus = applicationFromAssessor.ApplicationStatus
            };

            return new FinancialApplicationViewModel(id, applicationFromAssessor.ApplicationId, financialSection, grade, application);
        }

        private List<FinancialEvidence> GetFinancialEvidence(QnA.Api.Types.Sequence financialSequence, QnA.Api.Types.Section financialSection)
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
            switch (vm.Grade.SelectedGrade)
            {
                case FinancialApplicationSelectedGrade.Outstanding:
                    return vm.OutstandingFinancialDueDate.ToDateTime();
                case FinancialApplicationSelectedGrade.Good:
                    return vm.GoodFinancialDueDate.ToDateTime();
                case FinancialApplicationSelectedGrade.Satisfactory:
                    return vm.SatisfactoryFinancialDueDate.ToDateTime();
                case null:
                default:
                    return null;
            }
        }

    }
}