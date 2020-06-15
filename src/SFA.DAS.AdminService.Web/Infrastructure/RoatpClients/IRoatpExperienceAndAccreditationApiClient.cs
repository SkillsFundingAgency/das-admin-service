using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;

namespace SFA.DAS.AdminService.Web.Infrastructure.RoatpClients
{
    public interface IRoatpExperienceAndAccreditationApiClient
    {
        Task<SubcontractorDeclaration> GetSubcontractorDeclaration(Guid applicationId);
        Task<FileStreamResult> GetSubcontractorDeclarationContractFile(Guid applicationId);
        Task<string> GetOfficeForStudents(Guid applicationId);
        Task<InitialTeacherTraining> GetInitialTeacherTraining(Guid applicationId);
        Task<OfstedDetails> GetOfstedDetails(Guid applicationId);
    }
}
