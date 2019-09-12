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

            var paginatedApplications = new PaginatedList<FinancialApplicationSummaryItem>(applications, applications.Count(), page, int.MaxValue);

            var viewmodel = new FinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Financial/OpenApplications.cshtml", viewmodel);
        }

        [HttpGet("/Financial/Rejected")]
        public async Task<IActionResult> RejectedApplications(int page = 1)
        {
            // NOTE: Rejected actually means Feedback Added or it was graded as Inadequate
            var applications = await _apiClient.GetFeedbackAddedFinancialApplications();

            var paginatedApplications = new PaginatedList<FinancialApplicationSummaryItem>(applications, applications.Count(), page, int.MaxValue);

            var viewmodel = new FinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Financial/RejectedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Financial/Closed")]
        public async Task<IActionResult> ClosedApplications(int page = 1)
        {
            var applications = await _apiClient.GetClosedFinancialApplications();

            var paginatedApplications = new PaginatedList<FinancialApplicationSummaryItem>(applications, applications.Count(), page, int.MaxValue);

            var viewmodel = new FinancialDashboardViewModel { Applications = paginatedApplications };

            return View("~/Views/Apply/Financial/ClosedApplications.cshtml", viewmodel);
        }

        [HttpGet("/Financial/{Id}")]
        public async Task<IActionResult> ViewApplication(Guid id)
        {
            var applicationFromAssessor = await _apiClient.GetApplicationFromAssessor(id.ToString());

            var grade = applicationFromAssessor?.financialGrade;

            var vm = await createFinancialApplicationViewModel(id, grade);

            return View("~/Views/Apply/Financial/Application.cshtml", vm);
        }


        [HttpGet("/Financial/{Id}/Graded")]
        public async Task<IActionResult> ViewGradedApplication(Guid Id)
        {
            var applicationFromAssessor = await _apiClient.GetApplicationFromAssessor(Id.ToString());
            var sequence = await _qnaApiClient.GetApplicationActiveSequence(applicationFromAssessor.ApplicationId);
            var sections = await _qnaApiClient.GetSections(applicationFromAssessor.ApplicationId, sequence.Id);

            var application = await _apiClient.GetApplication(applicationFromAssessor?.ApplicationId ?? Guid.Empty); 
            var financialSection = await _qnaApiClient.GetSection(applicationFromAssessor.ApplicationId, sections.SingleOrDefault(x => x.SectionNo == 3 && x.SequenceNo == 1).Id);

            var grade = applicationFromAssessor.financialGrade;

            var vm = new FinancialApplicationViewModel(Id,applicationFromAssessor.Id, financialSection, grade, application);
            
            return View("~/Views/Apply/Financial/Application_ReadOnly.cshtml", vm);
        }

        [HttpGet("/Financial/Download/Organisation/{OrgId}/Application/{ApplicationId}")]
        public async Task<IActionResult> Download(Guid orgId, Guid applicationId)
        {
            var sequence = await _qnaApiClient.GetApplicationActiveSequence(applicationId);
            var sections = await _qnaApiClient.GetSections(applicationId, sequence.Id);

            var org = await _apiClient.GetOrganisation(orgId);
            var financialSection = await _qnaApiClient.GetSection(applicationId, sections.SingleOrDefault(x => x.SectionNo == 3 && x.SequenceNo == 1).Id);

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

                                var downloadedFile = await _qnaApiClient.DownloadFile(applicationId, sections.Single(x => x.SectionNo == 3).Id, uploadPage.PageId, uploadQuestion.QuestionId, fileDownloadName);

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
                var org = await _apiClient.GetOrganisation(vm.OrgId);

                var sequence = await _qnaApiClient.GetApplicationActiveSequence(vm.ApplicationId);
                var sections = await _qnaApiClient.GetSections(vm.ApplicationId, sequence.Id);
                var financialSection = await _qnaApiClient.GetSection(vm.ApplicationId, sections.Single(x => x.SectionNo == 3 && x.SequenceNo == 1).Id);
               

                vm.Grade.GradedBy = $"{givenName} {surname}";

                GetFinancialDueDate(vm);
                vm.Grade.ApplicationReference = vm.Id.ToString();
                vm.Grade.GradedDateTime = DateTime.UtcNow;
                vm.Grade.FinancialEvidences = await RetrieveFinancialEvidence(vm.OrgId, vm.ApplicationId);

                if (vm.Grade.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate
               && !string.IsNullOrEmpty(vm.Grade.InadequateMoreInformation))
                {
                    var pageId = financialSection.QnAData.Pages.First(p => p.Active).PageId;
                    var feedback = new QnA.Api.Types.Page.Feedback { Message = vm.Grade.InadequateMoreInformation, From = "Staff member", Date = DateTime.UtcNow, IsNew = true };
                    await _qnaApiClient.UpdateFeedback(vm.ApplicationId, financialSection.Id, pageId, feedback);
                }

                await _apiClient.UpdateFinancialGrade(vm.Id,vm.OrgId, vm.Grade);

                return RedirectToAction("Evaluated", new {vm.Id});   
            }
            else
            {
                
                var grade = new FinancialGrade
                {
                    SelectedGrade = vm.Grade.SelectedGrade,
                    OutstandingFinancialDueDate = vm.Grade.OutstandingFinancialDueDate,
                    GoodFinancialDueDate = vm.Grade.GoodFinancialDueDate,
                    SatisfactoryFinancialDueDate = vm.Grade.SatisfactoryFinancialDueDate
                };

                var newvm = await createFinancialApplicationViewModel(vm.Id,grade);
                return View("~/Views/Apply/Financial/Application.cshtml", newvm);
            }
        }


        [HttpGet("/Financial/{Id}/Evaluated")]
        public async Task<IActionResult> Evaluated(Guid id)
        {
            var applicationFromAssessor = await _apiClient.GetApplicationFromAssessor(id.ToString());
            return View("~/Views/Apply/Financial/Graded.cshtml", applicationFromAssessor.financialGrade);
        }

        private async  Task<FinancialApplicationViewModel> createFinancialApplicationViewModel(Guid id, FinancialGrade grade)
        {
            var applicationFromAssessor = await _apiClient.GetApplicationFromAssessor(id.ToString());
            var sequence = await _qnaApiClient.GetApplicationActiveSequence(applicationFromAssessor.ApplicationId);
            var sections = await _qnaApiClient.GetSections(applicationFromAssessor.ApplicationId, sequence.Id);
            var financialSection = await _qnaApiClient.GetSection(applicationFromAssessor.ApplicationId, sections.Single(x => x.SectionNo == 3 && x.SequenceNo == 1).Id);

            var orgId = applicationFromAssessor?.OrganisationId ?? Guid.Empty;
            var organisation = await _apiClient.GetOrganisation(orgId);

            var application = new AssessorService.ApplyTypes.Application();
            var apply = applicationFromAssessor?.ApplyData.Apply;


            if (apply != null)
                application.ApplicationData = Mapper.Map<AssessorService.ApplyTypes.Apply, ApplicationData>(apply);

            application.ApplyingOrganisation = organisation;
            application.ApplyingOrganisationId = orgId;
            application.ApplicationStatus = applicationFromAssessor.ApplicationStatus;

            return new FinancialApplicationViewModel(id, applicationFromAssessor.ApplicationId, financialSection, grade, application);
        }

        private async Task<List<FinancialEvidence>> RetrieveFinancialEvidence(Guid orgId, Guid applicationId)
        {
            var sequence = await _qnaApiClient.GetApplicationActiveSequence(applicationId);
            var sections = await _qnaApiClient.GetSections(applicationId, sequence.Id);

            var org = await _apiClient.GetOrganisation(orgId);
            var financialSection = await _qnaApiClient.GetSection(applicationId, sections.SingleOrDefault(x => x.SectionNo == 3 && x.SequenceNo == 1).Id);
            var listOfEvidence = new List<FinancialEvidence>();


            var pagesContainingQuestionsWithFileupload = financialSection.QnAData.Pages.Where(x => x.Questions.Any(y => y.Input.Type == "FileUpload")).ToList();
            foreach (var uploadPage in pagesContainingQuestionsWithFileupload)
            {
                foreach (var uploadQuestion in uploadPage.Questions)
                {
                    foreach (var answer in financialSection.QnAData.Pages.SelectMany(p => p.PageOfAnswers).SelectMany(a => a.Answers).Where(a => a.QuestionId == uploadQuestion.QuestionId))
                    {
                        if (string.IsNullOrWhiteSpace(answer.ToString())) continue;
                        listOfEvidence.Add(new FinancialEvidence { Filename = answer.Value });
                    }
                }

            }

            return listOfEvidence;
        }

        private static void GetFinancialDueDate(FinancialApplicationViewModel vm)
        {
            switch (vm?.Grade?.SelectedGrade)
            {
                case FinancialApplicationSelectedGrade.Outstanding:
                    vm.Grade.FinancialDueDate = vm.Grade.OutstandingFinancialDueDate.ToDateTime();
                    break;
                case FinancialApplicationSelectedGrade.Good:
                    vm.Grade.FinancialDueDate = vm.Grade.GoodFinancialDueDate.ToDateTime();
                    break;
                case FinancialApplicationSelectedGrade.Satisfactory:
                    vm.Grade.FinancialDueDate = vm.Grade.SatisfactoryFinancialDueDate.ToDateTime();
                    break;
                case null:
                default:
                    break;

            }
        }

    }
}