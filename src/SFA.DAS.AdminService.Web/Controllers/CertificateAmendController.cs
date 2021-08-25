using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels;
using SFA.DAS.AdminService.Web.ViewModels.Private;
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
        public async Task<IActionResult> Check(Guid certificateId, string searchString, int page, bool fromApproval)
        {
            var (actionResult,_) = await GetCheckViewModel(certificateId, searchString, page, fromApproval);
            return actionResult;
        }

        [HttpPost(Name = "Check")]
        public async Task<IActionResult> ConfirmAndSubmit(CertificateCheckViewModel vm)
        {
            var (actionResult, model) = await GetCheckViewModel(vm.Id, vm.SearchString, vm.Page, vm.FromApproval);
            var options = await ApiClient.GetStandardOptions(vm.GetStandardId());
            var isIncompleteAddressee = string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.AddressLine1);
            var isMissingOptions = options != null && options.HasOptions() && string.IsNullOrWhiteSpace(model.Option);

            if (isIncompleteAddressee || isMissingOptions)
            {
                if (isMissingOptions)
                {
                    ModelState.AddModelError("Option", "Add an option");
                }
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    ModelState.AddModelError("Name", "You need to give a name of who will receive the certificate"); 
                }
                if (string.IsNullOrWhiteSpace(model.AddressLine1))
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

            if (vm.Status == CertificateStatus.Draft &&
                vm.PrivatelyFundedStatus == CertificateStatus.Rejected && vm.FromApproval)
            {
                var certificate = await ApiClient.GetCertificate(vm.Id);
                var approvalResults = new ApprovalResult[1];
                approvalResults[0] = new ApprovalResult
                {
                    IsApproved = CertificateStatus.Submitted,
                    CertificateReference = certificate.CertificateReference,
                    PrivatelyFundedStatus = CertificateStatus.Approved
                };

                await ApiClient.ApproveCertificates(new CertificatePostApprovalViewModel
                {
                    UserName = ContextAccessor.HttpContext.User
                    .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value,
                    ApprovalResults = approvalResults
                });
                return RedirectToAction("Approved", "CertificateApprovals");
            }
            else
            {
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
        }

        private async Task<(IActionResult, CertificateCheckViewModel)> GetCheckViewModel(Guid certificateId, string searchString, int page, bool fromApproval)
        {
            var actionResult = await LoadViewModel<CertificateCheckViewModel>(certificateId, "~/Views/CertificateAmend/Check.cshtml");
            var viewResult = (actionResult as ViewResult);
            var certificateCheckViewModel = viewResult.Model as CertificateCheckViewModel;

            certificateCheckViewModel.Page = page;
            certificateCheckViewModel.SearchString = searchString;
            certificateCheckViewModel.FromApproval = fromApproval;

            var standards = await ApiClient.GetStandardVersions(certificateCheckViewModel.StandardCode);
            certificateCheckViewModel.StandardHasMultipleVersions = standards.Count() > 1;

            var options = await ApiClient.GetStandardOptions(certificateCheckViewModel.GetStandardId());
            TempData["ShowOptionsChangeLink"] = options != null && options.HasMoreThanOneOption();

            return (actionResult, certificateCheckViewModel);
        }
    }
}