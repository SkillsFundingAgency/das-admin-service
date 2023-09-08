using System;

namespace SFA.DAS.AdminService.Web.Domain.Apply
{
    public class WithdrawalApplicationDetails
    {
        public Guid ApplicationId { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public int StandardCode { get; set; }
        public DateTime ConfirmedWithdrawalDate { get; set; }
    }

}
