using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Private;
using SFA.DAS.AssessorService.Application.Api.Client;

namespace SFA.DAS.AdminService.Web.Controllers.Private
{
    [Authorize]
    [Route("certificate/firstname")]
    public class CertificatePrivateFirstNameController : CertificateBaseController
    {
        public CertificatePrivateFirstNameController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClientFactory<ApiClient> apiClient) : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> FirstName(Guid certificateId, bool fromApproval)
        {
            var viewModel = await LoadViewModel<CertificateFirstNameViewModel>(certificateId, "~/Views/CertificateAmend/FirstName.cshtml");
            if (viewModel is ViewResult viewResult && viewResult.Model is CertificateFirstNameViewModel certificateFirstNameViewModel)
                certificateFirstNameViewModel.FromApproval = fromApproval;

            return viewModel;
        }

        [HttpPost(Name = "FirstName")]
        public async Task<IActionResult> FirstName(CertificateFirstNameViewModel vm)
        {
            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/FirstName.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id , fromapproval = vm.FromApproval }), action: CertificateActions.FirstName);

            return actionResult;
        }
    }
}