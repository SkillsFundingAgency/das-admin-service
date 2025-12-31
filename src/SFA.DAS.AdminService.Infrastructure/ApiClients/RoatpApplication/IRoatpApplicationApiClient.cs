using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types.AllowedProviders;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication;

public interface IRoatpApplicationApiClient
{
    Task<List<RoatpApplicationOversightDownloadItem>> GetApplicationOversightDetailsForDownload(DateTime dateFrom, DateTime dateTo);
    Task<AllowedProvider> GetAllowedProviderDetails(int ukprn);
    Task<List<AllowedProvider>> GetAllowedProvidersList(string sortColumn, string sortOrder);
    Task<bool> AddToAllowedProviders(int ukprn, DateTime startDate, DateTime endDate);
    Task<bool> RemoveFromAllowedProviders(int ukprn);
}
