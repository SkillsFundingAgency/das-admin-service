using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types.AllowedProviders;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication
{
    public class RoatpApplicationApiClient : RoatpApiClientBase<RoatpApplicationApiClient>, IRoatpApplicationApiClient
    {
        public RoatpApplicationApiClient(IRoatpApplicationApiClientFactory clientFactory, ILogger<RoatpApplicationApiClient> logger)
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task StartApplicationSectionReview(Guid applicationId, int sequenceId, int sectionId, string reviewer)
        {
            await Post($"/Application/{applicationId}/StartAssessorSectionReview",
                new { applicationId, sequenceId, sectionId, reviewer });
        }

        public async Task StartFinancialReview(Guid applicationId, string reviewer)
        {
            await Post($"/Financial/{applicationId}/StartReview", new { reviewer });
        }

        public async Task<Contact> GetContactForApplication(Guid applicationId)
        {
            return await Get<Contact>($"/Application/{applicationId}/Contact");
        }

        public async Task StartAssessorReview(Guid applicationId, string reviewer)
        {
            await Post($"/Application/{applicationId}/StartAssessorReview", new { reviewer });
        }

        public async Task<bool> RemoveClarificationFile(Guid applicationId, string userId, string fileName)
        {
            try
            {
                var response = await Post($"/Clarification/Applications/{applicationId}/Remove", new { fileName, userId });

                return response == HttpStatusCode.OK;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    $"Error when submitting Clarification File removal for Application: {applicationId} | Filename: {fileName}");
                return false;
            }
        }

        public async Task<HttpResponseMessage> DownloadClarificationFile(Guid applicationId, string filename)
        {
            var response = await _client.GetAsync($"/Clarification/Applications/{applicationId}/Download/{filename}");

            return response;
        }

        public async Task<List<RoatpApplicationOversightDownloadItem>> GetApplicationOversightDetailsForDownload(DateTime dateFrom, DateTime dateTo)
        {
            return await Get<List<RoatpApplicationOversightDownloadItem>>($"Oversights/Download?dateFrom={dateFrom:yyyy-MM-dd}&dateTo={dateTo:yyyy-MM-dd}");
        }

        public async Task<bool> UploadClarificationFile(Guid applicationId, string userId, IFormFileCollection clarificationFiles)
        {
            var fileName = string.Empty;
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(userId), "userId");

            if (clarificationFiles != null && clarificationFiles.Any())
            {
                foreach (var file in clarificationFiles)
                {
                    fileName = file.FileName;
                    var fileContent = new StreamContent(file.OpenReadStream())
                    {
                        Headers =
                        {
                            ContentLength = file.Length, ContentType = new MediaTypeHeaderValue(file.ContentType)
                        }
                    };
                    content.Add(fileContent, file.FileName, file.FileName);
                }

                try
                {
                    var response = await _client.PostAsync($"/Clarification/Applications/{applicationId}/Upload", content);

                    return response.StatusCode == HttpStatusCode.OK;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex,
                        $"Error when submitting Clarification File update for Application: {applicationId} | Filename: {fileName}");
                    return false;
                }
            }

            return true;
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
            try
            {
                var response = await Post($"/AllowedProviders", new AllowedProvider { Ukprn = ukprn, StartDateTime = startDate, EndDateTime = endDate });
                return response == HttpStatusCode.OK;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when adding UKPRN {ukprn} to Allowed Providers list");
                return false;
            }
        }

        public async Task<bool> RemoveFromAllowedProviders(int ukprn)
        {
            try
            {
                var response = await Delete($"/AllowedProviders/{ukprn}");
                return response == HttpStatusCode.OK;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when adding UKPRN {ukprn} to Allowed Providers list");
                return false;
            }
        }
    }
}
