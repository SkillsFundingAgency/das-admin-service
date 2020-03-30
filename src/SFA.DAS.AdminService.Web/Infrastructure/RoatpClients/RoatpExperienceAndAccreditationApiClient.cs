using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;

namespace SFA.DAS.AdminService.Web.Infrastructure.RoatpClients
{
    public class RoatpExperienceAndAccreditationApiClient : RoatpApiClientBase<RoatpExperienceAndAccreditationApiClient>, IRoatpExperienceAndAccreditationApiClient
    {
        public RoatpExperienceAndAccreditationApiClient(string baseUri, ILogger<RoatpExperienceAndAccreditationApiClient> logger, IRoatpTokenService tokenService) : base(baseUri, logger, tokenService)
        {
        }

        public async Task<SubcontractorDeclaration> GetSubcontractorDeclaration(Guid applicationId)
        {
            return await Get<SubcontractorDeclaration>($"/Accreditation/{applicationId}/SubcontractDeclaration");
        }

        public async Task<FileStreamResult> GetSubcontractorDeclarationContractFile(Guid applicationId)
        {
            var response = await GetResponse($"/Accreditation/{applicationId}/SubcontractDeclaration/ContractFile");

            var fileStream = await response.Content.ReadAsStreamAsync();
            var result = new FileStreamResult(fileStream, response.Content.Headers.ContentType.MediaType);
            result.FileDownloadName = response.Content.Headers.ContentDisposition.FileName;
            return result;
        }

        public async Task<string> GetOfficeForStudents(Guid applicationId)
        {
            return await Get($"/Accreditation/{applicationId}/OfficeForStudents");
        }

        public async Task<string> GetInitialTeacherTraining(Guid applicationId)
        {
            return await Get($"/Accreditation/{applicationId}/InitialTeacherTraining");
        }
    }
}
