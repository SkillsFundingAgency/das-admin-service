using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types.AllowedProviders;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication;

public class RoatpApplicationApiClient : RoatpApiClientBase<RoatpApplicationApiClient>, IRoatpApplicationApiClient
{
    public RoatpApplicationApiClient(IRoatpApplicationApiClientFactory clientFactory, ILogger<RoatpApplicationApiClient> logger)
        : base(clientFactory.CreateHttpClient(), logger)
    {
    }

    public async Task<List<RoatpApplicationOversightDownloadItem>> GetApplicationOversightDetailsForDownload(DateTime dateFrom, DateTime dateTo)
    {
        return await Get<List<RoatpApplicationOversightDownloadItem>>($"Oversights/Download?dateFrom={dateFrom:yyyy-MM-dd}&dateTo={dateTo:yyyy-MM-dd}");
    }

    public async Task<AllowedProvider> GetAllowedProviderDetails(int ukprn)
    {
        return await Get<AllowedProvider>($"/AllowedProviders/{ukprn}");
    }

    public async Task<List<AllowedProvider>> GetAllowedProvidersList(string sortColumn, string sortOrder)
    {
        return await Get<List<AllowedProvider>>($"/AllowedProviders?sortColumn={sortColumn}&sortOrder={sortOrder}");
    }

    public async Task<bool> AddToAllowedProviders(int ukprn, DateTime startDate, DateTime endDate)
    {
        var response = await Post($"/AllowedProviders", new AllowedProvider { Ukprn = ukprn, StartDateTime = startDate, EndDateTime = endDate });
        return response == HttpStatusCode.OK;
    }

    public async Task<bool> RemoveFromAllowedProviders(int ukprn)
    {
        var response = await Delete($"/AllowedProviders/{ukprn}");
        return response == HttpStatusCode.OK;
    }
}
