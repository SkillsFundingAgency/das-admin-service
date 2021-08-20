using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class WithdrawOrganisationRequest 
    {
        public Guid ApplicationId { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public DateTime WithdrawalDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
