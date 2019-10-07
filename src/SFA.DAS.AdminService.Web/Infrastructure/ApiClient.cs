using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AdminService.Web.ViewModels.Private;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using OrganisationType = SFA.DAS.AssessorService.Api.Types.Models.AO.OrganisationType;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AdminService.Web.Services;
using Microsoft.AspNetCore.Http;
using Page = SFA.DAS.AssessorService.ApplyTypes.Page;
using FinancialGrade = SFA.DAS.AssessorService.ApplyTypes.FinancialGrade;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiClient> _logger;
        private readonly ITokenService _tokenService;

        public ApiClient(HttpClient client, ILogger<ApiClient> logger, ITokenService tokenService)
        {
            _client = client;
            _logger = logger;
            _tokenService = tokenService;
        }

        public ApiClient(string baseUri, ILogger<ApiClient> logger, ITokenService tokenService)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            _logger = logger;
            _tokenService = tokenService;
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        private async Task<U> Post<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

        private async Task Post<T>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"))) { }
        }

        protected async Task<U> Put<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PutAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

        protected async Task<T> Delete<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.DeleteAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        public async Task<List<CertificateResponse>> GetCertificates()
        {
            return await Get<List<CertificateResponse>>("/api/v1/certificates?statusses=Submitted");
        }

        public async Task<PaginatedList<CertificateSummaryResponse>> GetCertificatesToBeApproved(int pageSize, int pageIndex, string status, string privatelyFundedStatus)
        {
            return await Get<PaginatedList<CertificateSummaryResponse>>($"/api/v1/certificates/approvals/?pageSize={pageSize}&pageIndex={pageIndex}&status={status}&privatelyFundedStatus={privatelyFundedStatus}");
        }

        public async Task<StaffSearchResult> Search(string searchString, int page)
        {
            return await Get<StaffSearchResult>($"/api/v1/staffsearch?searchQuery={searchString}&page={page}");
        }

        public async Task<List<AssessmentOrganisationSummary>> SearchOrganisations(string searchString)
        {
            return await Get<List<AssessmentOrganisationSummary>>(
                $"/api/ao/assessment-organisations/search/{searchString}");
        }

        public async Task<string> ImportOrganisations()
        {
            var uri = "/api/ao/assessment-organisations/";
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.PatchAsync(new Uri(uri, UriKind.Relative), null))
            {
                var res = await response.Content.ReadAsAsync<AssessmentOrgsImportResponse>();
                return res.Status;
            }
        }


        public async Task<List<OrganisationType>> GetOrganisationTypes()
        {
            return await Get<List<OrganisationType>>($"/api/ao/organisation-types");
        }
        

        public async Task<EpaOrganisation> GetEpaOrganisation(string organisationId)
        {
            return await Get<EpaOrganisation>($"api/ao/assessment-organisations/{organisationId}");
        }
        
        public async Task<AssessmentOrganisationContact> GetEpaContact(string contactId)
        {
            return await Get<AssessmentOrganisationContact>($"api/ao/assessment-organisations/contacts/{contactId}");
        }

        public async Task<EpaContact> GetEpaContactBySignInId(Guid signInId)
        {
            return await Get<EpaContact>($"api/ao/assessment-organisations/contacts/signInId/{signInId.ToString()}");
        }

        public async Task<EpaContact> GetEpaContactByEmail(string email)
        {
            return await Get<EpaContact>($"api/ao/assessment-organisations/contacts/email/{email}");
        }

        public async Task<List<ContactResponse>> GetEpaOrganisationContacts(string organisationId)
        {
            return await Get<List<ContactResponse>>($"api/v1/contacts/get-all/{organisationId}");
        }

        public async Task<List<OrganisationStandardSummary>> GetEpaOrganisationStandards(string organisationId)
        {
            return await Get<List<OrganisationStandardSummary>>($"/api/ao/assessment-organisations/{organisationId}/standards");
        }

        public async Task<ValidationResponse> CreateOrganisationValidate(CreateEpaOrganisationValidationRequest request)
        {
            return await Post<CreateEpaOrganisationValidationRequest, ValidationResponse>("api/ao/assessment-organisations/validate-new", request);
        }

        public async Task<string> CreateEpaOrganisation(CreateEpaOrganisationRequest request)
        {
            var result =
                await Post<CreateEpaOrganisationRequest, EpaOrganisationResponse>("api/ao/assessment-organisations",
                    request);
            return result.Details;
        }

        public async Task<ValidationResponse> CreateOrganisationStandardValidate(CreateEpaOrganisationStandardValidationRequest request)
        {
            return await Post<CreateEpaOrganisationStandardValidationRequest, ValidationResponse>("api/ao/assessment-organisations/standards/validate-new", request);
        }

        public async Task<string> CreateEpaOrganisationStandard(CreateEpaOrganisationStandardRequest request)
        {
            var result =
                await Post<CreateEpaOrganisationStandardRequest, EpaoStandardResponse>("api/ao/assessment-organisations/standards",
                    request);
            return result.Details;
        }

        public async Task<string> UpdateEpaOrganisationStandard(UpdateEpaOrganisationStandardRequest request)
        {
            var result =
                await Put<UpdateEpaOrganisationStandardRequest, EpaoStandardResponse>("api/ao/assessment-organisations/standards",
                    request);
            return result.Details;
        }

        public async Task<string> UpdateEpaOrganisation(UpdateEpaOrganisationRequest request)
        {
            var result = await Put<UpdateEpaOrganisationRequest, EpaOrganisationResponse>("api/ao/assessment-organisations", request);
            return result.Details;
        }

        public async Task<ValidationResponse> CreateEpaContactValidate(CreateEpaContactValidationRequest request)
        {
            return await Post<CreateEpaContactValidationRequest, ValidationResponse>("api/ao/assessment-organisations/contacts/validate-new", request);
        }

        public async Task<string> CreateEpaContact(CreateEpaOrganisationContactRequest request)
        {
            var result = await Post<CreateEpaOrganisationContactRequest, EpaOrganisationContactResponse>("api/ao/assessment-organisations/contacts", request);
            return result.Details;
        }

        public async Task<string> UpdateEpaContact(UpdateEpaOrganisationContactRequest request)
        {
            var result = await Put<UpdateEpaOrganisationContactRequest, EpaOrganisationContactResponse>("api/ao/assessment-organisations/contacts", request);
            return result.Details;
        }

        public async Task<bool> AssociateOrganisationWithEpaContact(AssociateEpaOrganisationWithEpaContactRequest request)
        {
            return await Put<AssociateEpaOrganisationWithEpaContactRequest, bool>("api/ao/assessment-organisations/contacts/associate-organisation", request);
        }

        public async Task<PaginatedList<StaffBatchSearchResult>> BatchSearch(int batchNumber, int page)
        {
            return await Get<PaginatedList<StaffBatchSearchResult>>(
                $"/api/v1/staffsearch/batch?batchNumber={batchNumber}&page={page}");
        }

        public async Task<PaginatedList<StaffBatchLogResult>> BatchLog(int page)
        {
            return await Get<PaginatedList<StaffBatchLogResult>>($"/api/v1/staffsearch/batchlog?page={page}");
        }

        public async Task<LearnerDetail> GetLearner(int stdCode, long uln, bool allLogs)
        {
            return await Get<LearnerDetail>($"/api/v1/learnerDetails?stdCode={stdCode}&uln={uln}&alllogs={allLogs}");
        }

        public async Task<Certificate> GetCertificate(Guid certificateId)
        {
            return await Get<Certificate>($"api/v1/certificates/{certificateId}");
        }

        public async Task<Organisation> GetOrganisation(Guid id)
        {
            return await Get<Organisation>($"/api/v1/organisations/organisation/{id}");
        }

        public async Task<List<AssessorService.Domain.Entities.Option>> GetOptions(int stdCode)
        {
            return await Get<List<AssessorService.Domain.Entities.Option>>($"api/v1/certificates/options/?stdCode={stdCode}");
        }

        public async Task<Certificate> UpdateCertificate(UpdateCertificateRequest certificateRequest)
        {
            return await Put<UpdateCertificateRequest, Certificate>("api/v1/certificates/update", certificateRequest);
        }

        public async Task<ScheduleRun> GetNextScheduleToRunNow()
        {
            return await Get<ScheduleRun>($"api/v1/schedule?scheduleType=1");
        }

        public async Task<ScheduleRun> GetNextScheduledRun(int scheduleType)
        {
            return await Get<ScheduleRun>($"api/v1/schedule/next?scheduleType={scheduleType}");
        }

        public async Task<object> RunNowScheduledRun(int scheduleType)
        {
            return await Post<object, object>($"api/v1/schedule/runnow?scheduleType={scheduleType}", default(object));
        }

        public async Task<object> CreateScheduleRun(ScheduleRun schedule)
        {
            return await Put<ScheduleRun, object>($"api/v1/schedule/create", schedule);
        }

        public async Task<ScheduleRun> GetScheduleRun(Guid scheduleRunId)
        {
            return await Get<ScheduleRun>($"api/v1/schedule?scheduleRunId={scheduleRunId}");
        }

        public async Task<IList<ScheduleRun>> GetAllScheduledRun(int scheduleType)
        {
            return await Get<IList<ScheduleRun>>($"api/v1/schedule/all?scheduleType={scheduleType}");
        }

        public async Task<object> DeleteScheduleRun(Guid scheduleRunId)
        {
            return await Delete<object>($"api/v1/schedule?scheduleRunId={scheduleRunId}");
        }

        public async Task<Certificate> PostReprintRequest(
            StaffCertificateDuplicateRequest staffCertificateDuplicateRequest)
        {
            return await Post<StaffCertificateDuplicateRequest, Certificate>("api/v1/staffcertificatereprint",
                staffCertificateDuplicateRequest);
                }

        public async Task<List<StandardCollation>> SearchStandards(string searchString)
        {
            return await Get<List<StandardCollation>>($"/api/ao/assessment-organisations/standards/search/{searchString}");
        }

        public async Task ApproveCertificates(CertificatePostApprovalViewModel certificatePostApprovalViewModel)
        {
            await Post<CertificatePostApprovalViewModel>("api/v1/certificates/approvals",
                certificatePostApprovalViewModel);
        }

        public async Task<List<DeliveryArea>> GetDeliveryAreas()
        {
            return await Get<List<DeliveryArea>>("/api/ao/delivery-areas");
        }

        public async Task<OrganisationStandard> GetOrganisationStandard(int organisationStandardId)
        {
            return await Get<OrganisationStandard>($"/api/ao/assessment-organisations/organisation-standard/{organisationStandardId}");
        }

        #region Reports
        public async Task<IEnumerable<StaffReport>> GetReportList()
        {
            return await Get<IEnumerable<StaffReport>>($"api/v1/staffreports");
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetReport(Guid reportId)
        {
            return await Get<IEnumerable<IDictionary<string, object>>>($"api/v1/staffreports/{reportId}");
        }

        public async Task<ReportType> GetReportTypeFromId(Guid reportId)
        {
            return await Get<ReportType>($"api/v1/staffreports/{reportId}/report-type");
        }

        public async Task<ReportDetails> GetReportDetailsFromId(Guid reportId)
        {
            return await Get<ReportDetails>($"api/v1/staffreports/{reportId}/report-details");
        }

        public async Task GatherAndCollateStandards()
        {
             await Post($"api/ao/update-standards", new GatherStandardsRequest());
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetDataFromStoredProcedure(string storedProcedure)
        {
            return await Get<IEnumerable<IDictionary<string, object>>>($"api/v1/staffreports/report-content/{storedProcedure}");
        }
        #endregion

        public async Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest)
        {
            await Post("api/ao/assessment-organisations/update-financials", updateFinancialsRequest);
        }

        #region Apply
        public async Task<List<ApplicationSummaryItem>> GetOpenApplications(int sequenceId)
        {
            return await Get<List<ApplicationSummaryItem>>($"/Review/OpenApplications?sequenceNo={sequenceId}");
        }

        public async Task<List<ApplicationSummaryItem>> GetFeedbackAddedApplications()
        {
            return await Get<List<ApplicationSummaryItem>>($"/Review/FeedbackAddedApplications");
        }

        public async Task<List<ApplicationSummaryItem>> GetClosedApplications()
        {
            return await Get<List<ApplicationSummaryItem>>($"/Review/ClosedApplications");
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications()
        {
            return await Get<List<FinancialApplicationSummaryItem>>($"/Financial/OpenApplications");
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications()
        {
            return await Get<List<FinancialApplicationSummaryItem>>($"/Financial/FeedbackAddedApplications");
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications()
        {
            return await Get<List<FinancialApplicationSummaryItem>>($"/Financial/ClosedApplications");
        }

        public async Task ImportWorkflow(IFormFile file)
        {
            var formDataContent = new MultipartFormDataContent();

            var fileContent = new StreamContent(file.OpenReadStream())
            { Headers = { ContentLength = file.Length, ContentType = new MediaTypeHeaderValue(file.ContentType) } };
            formDataContent.Add(fileContent, file.Name, file.FileName);

            await _client.PostAsync($"/Import/Workflow", formDataContent);
        }

        public async Task<AssessorService.ApplyTypes.Application> GetApplication(Guid applicationId)
        {
            return await Get<AssessorService.ApplyTypes.Application>($"/Application/{applicationId}");
        }

        public async Task<ApplicationResponse> GetApplicationFromAssessor(string Id)
        {
            return await Get<ApplicationResponse>($"/api/v1/applications/{Id}/application");
        }

        public async Task<ApplicationSequence> GetActiveSequence(Guid applicationId)
        {
            return await Get<ApplicationSequence>($"/Review/Applications/{applicationId}");
        }

        public async Task<ApplicationSequence> GetSequence(Guid applicationId, int sequenceId)
        {
            return await Get<ApplicationSequence>($"Application/{applicationId}/User/null/Sequences/{sequenceId}");
        }

        public async Task<ApplicationSection> GetSection(Guid applicationId, int sequenceId, int sectionId)
        {
            return await Get<ApplicationSection>($"Application/{applicationId}/User/null/Sequences/{sequenceId}/Sections/{sectionId}");
        }

        public async Task EvaluateSection(Guid applicationId, int sequenceId, int sectionId, bool isSectionComplete)
        {
            await Post($"Review/Applications/{applicationId}/Sequences/{sequenceId}/Sections/{sectionId}/Evaluate",
                new { isSectionComplete });
        }

        public async Task<Page> GetPage(Guid applicationId, int sequenceId, int sectionId, string pageId)
        {
            var page = await Get<Page>($"Application/{applicationId}/User/null/Sequence/{sequenceId}/Sections/{sectionId}/Pages/{pageId}");
           if (page != null) page.ApplicationId = applicationId;
            return page;
        }

        public async Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, AssessorService.ApplyTypes.Feedback feedback)
        {
            await Post(
                $"Review/Applications/{applicationId}/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{pageId}/AddFeedback",
                feedback);
        }

        public async Task DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId)
        {
            await Post(
                $"Review/Applications/{applicationId}/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{pageId}/DeleteFeedback",
                feedbackId);
        }

        public async Task ReturnApplicationSequence(Guid applicationId, int sequenceId, string returnType, string returnedBy)
        {
            await Post($"Review/Applications/{applicationId}/Sequences/{sequenceId}/Return", new { returnType, returnedBy });
        }

        public async Task<HttpResponseMessage> DownloadFile(Guid applicationId, int pageId, string questionId, Guid userId, int sequenceId, int sectionId, string filename)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            return await _client.GetAsync(new Uri($"/Download/Application/{applicationId}/User/{userId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}/Question/{questionId}/{filename}", UriKind.Relative));
        }

        public async Task<HttpResponseMessage> Download(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId, string filename)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());


            var downloadResponse = await _client.GetAsync(
                $"/Download/Application/{applicationId}/User/{userId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}/Question/{questionId}/{filename}");
            return downloadResponse;
        }

        public async Task<FileInfoResponse> FileInfo(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId, string filename)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            var downloadResponse = await (await _client.GetAsync(
                $"/FileInfo/Application/{applicationId}/User/{userId}/Sequence/{sequenceId}/Section/{sectionId}/Page/{pageId}/Question/{questionId}/{filename}")).Content.ReadAsAsync<FileInfoResponse>();
            return downloadResponse;
        }

        public async Task StartFinancialReview(Guid applicationId)
        {
            await Post($"/Financial/{applicationId}/StartReview", new { applicationId });
        }

        public async Task ReturnFinancialReview(Guid applicationId, FinancialGrade grade)
        {
            await Post($"/Financial/{applicationId}/Return", grade);
        }

        public async Task<Organisation> GetOrganisationForApplication(Guid applicationId)
        {
            return await Get<Organisation>($"/Application/{applicationId}/Organisation");
        }

        public async Task StartApplicationReview(Guid applicationId, int sequenceNo)
        {
            await Post($"/Review/Applications/{applicationId}/Sequences/{sequenceNo}/StartReview", new { sequenceNo });
        }

        public async Task<GetAnswersResponse> GetAnswer(Guid applicationId, string questionTag)
        {
            return await Get<GetAnswersResponse>($"/Answer/{questionTag}/{applicationId}");
        }

        public async Task<GetAnswersResponse> GetJsonAnswer(Guid applicationId, string questionTag)
        {
            return await Get<GetAnswersResponse>($"/JsonAnswer/{questionTag}/{applicationId}");
        }

        public async Task<List<Contact>> GetOrganisationContacts(Guid organisationId)
        {
            return await Get<List<Contact>>($"/Account/Organisation/{organisationId}/Contacts");
        }

        public async Task<Contact> GetContact(Guid contactId)
        {
            return await Get<Contact>($"/Account/Contact/{contactId}");
        }

        public async Task UpdateRoEpaoApprovedFlag(Guid applicationId, Guid contactId, string endPointAssessorOrganisationId, bool roEpaoApprovedFlag)
        {
            await Post($"/organisations/{applicationId}/{contactId}/{endPointAssessorOrganisationId}/RoEpaoApproved/{roEpaoApprovedFlag}", new { roEpaoApprovedFlag });
        }
        #endregion

    }
}