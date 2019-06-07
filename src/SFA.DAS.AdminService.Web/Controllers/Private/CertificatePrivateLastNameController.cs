using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/lastname")]
    public class CertificatePrivateLastNameController : CertificateBaseController
    {
        public CertificatePrivateLastNameController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> LastName(Guid certificateId, bool fromApproval)
        {
            var viewModel = await LoadViewModel<CertificateLastNameViewModel>(certificateId, "~/Views/CertificateAmend/LastName.cshtml");
            if (viewModel is ViewResult viewResult && viewResult.Model is CertificateLastNameViewModel certificateLastNameViewModel)
                certificateLastNameViewModel.FromApproval = fromApproval;

            return viewModel;
        }

        [HttpPost(Name = "LastName")]
        public async Task<IActionResult> LastName(CertificateLastNameViewModel vm)
        {
            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/LastName.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id, fromapproval = vm.FromApproval }), action: CertificateActions.LastName);

            return actionResult;
        }
    }
}