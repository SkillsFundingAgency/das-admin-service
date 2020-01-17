
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public interface IApiClientConfig
    {
        ITokenService GetTokenService(ApplicationType applicationType);
        string GetBaseAddress(ApplicationType applicationType);
    }
}
