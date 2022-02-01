using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    public class CertificateDateController : CertificateBaseController
    {
        private readonly CertificateDateViewModelValidator _validator;

        public CertificateDateController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            IApiClient apiClient,
            CertificateDateViewModelValidator validator) : base(logger, contextAccessor, apiClient)
        {
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> Date(Guid certificateId)
        {
            var actionResult = await LoadViewModel<CertificateDateViewModel>(certificateId, "~/Views/CertificateAmend/Date.cshtml");
            return actionResult;
        }

        [HttpPost(Name = "Date")]
        public async Task<IActionResult> Date(CertificateDateViewModel vm)
        {
            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Date.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.Date);

            return actionResult;
        }
    }
}