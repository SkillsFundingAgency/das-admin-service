using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AdminService.Web.ViewModels.Merge
{
    public class ConfirmEpaoViewModel
    {
        public string Name { get; set; }
        public string EpaoId { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryContactEmail { get; set; }
        public long? Ukprn { get; set; }
        public string OrganisationType { get; set; } 
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
        public string Postcode { get; set; }

        public string MergeOrganisationType { get; set; }
        public string BackRouteSearchString { get; set; }

        public ConfirmEpaoViewModel() { }

        public ConfirmEpaoViewModel(EpaOrganisation epao, string mergeOrganisationType, string searchString)
        {
            Name = epao.Name;
            EpaoId = epao.OrganisationId;
            PrimaryContactName = epao.PrimaryContactName;
            PrimaryContactEmail = epao.PrimaryContact;
            Ukprn = epao.Ukprn;
            LegalName = epao.OrganisationData.LegalName;
            OrganisationType = epao.OrganisationData.OrganisationReferenceType;
            TradingName = epao.OrganisationData.TradingName;
            CompanyNumber = epao.OrganisationData.CompanyNumber;
            Email = epao.OrganisationData.Email;
            PhoneNumber = epao.OrganisationData.PhoneNumber;
            Website = epao.OrganisationData.WebsiteLink;
            Address1 = epao.OrganisationData.Address1;
            Address2 = epao.OrganisationData.Address2;
            Address3 = epao.OrganisationData.Address3;
            Address4 = epao.OrganisationData.Address4;
            Postcode = epao.OrganisationData.Postcode;

            MergeOrganisationType = mergeOrganisationType.ToLower();
            BackRouteSearchString = searchString;
        }
    }
}
