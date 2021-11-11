using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    //TODO: Remove after Roatp FHA migration (APR-1823)
    public class RoatpFinancialSummaryItem : RoatpApplicationSummaryItem
    {
        public string DeclaredInApplication { get; set; }

        public DateTime GatewayOutcomeDate { get; set; }

        public string SelectedGrade { get; set; }
    }
}
