﻿using SFA.DAS.AssessorService.Api.Types.Models.Azure;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Azure
{
    public interface IAzureApiClient
    {
        Task<AzureUser> CreateUser(string ukprn);
        Task EnableUser(string userId);
        Task DisableUser(string userId);
        Task<bool> DeleteSubscriptionAndResubscribe(string ukprn, string subscriptionId);
        Task DeleteUser(string userId);
        Task<AzureUser> GetUserDetails(string userId, bool includeSubscriptions = false, bool includeGroups = false);
        Task<IEnumerable<AzureUser>> GetUserDetailsByUkprn(string ukprn, bool includeSubscriptions = false, bool includeGroups = false);
        Task<AzureUser> GetUserDetailsByEmail(string email, bool includeSubscriptions = false, bool includeGroups = false);
        Task<PaginatedList<AzureUser>> ListUsers(int page);
        Task<object> RegeneratePrimarySubscriptionKey(string subscriptionId);
        Task<object> RegenerateSecondarySubscriptionKey(string subscriptionId);
    }
}