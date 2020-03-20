using SFA.DAS.AdminService.Web.ViewModels.Roatp.Applications;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Services
{
    public interface IRoatpApplicationApprovalService
    {
        bool IsEligibleForRegister(string gatewayAssessmentStatus, string financialReviewStatus, IEnumerable<RoatpApplySequence> applySequences);

        Task<RoatpApplicationApprovalViewModel> BuildApplicationApprovalViewModel(Guid applicationId);

        bool SubmitOrganisationToRoatpRegister(RoatpApplicationApprovalViewModel roatpApplicationModel);
    }
}
