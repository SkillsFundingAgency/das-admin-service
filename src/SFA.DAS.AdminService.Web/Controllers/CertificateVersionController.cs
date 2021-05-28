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
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> Version(Guid certificateId, bool fromApproval)
        {
            var viewModel = await LoadViewModel<CertificateVersionViewModel>(certificateId, "~/Views/CertificateAmend/Version.cshtml");
            if (viewModel is ViewResult viewResult && viewResult.Model is CertificateVersionViewModel certificateVersionViewModel)
            {
                certificateVersionViewModel.FromApproval = fromApproval;
                certificateVersionViewModel.Standards = await ApiClient.GetStandardVersions(certificateVersionViewModel.StandardCode);
            }

            return viewModel;
        }

        [HttpPost(Name = "Version")]
        public async Task<IActionResult> Version(CertificateVersionViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Version.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id, fromapproval = vm.FromApproval }), action: CertificateActions.Version);
        }
    }
}
