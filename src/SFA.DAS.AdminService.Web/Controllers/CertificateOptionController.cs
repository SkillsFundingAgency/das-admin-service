using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Controllers
{
    public class CertificateOptionController : CertificateBaseController
    {
        public CertificateOptionController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient)
            : base(logger, contextAccessor, apiClient)
        { }


        [HttpGet]
        public async Task<IActionResult> Option(Guid certificateId)
        {
            var viewModel = await LoadViewModel<CertificateOptionViewModel>(certificateId, "~/Views/CertificateAmend/Option.cshtml");
            if (viewModel is ViewResult viewResult && viewResult.Model is CertificateOptionViewModel certificateOptionViewModel)
            {
                var standardOption = await ApiClient.GetStandardOptions(certificateOptionViewModel.GetStandardId());

                certificateOptionViewModel.Options = standardOption != null ? standardOption.CourseOption : new List<string>();
                certificateOptionViewModel.SelectedOption = certificateOptionViewModel.Option;
            }

            return viewModel;
        }

        [HttpPost(Name = "Option")]
        public async Task<IActionResult> Option(CertificateOptionViewModel vm)
        {
            return await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/Option.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.Option);
        }
    }
}