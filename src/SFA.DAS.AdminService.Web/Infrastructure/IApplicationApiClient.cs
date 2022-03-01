using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using FinancialGrade = SFA.DAS.AssessorService.ApplyTypes.FinancialGrade;
using UpdateFinancialsRequest = SFA.DAS.AssessorService.Api.Types.Models.Register.UpdateFinancialsRequest;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IApplicationApiClient
    {
        Task<ApplicationReviewStatusCounts> GetApplicationReviewStatusCounts();
    }

    public class ApplicationResponse
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string ApplicationType { get; set; }
        public Guid OrganisationId { get; set; }
        public string EndPointAssessorName { get; set; }
        public FinancialGrade FinancialGrade { get; set; }
        public string ApplicationStatus { get; set; }
        public string ReviewStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public ApplyData ApplyData { get; set; }
        public int? StandardCode { get; set; }
        public string CreatedBy { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string StandardApplicationType { get; set; }
    }
}