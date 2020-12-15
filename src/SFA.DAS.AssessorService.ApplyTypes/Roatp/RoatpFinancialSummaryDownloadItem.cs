using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public class RoatpFinancialSummaryDownloadItem : RoatpFinancialSummaryItem
    {
        public FinancialData FinancialData { get; set; }
    }

    public class FinancialData
    {
        public Guid ApplicationId { get; set; }
        public long TurnOver { get; set; }
        public long Depreciation { get; set; }
        public long ProfitLoss { get; set; }
        public long Dividends { get; set; }
        public long IntangibleAssets { get; set; }
        public long Assets { get; set; }
        public long Liabilities { get; set; }
        public long ShareholderFunds { get; set; }
        public long Borrowings { get; set; }
    }

}
