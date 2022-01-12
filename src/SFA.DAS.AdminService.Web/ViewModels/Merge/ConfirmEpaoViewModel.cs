using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AdminService.Web.ViewModels.Merge
{
    public class ConfirmEpaoViewModel
    {
        public string Name { get; set; }
        public string EpaoId { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryContactEmail { get; set; }
        public long Ukprn { get; set; }
        public string EpaoOrganisationType { get; set; } // Rename to OrganisationType once I have renamed my flag
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }

        // my flag - need to change as OrganisationType exists on EpaOrganisation object
        public string OrganisationType { get; set; }

        public ConfirmEpaoViewModel() { }

        public ConfirmEpaoViewModel(EpaOrganisation epao, string organisationType)
        {
            Name = epao.Name;
            EpaoId = epao.OrganisationId;
            PrimaryContactName = epao.PrimaryContactName;
            PrimaryContactEmail = epao.PrimaryContact;

            OrganisationType = organisationType.ToLower();
        }
    }
}
