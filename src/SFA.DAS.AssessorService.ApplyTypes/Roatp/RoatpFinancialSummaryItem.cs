
namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public class RoatpFinancialSummaryItem : RoatpApplicationSummaryItem
    {
        public string DeclaredInApplication { get; set; }

        public FinancialReviewDetails FinancialReviewDetails { get; set; }
    }
}
