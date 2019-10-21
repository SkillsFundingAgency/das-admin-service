using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Controllers.RoatpAssessor
{
    [Authorize(Roles = Domain.Roles.RoatpAssessorTeam)]
    [Route("roatp-review/{applicationId:Guid}/download")]
    public class RoatpAssessorDownloadFileController : Controller
    {
        private readonly IQnaApiClient _qnaApiClient;

        public RoatpAssessorDownloadFileController(IQnaApiClient qnaApiClient)
        {
            _qnaApiClient = qnaApiClient;
        }

        [HttpGet("sequence/{sequenceNo}/section/{sectionNo}/page/{pageId}/question/{questionId}/{filename}/download")]
        public async Task<IActionResult> DownloadFile(Guid applicationId, int sequenceNo, int sectionNo, string pageId, string questionId, string filename)
        {
            var section = await _qnaApiClient.GetSectionBySectionNo(applicationId, sequenceNo, sectionNo);

            var response = await _qnaApiClient.DownloadFile(applicationId, section.Id, pageId, questionId, filename);
            var fileStream = await response.Content.ReadAsStreamAsync();

            return File(fileStream, response.Content.Headers.ContentType.MediaType, filename);
        }
    }
}