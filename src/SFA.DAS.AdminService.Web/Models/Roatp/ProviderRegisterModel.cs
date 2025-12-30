using System.ComponentModel;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;

namespace SFA.DAS.AdminService.Web.Models.Roatp;

public class ProviderRegisterModel
{
    [DisplayName("Provider type")]
    public ProviderType ProviderType { get; set; }
    [DisplayName("UKPRN")]
    public int Ukprn { get; set; }
    [DisplayName("Legal name")]
    public string LegalName { get; set; }
    [DisplayName("Trading name")]
    public string TradingName { get; set; }
    [DisplayName("Organisation type")]
    public string OrganisationType { get; set; }
    [DisplayName("Company number")]
    public string CompanyNumber { get; set; }
    [DisplayName("Charity number")]
    public string CharityNumber { get; set; }
    public OrganisationStatus Status { get; set; }
    [DisplayName("Status date")]
    public string StatusDate { get; set; }
    [DisplayName("Reason")]
    public string RemovedReason { get; set; }
    [DisplayName("Date joined")]
    public string StartDate { get; set; }

    public static implicit operator ProviderRegisterModel(OrganisationModel source)
        => new()
        {
            ProviderType = source.ProviderType,
            Ukprn = source.Ukprn,
            LegalName = source.LegalName,
            TradingName = source.TradingName,
            OrganisationType = source.OrganisationType,
            CompanyNumber = source.CompanyNumber,
            CharityNumber = source.CharityNumber,
            Status = source.Status,
            StatusDate = source.StatusDate.ToString("dd/MM/yyyy"),
            RemovedReason = source.RemovedReason,
            StartDate = source.StartDate?.ToString("dd/MM/yyyy")
        };
}
