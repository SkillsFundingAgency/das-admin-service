namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    //TODO: Remove after Roatp FHA migration (APR-1823)
    public class RoatpFinancialApplicationsStatusCounts
    {
        public int ApplicationsOpen { get; set; }
        public int ApplicationsWithClarification { get; set; }
        public int ApplicationsClosed { get; set; }
    }
}
