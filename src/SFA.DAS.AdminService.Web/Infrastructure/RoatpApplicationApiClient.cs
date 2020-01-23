using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class RoatpApplicationApiClient : IRoatpApplicationApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpApplicationApiClient> _logger;
        private readonly IRoatpApplyTokenService _tokenService;

        public RoatpApplicationApiClient(string baseUri, ILogger<RoatpApplicationApiClient> logger, IRoatpApplyTokenService tokenService)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Feedback feedback)
        {
            
        }

        public async Task DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId)
        {
            
        }

        public async Task EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy)
        {
            
        }

        public async Task<ApplicationResponse> GetApplication(Guid Id)
        {
            return await Task.FromResult(new ApplicationResponse());
        }

        public async Task<List<ApplicationSummaryItem>> GetClosedApplications()
        {
            return await Task.FromResult(new List<ApplicationSummaryItem>());
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications()
        {
            return await Task.FromResult(new List<FinancialApplicationSummaryItem>());
        }

        public async Task<List<ApplicationSummaryItem>> GetFeedbackAddedApplications()
        {
            return await Task.FromResult(new List<ApplicationSummaryItem>());
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications()
        {
            return await Task.FromResult(new List<FinancialApplicationSummaryItem>());
        }

        public async Task<List<ApplicationSummaryItem>> GetOpenApplications(int sequenceNo)
        {
            return await Task.FromResult(new List<ApplicationSummaryItem>());
        }

        public async Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications()
        {
            return await Task.FromResult(new List<FinancialApplicationSummaryItem>());
        }

        public async Task ReturnApplicationSequence(Guid applicationId, int sequenceNo, string returnType, string returnedBy)
        {
            
        }

        public async Task ReturnFinancialReview(Guid applicationId, FinancialGrade grade)
        {
            
        }

        public async Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer)
        {
            
        }

        public async Task StartFinancialReview(Guid applicationId, string reviewer)
        {
            
        }

        public async Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest)
        {
            
        }
    }
}
