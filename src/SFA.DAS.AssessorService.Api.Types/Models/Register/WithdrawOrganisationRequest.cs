using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class WithdrawOrganisationRequest 
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public DateTime WithdrawalDate { get; set; }
    }
}
