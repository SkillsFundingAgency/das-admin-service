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
    public class CertificateGradeController : CertificateBaseController
    {
        public CertificateGradeController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        public async Task<IActionResult> Grade(Guid certificateId, bool fromApproval)
        {
            var viewModel = await LoadViewModel<CertificateGradeViewModel>(certificateId, "~/Views/CertificateAmend/Grade.cshtml");
            if (viewModel is ViewResult viewResult && viewResult.Model is CertificateGradeViewModel certificateGradeViewModel)
                certificateGradeViewModel.FromApproval = fromApproval;

            return viewModel;
        }

        [HttpPost(Name = "Grade")]
        public async Task<IActionResult> Grade(CertificateGradeViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Grade.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id , fromapproval = vm.FromApproval }), action: CertificateActions.Grade);
        }
    }
}