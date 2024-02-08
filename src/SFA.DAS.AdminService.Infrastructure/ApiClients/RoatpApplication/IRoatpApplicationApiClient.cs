using Microsoft.AspNetCore.Http;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types.AllowedProviders;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types.Apply;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication
{
    public interface IRoatpApplicationApiClient
    {
        Task<RoatpApply> GetApplication(Guid applicationId);
        Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer);
        Task StartFinancialReview(Guid applicationId, string reviewer);

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
