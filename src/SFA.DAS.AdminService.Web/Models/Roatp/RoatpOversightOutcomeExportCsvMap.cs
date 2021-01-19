using System.Globalization;
using CsvHelper.Configuration;

namespace SFA.DAS.AdminService.Web.Models.Roatp
{
    public sealed class RoatpOversightOutcomeExportCsvMap : ClassMap<RoatpOversightOutcomeExportViewModel>
    {
        public RoatpOversightOutcomeExportCsvMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(x => x.Id).Ignore();
            Map(x => x.ApplicationId).Ignore();
            Map(x => x.ApplicationSubmittedDate).Name("Application submitted date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(x => x.ApplicationReferenceNumber).Name("Application reference Id");
            Map(x => x.Ukprn).Name("UKPRN");
            Map(x => x.OrganisationName).Name("Legal name");
            Map(x => x.ProviderRoute).Name("Application provider route");
            Map(x => x.IsOnRegister).Name("On RoATP").TypeConverter<BooleanConverter>();
            Map(x => x.ProviderRouteNameOnRegister).Name("Route on RoATP");
            Map(x => x.CompanyNumber).Name("Company No.");
            Map(x => x.OrganisationType).Name("Organisation type");
            Map(x => x.GatewayOutcome).Name("Gateway outcome");
            Map(x => x.AssessorOutcome).Name("Assessor outcome");
            Map(x => x.FHCOutcome).Name("FHC outcome");
            Map(x => x.OverallOutcome).Name("Overall assessment outcome");
        }
    }
}