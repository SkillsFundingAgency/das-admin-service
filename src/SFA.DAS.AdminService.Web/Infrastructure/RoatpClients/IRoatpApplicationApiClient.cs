using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.AllowedProviders;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AdminService.Web.Infrastructure.RoatpClients
{
    public interface IRoatpApplicationApiClient
    {
        Task<RoatpApply> GetApplication(Guid applicationId);
        Task<FinancialReviewDetails> GetFinancialReviewDetails(Guid applicationId);
        Task<List<RoatpFinancialSummaryItem>> GetClosedFinancialApplications(string searchTerm, string sortColumn, string sortOrder);
        Task<List<RoatpFinancialSummaryItem>> GetClarificationFinancialApplications(string searchTerm, string sortColumn, string sortOrder);
        Task<List<RoatpFinancialSummaryItem>> GetOpenFinancialApplications(string searchTerm, string sortColumn, string sortOrder);
        Task<List<RoatpFinancialSummaryDownloadItem>> GetOpenFinancialApplicationsForDownload();
        Task<RoatpFinancialApplicationsStatusCounts> GetFinancialApplicationsStatusCounts(string searchTerm);

        Task ReturnFinancialReview(Guid applicationId, FinancialReviewDetails financialReviewDetails);
        Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer);
        Task StartFinancialReview(Guid applicationId, string reviewer);

        Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest);

        Task<Contact> GetContactForApplication(Guid applicationId);

        Task<List<RoatpSequence>> GetRoatpSequences();

        Task StartAssessorReview(Guid applicationId, string reviewer);

        Task<Guid> SnapshotApplication(Guid Id, Guid NewApplicationId, List<RoatpApplySequence> sequences);


        Task<bool> UploadClarificationFile(Guid applicationId, string userId, IFormFileCollection clarificationFiles);
        Task<bool> RemoveClarificationFile(Guid applicationId, string userId, string filename);
        Task<HttpResponseMessage> DownloadClarificationFile(Guid applicationId, string filename);
        Task<List<RoatpApplicationOversightDownloadItem>> GetApplicationOversightDetailsForDownload(DateTime dateFrom, DateTime dateTo);

        Task<AllowedProvider> GetAllowedProviderDetails(int ukprn);
        Task<List<AllowedProvider>> GetAllowedProvidersList(string sortColumn, string sortOrder);
        Task<bool> AddToAllowedProviders(int ukprn, DateTime startDate, DateTime endDate);
        Task<bool> RemoveFromAllowedProviders(int ukprn);
    }
}
