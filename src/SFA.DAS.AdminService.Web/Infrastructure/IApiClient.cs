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
        Task<List<OrganisationStandardSummary>> GetEpaOrganisationStandards(string organisationId);
        Task<LearnerDetail> GetLearner(int stdCode, long uln, bool allLogs);
        Task<ScheduleRun> GetNextScheduledRun(int scheduleType);
        Task<ScheduleRun> GetNextScheduleToRunNow();
        Task<List<Option>> GetOptions(int stdCode);
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
    }
}