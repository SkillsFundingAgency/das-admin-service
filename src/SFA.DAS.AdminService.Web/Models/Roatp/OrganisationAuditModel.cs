using System.ComponentModel;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;

namespace SFA.DAS.AdminService.Web.Models.Roatp;

public class OrganisationAuditModel
{
    [DisplayName("UKPRN")]
    public int Ukprn { get; set; }
    [DisplayName("Legal name")]
    public string LegalName { get; set; }
    [DisplayName("Field of change")]
    public string FieldChanged { get; set; }
    [DisplayName("Old value")]
    public string PreviousValue { get; set; }
    [DisplayName("Old status date time")]
    public string PreviousStatusDate { get; set; }
    [DisplayName("New value")]
    public string NewValue { get; set; }
    [DisplayName("Change date time")]
    public string UpdatedAt { get; set; }
    [DisplayName("Who")]
    public string UpdatedBy { get; set; }
    public static implicit operator OrganisationAuditModel(OrganisationAuditRecord source)
    {
        return new OrganisationAuditModel
        {
            Ukprn = source.Ukprn,
            LegalName = source.LegalName,
            FieldChanged = source.FieldChanged,
            PreviousValue = source.PreviousValue,
            NewValue = source.NewValue,
            PreviousStatusDate = source.PreviousStatusDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
            UpdatedAt = source.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedBy = source.UpdatedBy
        };
    }
}
