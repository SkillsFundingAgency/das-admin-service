using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public interface IGatewayExperienceAndAccreditationOrchestrator
    {
        Task<SubcontractorDeclarationViewModel> GetSubcontractorDeclarationViewModel(GetSubcontractorDeclarationRequest subcontractorDeclarationRequest);
        Task<FileStreamResult> GetSubcontractorDeclarationContractFile(GetSubcontractorDeclarationContractFileRequest subcontractorDeclarationRequest);
        Task<OfficeForStudentsViewModel> GetOfficeForStudentsViewModel(GetOfficeForStudentsRequest request);
        Task<OfstedDetailsViewModel> GetOfstedDetailsViewModel(GetOfstedDetailsRequest request);
    }
}
