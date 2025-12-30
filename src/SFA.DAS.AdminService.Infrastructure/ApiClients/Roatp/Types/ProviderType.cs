using System.ComponentModel;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;

public enum ProviderType
{
    [Description("Main provider")]
    Main = 1,
    [Description("Employer provider")]
    Employer = 2,
    [Description("Supporting provider")]
    Supporting = 3
}
