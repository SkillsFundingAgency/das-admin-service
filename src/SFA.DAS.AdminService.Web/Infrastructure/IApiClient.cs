using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AdminService.Web.ViewModels.Private;
using OrganisationType = SFA.DAS.AssessorService.Api.Types.Models.AO.OrganisationType;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.QnA.Api.Types.Page;
using Page = SFA.DAS.AssessorService.ApplyTypes.Page;
using FinancialGrade = SFA.DAS.AssessorService.ApplyTypes.FinancialGrade;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IApiClient
    {
        Task ApproveCertificates(CertificatePostApprovalViewModel certificatePostApprovalViewModel);
        Task<PaginatedList<StaffBatchLogResult>> BatchLog(int page);
        Task<PaginatedList<StaffBatchSearchResult>> BatchSearch(int batchNumber, int page);
        Task<ValidationResponse> CreateEpaContactValidate(CreateEpaContactValidationRequest request);
        Task<string> CreateEpaContact(CreateEpaOrganisationContactRequest request);  
        Task<bool> AssociateOrganisationWithEpaContact(AssociateEpaOrganisationWithEpaContactRequest request);
        Task<ValidationResponse> CreateOrganisationValidate(CreateEpaOrganisationValidationRequest request);
        Task<string> CreateEpaOrganisation(CreateEpaOrganisationRequest request);
        Task<ValidationResponse> CreateOrganisationStandardValidate(CreateEpaOrganisationStandardValidationRequest request);
        Task<string> CreateEpaOrganisationStandard(CreateEpaOrganisationStandardRequest request); 
        Task<object> CreateScheduleRun(ScheduleRun schedule);
        Task<object> DeleteScheduleRun(Guid scheduleRunId);
        Task GatherAndCollateStandards();
        Task<IList<ScheduleRun>> GetAllScheduledRun(int scheduleType);
        Task<Certificate> GetCertificate(Guid certificateId);
        Task<List<CertificateResponse>> GetCertificates();
        Task<PaginatedList<CertificateSummaryResponse>> GetCertificatesToBeApproved(int pageSize, int pageIndex,
            string status, string privatelyFundedStatus);
        Task<IEnumerable<IDictionary<string, object>>> GetDataFromStoredProcedure(string storedProcedure);
        Task<List<DeliveryArea>> GetDeliveryAreas();
        Task<AssessmentOrganisationContact> GetEpaContact(string contactId);
        Task<EpaContact> GetEpaContactBySignInId(Guid signInId);
        Task<EpaContact> GetEpaContactByEmail(string email);
        Task<EpaOrganisation> GetEpaOrganisation(string organisationId);
        Task<List<ContactResponse>> GetEpaOrganisationContacts(string organisationId);
        Task<List<OrganisationStandardSummary>> GetEpaOrganisationStandards(string organisationId);
        Task<LearnerDetail> GetLearner(int stdCode, long uln, bool allLogs);
        Task<ScheduleRun> GetNextScheduledRun(int scheduleType);
        Task<ScheduleRun> GetNextScheduleToRunNow();
        Task<List<AssessorService.Domain.Entities.Option>> GetOptions(int stdCode);
        Task<Organisation> GetOrganisation(Guid id);
        Task<OrganisationStandard> GetOrganisationStandard(int organisationStandardId);
        Task<List<OrganisationType>> GetOrganisationTypes();
        Task<IEnumerable<IDictionary<string, object>>> GetReport(Guid reportId);
        Task<ReportDetails> GetReportDetailsFromId(Guid reportId);
        Task<IEnumerable<StaffReport>> GetReportList();
        Task<ReportType> GetReportTypeFromId(Guid reportId);
        Task<ScheduleRun> GetScheduleRun(Guid scheduleRunId);
        Task<string> ImportOrganisations();
        Task<Certificate> PostReprintRequest(StaffCertificateDuplicateRequest staffCertificateDuplicateRequest);
        Task<object> RunNowScheduledRun(int scheduleType);
        Task<StaffSearchResult> Search(string searchString, int page);
        Task<List<AssessmentOrganisationSummary>> SearchOrganisations(string searchString);
        Task<List<StandardCollation>> SearchStandards(string searchString);
        Task<Certificate> UpdateCertificate(UpdateCertificateRequest certificateRequest);
        Task<string> UpdateEpaContact(UpdateEpaOrganisationContactRequest request);
        Task<string> UpdateEpaOrganisation(UpdateEpaOrganisationRequest request);
        Task<string> UpdateEpaOrganisationStandard(UpdateEpaOrganisationStandardRequest request);
        Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest);

        //Apply
        Task ImportWorkflow(IFormFile file);
        Task<HttpResponseMessage> Download(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId, string filename);
        Task<FileInfoResponse> FileInfo(Guid applicationId, Guid userId, int sequenceId, int sectionId, string pageId, string questionId, string filename);
        Task<GetAnswersResponse> GetAnswer(Guid applicationId, string questionTag);
        Task<GetAnswersResponse> GetJsonAnswer(Guid applicationId, string questionTag);
        Task<AssessorService.ApplyTypes.Application> GetApplication(Guid applicationId);
        Task<ApplicationSequence> GetActiveSequence(Guid applicationId);
        Task<ApplicationSequence> GetSequence(Guid applicationId, int sequenceId);
        Task<ApplicationSection> GetSection(Guid applicationId, int sequenceId, int sectionId);
        Task<Organisation> GetOrganisationForApplication(Guid applicationId);
        Task<Contact> GetContact(Guid contactId);
        Task<List<Contact>> GetOrganisationContacts(Guid organisationId);
        Task UpdateRoEpaoApprovedFlag(Guid applicationId, Guid contactId, string endPointAssessorOrganisationId,
            bool roEpaoApprovedFlag);
        Task<List<ApplicationSummaryItem>> GetOpenApplications(int sequenceId);
        Task<List<ApplicationSummaryItem>> GetFeedbackAddedApplications();
        Task<List<ApplicationSummaryItem>> GetClosedApplications();
        Task StartApplicationReview(Guid applicationId, int sequenceId);
        Task EvaluateSection(Guid applicationId, int sequenceId, int sectionId, bool isSectionComplete);
        Task<Page> GetPage(Guid applicationId, int sequenceId, int sectionId, string pageId);
        Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, AssessorService.ApplyTypes.Feedback feedback);
        Task DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId);
        Task ReturnApplication(Guid applicationId, int sequenceId, string returnType);
        Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications();
        Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications();
        Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications();
        Task StartFinancialReview(Guid applicationId);
        Task<HttpResponseMessage> DownloadFile(Guid applicationId, int pageId, string questionId, Guid userId, int sequenceId, int sectionId, string filename);
        Task UpdateFinancialGrade(Guid id, Guid orgId, FinancialGrade vmGrade);
        Task<ApplicationResponse> GetApplicationFromAssessor(string Id);
    }

    public class FileInfoResponse
    {
        public string Filename { get; set; }
        public string ContentType { get; set; }
    }

    public class ApplicationResponse
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public FinancialGrade financialGrade { get; set; }
        public string ApplicationStatus { get; set; }
        public ApplyData ApplyData { get; set; }
    }
}