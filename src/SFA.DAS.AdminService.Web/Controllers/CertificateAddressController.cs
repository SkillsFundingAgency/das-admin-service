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
    public class CertificateAddressController : CertificateBaseController
    {
        public CertificateAddressController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClientFactory<ApiClient> apiClient)
            : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> Address(Guid certificateId, bool fromApproval)
        {
            var viewModel = await LoadViewModel<CertificateAddressViewModel>(certificateId, "~/Views/CertificateAmend/Address.cshtml");
            if (viewModel is ViewResult viewResult && viewResult.Model is CertificateAddressViewModel certificateAddressViewModel)
                certificateAddressViewModel.FromApproval = fromApproval;

            return viewModel;
        }

        [HttpPost(Name = "Grade")]
        public async Task<IActionResult> Address(CertificateAddressViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Address.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id , fromapproval = vm.FromApproval }), action: CertificateActions.Address);
        }
    }
}