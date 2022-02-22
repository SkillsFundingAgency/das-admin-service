using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize]
    public class CertificateNamesController : CertificateBaseController
    {
        public CertificateNamesController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        { }

        [HttpGet]
        [Route("certificate/givennames")]
        public async Task<IActionResult> GivenNames(Guid certificateId)
        {
            var viewModel = await LoadViewModel<CertificateGivenNamesViewModel>(certificateId, "~/Views/CertificateAmend/GivenNames.cshtml");
            return viewModel;
        }

        [HttpPost]
        [Route("certificate/givennames")]
        public async Task<IActionResult> FirstName(CertificateGivenNamesViewModel vm)
        {
            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/GivenNames.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.GivenNames);

            return actionResult;
        }

        [HttpGet]
        [Route("certificate/familyname")]
        public async Task<IActionResult> FamilyName(Guid certificateId)
        {
            var viewModel = await LoadViewModel<CertificateFamilyNameViewModel>(certificateId, "~/Views/CertificateAmend/FamilyName.cshtml");
            return viewModel;
        }

        [HttpPost]
        [Route("certificate/familyname")]
        public async Task<IActionResult> FamilyName(CertificateFamilyNameViewModel vm)
        {
            var actionResult = await SaveViewModel(vm,
                returnToIfModelNotValid: "~/Views/CertificateAmend/FamilyName.cshtml",
                nextAction: RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id }), action: CertificateActions.GivenNames);

            return actionResult;
        }
    }
}
