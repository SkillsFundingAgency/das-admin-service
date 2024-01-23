using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    public class CertificateVersionController : CertificateBaseController
    {
        public CertificateVersionController(
            ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ICertificateApiClient certificateApiClient,
            ILearnerDetailsApiClient learnerDetailsApiClient,
            IOrganisationsApiClient organisationsApiClient,
            IScheduleApiClient scheduleApiClient,
            IStandardVersionApiClient standardVersionApiClient) : base(logger, contextAccessor, certificateApiClient, learnerDetailsApiClient, organisationsApiClient, scheduleApiClient, standardVersionApiClient)
        { 
        }

        [HttpGet]
        public async Task<IActionResult> Version(Guid certificateId)
        {
            var actionResult = await LoadViewModel<CertificateVersionViewModel>(certificateId, "~/Views/CertificateAmend/Version.cshtml");
            if (actionResult is ViewResult viewResult && viewResult.Model is CertificateVersionViewModel certificateVersionViewModel)
            {
                certificateVersionViewModel.Standards = await StandardVersionApiClient.GetStandardVersionsByLarsCode(certificateVersionViewModel.StandardCode);
            }

            return actionResult;
        }

        [HttpPost(Name = "Version")]
        public async Task<IActionResult> Version(CertificateVersionViewModel vm)
        {
            // After selecting version, retrieve individual standard for version number
            vm.Version = (await StandardVersionApiClient.GetStandardVersionById(vm.StandardUId))?.Version;
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Version.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.Version);
        }
    }
}
