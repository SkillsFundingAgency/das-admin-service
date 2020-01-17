using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels;
using SFA.DAS.AssessorService.Application.Api.Client;

namespace SFA.DAS.AdminService.Web.Controllers
{
    public class CertificateRecipientController : CertificateBaseController
    {
        public CertificateRecipientController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClientFactory<ApiClient> apiClient)
            : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> Recipient(Guid certificateId, bool fromApproval)
        {
            var viewModel = await LoadViewModel<CertificateRecipientViewModel>(certificateId, "~/Views/CertificateAmend/Recipient.cshtml");
            if (viewModel is ViewResult viewResult && viewResult.Model is CertificateRecipientViewModel certificateRecipientViewModel)
                certificateRecipientViewModel.FromApproval = fromApproval;

            return viewModel;
        }

        [HttpPost(Name = "Recipient")]
        public async Task<IActionResult> Recipient(CertificateRecipientViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Recipient.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id , fromapproval = vm.FromApproval }), action: CertificateActions.Recipient);
        }
    }
}