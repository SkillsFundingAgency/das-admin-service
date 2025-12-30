using System.ComponentModel;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;

public enum OrganisationStatus
{
    Removed = 0,
    Active = 1,
    [Description("Active - but not taking on apprentices")]
    ActiveNoStarts = 2,
    [Description("On-boarding")]
    OnBoarding = 3
}
