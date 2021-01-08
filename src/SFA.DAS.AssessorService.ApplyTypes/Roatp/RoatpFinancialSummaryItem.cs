using System;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public class RoatpFinancialSummaryItem : RoatpApplicationSummaryItem
    {
        public string DeclaredInApplication { get; set; }

        public DateTime GatewayOutcomeDate { get; set; }

        public string SelectedGrade { get; set; }
    }
}
