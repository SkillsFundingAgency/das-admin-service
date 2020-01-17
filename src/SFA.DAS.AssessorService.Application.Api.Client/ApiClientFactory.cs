using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class ApiClientFactory<T> where T : ApiClientBase
    {
        private readonly IApiClientConfig _apiClientConfig;
        private readonly ILogger<ApiClientBase> _logger;

        public ApiClientFactory() 
        {
        }

        public ApiClientFactory(IApiClientConfig apiClientConfig, ILogger<ApiClientBase> logger)
        {
            _apiClientConfig = apiClientConfig;
            _logger = logger;
        }

        public virtual T GetApiClient(ApplicationType applicationType)
        {
            return (T)Activator.CreateInstance(typeof(T), 
                _apiClientConfig.GetBaseAddress(applicationType), 
                _apiClientConfig.GetTokenService(applicationType), 
                _logger);
        }
    }
}
