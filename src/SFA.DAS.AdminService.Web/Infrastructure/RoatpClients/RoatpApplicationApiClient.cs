﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AdminService.Web.Infrastructure.RoatpClients
{
    public class RoatpApplicationApiClient : RoatpApiClientBase<RoatpApplicationApiClient>, IRoatpApplicationApiClient
    {
        public RoatpApplicationApiClient(string baseUri, ILogger<RoatpApplicationApiClient> logger, IRoatpApplyTokenService tokenService) : base(baseUri, logger, tokenService)
        {
        }

        public async Task<RoatpApply> GetApplication(Guid applicationId)
        {
            return await Get<RoatpApply>($"/Application/{applicationId}");
        }

        public async Task<List<RoatpFinancialSummaryItem>> GetClosedFinancialApplications()
        {
            return await Get<List<RoatpFinancialSummaryItem>>($"/Financial/ClosedApplications");            
        }

        public async Task<List<RoatpFinancialSummaryItem>> GetClarificationFinancialApplications()
        {
            return await Get<List<RoatpFinancialSummaryItem>>($"/Financial/ClarificationApplications");
        }

        public async Task<List<RoatpFinancialSummaryItem>> GetOpenFinancialApplications()
        {
            return await Get<List<RoatpFinancialSummaryItem>>($"/Financial/OpenApplications");
        }

        public async Task<List<RoatpFinancialSummaryDownloadItem>> GetOpenFinancialApplicationsForDownload()
        {
            return await Get<List<RoatpFinancialSummaryDownloadItem>>($"/Financial/OpenApplicationsForDownload");
        }

        public async Task<RoatpFinancialApplicationsStatusCounts> GetFinancialApplicationsStatusCounts()
        {
            return await Get<RoatpFinancialApplicationsStatusCounts>($"/Financial/StatusCounts");
        }

        public async Task ReturnFinancialReview(Guid applicationId, FinancialReviewDetails financialReviewDetails)
        {
            await Post<FinancialReviewDetails>($"/Financial/{applicationId}/Grade", financialReviewDetails);
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

        public async Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest)
        {
            
        }

        public async  Task<Contact> GetContactForApplication(Guid applicationId)
        {
            return await Get<Contact>($"/Application/{applicationId}/Contact");
        }

        public async Task<List<RoatpSequence>> GetRoatpSequences()
        {
            return await Get<List<RoatpSequence>>($"/roatp-sequences");
        }


        public async Task StartAssessorReview(Guid applicationId, string reviewer)
        {
            await Post($"/Application/{applicationId}/StartAssessorReview", new { reviewer });
        }

        public async Task<Guid> SnapshotApplication(Guid applicationId, Guid snapshotApplicationId, List<RoatpApplySequence> sequences)
        {
            return await Post<SnapshotApplicationRequest, Guid>($"/Application/Snapshot", new SnapshotApplicationRequest { ApplicationId = applicationId, SnapshotApplicationId = snapshotApplicationId, Sequences = sequences });
        }

        public async Task<bool> RemoveClarificationFile(Guid applicationId, string userId, string fileName)
        {
            try
            {
                var response = await Post($"/Clarification/Applications/{applicationId}/Remove", new {fileName, userId});

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
    }
}
