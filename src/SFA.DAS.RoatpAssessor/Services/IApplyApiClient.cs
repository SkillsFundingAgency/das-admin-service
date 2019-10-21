using SFA.DAS.RoatpAssessor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Services
{
    public interface IApplyApiClient
    {
        Task<IEnumerable<Domain.Entities.Application>> GetSubmittedApplicationsAsync();
        Task<Domain.Entities.Application> GetApplicationAsync(Guid applicationId);
        Task<Guid> CreateApplicationReview(Guid applicationId);
        Task<ApplicationReview> GetApplicationReviewAsync(Guid applicationId);
        Task UpdateApplicationReviewGatewayReviewAsync(Guid applicationId, ApplicationReviewStatus status);
        Task UpdateApplicationReviewPmoReviewAsync(Guid applicationId, ApplicationReviewStatus status);
        Task UpdateApplicationReviewAssessorReviewAsync(Guid applicationId, AssessorReviewNo reviewNo, ApplicationReviewStatus status);
        Task<IEnumerable<ApplicationReview>> GetActiveApplicationReviewsAsync();
        Task UpdateAssessorComments(Guid applicationId, AssessorReviewNo reviewNo, Guid sectionId, string pageId, string comment);
        Task UpdateApplicationReviewAssessorModerationAsync(Guid applicationId, ApplicationReviewStatus status);

    }
}