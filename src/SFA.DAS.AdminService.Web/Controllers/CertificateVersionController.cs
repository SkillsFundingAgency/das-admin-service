using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels;

namespace SFA.DAS.AdminService.Web.Controllers
{
    public class CertificateVersionController : CertificateBaseController
    {
        public CertificateVersionController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            IApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> Version(Guid certificateId)
        {
            var viewModel = await LoadViewModel<CertificateVersionViewModel>(certificateId, "~/Views/CertificateAmend/Version.cshtml");
            if (viewModel is ViewResult viewResult && viewResult.Model is CertificateVersionViewModel certificateVersionViewModel)
            {
                certificateVersionViewModel.Standards = await ApiClient.GetStandardVersions(certificateVersionViewModel.StandardCode);
            }

            return viewModel;
        }

        [HttpPost(Name = "Version")]
        public async Task<IActionResult> Version(CertificateVersionViewModel vm)
        {
            // After selecting version, retrieve individual standard for version number
            vm.Version = (await ApiClient.GetStandardVersion(vm.StandardUId))?.Version;
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Version.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.Version);
        }
    }
}
