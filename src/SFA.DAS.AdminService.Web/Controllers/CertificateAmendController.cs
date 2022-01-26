using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize]
    public class CertificateAmendController : CertificateBaseController
    {
        public CertificateAmendController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Check(Guid certificateId, string searchString, int page)
        {
            var (actionResult, _) = await GetCheckViewModel(certificateId, searchString, page);
            return actionResult;
        }

        [HttpPost(Name = "Check")]
        public async Task<IActionResult> ConfirmAndSubmit(CertificateCheckViewModel vm)
        {
            var (actionResult, model) = await GetCheckViewModel(vm.Id, vm.SearchString, vm.Page);
            var options = await ApiClient.GetStandardOptions(vm.GetStandardId());
            var isDueCertificate = vm.SelectedGrade != null & vm.SelectedGrade != CertificateGrade.Fail;
            var isMissingOptions = options != null && options.HasOptions() && string.IsNullOrWhiteSpace(model.Option);
            var isMissingRecipient = string.IsNullOrWhiteSpace(model.AddressLine1) || string.IsNullOrWhiteSpace(model.Name);

            if ((isDueCertificate & isMissingRecipient) || isMissingOptions)
            {
                if (isMissingOptions)
                {
                    ModelState.AddModelError("Option", "Add an option");
                }
                if (isDueCertificate & string.IsNullOrWhiteSpace(model.Name))
                {
                    ModelState.AddModelError("Name", "You need to give a name of who will receive the certificate");
                }
                if (isDueCertificate & string.IsNullOrWhiteSpace(model.AddressLine1))
                {
                    ModelState.AddModelError("AddressLine1", "You need to give an address of where we will send the certificate");
                }
                return actionResult;
            }

            if (vm.CanRequestDuplicate)
            {
                return RedirectToAction("ConfirmReprint", "DuplicateRequest",
                    new
                    {
                        certificateId = vm.Id,
                        redirectToCheck = vm.RedirectToCheck,
                        Uln = vm.Uln,
                        StdCode = vm.StandardCode,
                        Page = vm.Page,
                        SearchString = vm.SearchString
                    });
            }

            return RedirectToAction("Index", "Comment",
                new
                {
                    certificateId = vm.Id,
                    redirectToCheck = vm.RedirectToCheck,
                    Uln = vm.Uln,
                    StdCode = vm.StandardCode,
                    Page = vm.Page,
                    SearchString = vm.SearchString
                });
        }

        private async Task<(IActionResult, CertificateCheckViewModel)> GetCheckViewModel(Guid certificateId, string searchString, int page)
        {
            var actionResult = await LoadViewModel<CertificateCheckViewModel>(certificateId, "~/Views/CertificateAmend/Check.cshtml");
            var viewResult = (actionResult as ViewResult);
            var certificateCheckViewModel = viewResult.Model as CertificateCheckViewModel;

            certificateCheckViewModel.Page = page;
            certificateCheckViewModel.SearchString = searchString;
            
            var standards = await ApiClient.GetStandardVersions(certificateCheckViewModel.StandardCode);
            certificateCheckViewModel.StandardHasMultipleVersions = standards.Count() > 1;

            var options = await ApiClient.GetStandardOptions(certificateCheckViewModel.GetStandardId());
            TempData["ShowOptionsChangeLink"] = options != null && options.HasMoreThanOneOption();

            return (actionResult, certificateCheckViewModel);
        }
    }
}