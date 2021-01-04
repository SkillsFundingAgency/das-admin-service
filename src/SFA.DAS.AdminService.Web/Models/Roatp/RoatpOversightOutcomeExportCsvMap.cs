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
            Map(x => x.Ukprn).Name("UKPRN");
            Map(x => x.ApplicationSubmittedDate).Name("Application submitted date");
            Map(x => x.ApplicationReferenceNumber).Name("Application reference Id");
            Map(x => x.OrganisationName).Name("Legal name");
            Map(x => x.ProviderRoute).Name("Application provider route");
        }
    }
}