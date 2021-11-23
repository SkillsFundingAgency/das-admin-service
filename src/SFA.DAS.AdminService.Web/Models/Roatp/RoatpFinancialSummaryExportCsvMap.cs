﻿using System.Globalization;
using CsvHelper.Configuration;

namespace SFA.DAS.AdminService.Web.Models.Roatp
{
    //TODO: Remove after Roatp FHA migration (APR-1823)
    public sealed class RoatpFinancialSummaryExportCsvMap : ClassMap<RoatpFinancialSummaryExportViewModel>
    {
        public RoatpFinancialSummaryExportCsvMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(x => x.ApplicationId).Ignore();
            Map(x => x.ApplicationReference).Name("Application reference Id");
            Map(x => x.Route).Name("Provider route");
            Map(x => x.SubmissionDate).Name("Submitted date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(x => x.GatewayCompletionDate).Name("Gateway complete date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(x => x.ProviderName).Name("Provider legal name");
            Map(x => x.Ukprn).Name("UKPRN");
            Map(x => x.CompanyNo).Name("Company No");
            Map(x => x.CharityNo).Name("Charity No");
            Map(x => x.TurnOver).Name("Turnover");
            Map(x => x.Depreciation).Name("Depreciation/Amortisation");
            Map(x => x.ProfitLoss).Name("Profit/Loss");
            Map(x => x.Dividends);
            Map(x => x.IntangibleAssets).Name("Intangible assets");
            Map(x => x.Assets).Name("Total current assets");
            Map(x => x.Liabilities).Name("Total current liabilities");
            Map(x => x.ShareholderFunds).Name("Shareholder funds/Net assets");
            Map(x => x.Borrowings).Name("Total borrowings");
            Map(x => x.AccountingReferenceDate).Name("Accounting reference date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(x => x.AccountingPeriod).Name("Accounting period");
            Map(x => x.AverageNumberofFTEEmployees).Name("FTE");
        }
    }
}