using System;

namespace SFA.DAS.AdminService.Web.Models.Roatp
{
    public class RoatpFinancialSummaryExportViewModel
    {
        public Guid ApplicationId { get; set; }
        public string ApplicationReference { get; set; }
        public string Route { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime GatewayCompletionDate { get; set; }
        public string ProviderName { get; set; }
        public string Ukprn { get; set; }
        public string CompanyNo { get; set; }
        public string CharityNo { get; set; }
        public long? TurnOver { get; set; }
        public long? Depreciation { get; set; }
        public long? ProfitLoss { get; set; }
        public long? Dividends { get; set; }
        public long? IntangibleAssets { get; set; }
        public long? Assets { get; set; }
        public long? Liabilities { get; set; }
        public long? ShareholderFunds { get; set; }
        public long? Borrowings { get; set; }
        public DateTime? AccountingReferenceDate { get; set; }
        public byte? AccountingPeriod { get; set; }
    }
}
