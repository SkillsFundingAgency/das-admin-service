using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels;
using SFA.DAS.AdminService.Web.ViewModels.CertificateDelete;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize]
    public class CertificateDeleteController : CertificateBaseController
    {
        public CertificateDeleteController(ILogger<CertificateDeleteController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient) : base(logger, contextAccessor, apiClient)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid certificateId, string searchString, int page, bool fromApproval)
        {
            var viewModel =
                await LoadViewModel<CertificateDeleteViewModel>(certificateId,
                    "~/Views/CertificateDelete/Delete.cshtml");
            var viewResult = (viewModel as ViewResult);
            var certificateDeleteViewModel = viewResult.Model as CertificateDeleteViewModel;

            certificateDeleteViewModel.Page = page;
            certificateDeleteViewModel.SearchString = searchString;
            certificateDeleteViewModel.FromApproval = fromApproval;

            var options = await ApiClient.GetOptions(certificateDeleteViewModel.StandardCode);
            TempData["HideOption"] = !options.Any();

            return viewModel;
        }


        [HttpPost(Name = "ConfirmAndSubmit")]
        public async Task<IActionResult> ConfirmAndSubmit(CertificateDeleteViewModel vm)
        {
            return View(vm);
            //if (vm.Status == CertificateStatus.Printed ||
            //    vm.Status == CertificateStatus.Reprint)
            //{
            //    return RedirectToAction("Index", "DuplicateRequest",
            //        new
            //        {
            //            certificateId = vm.Id,
            //            redirectToCheck = vm.RedirectToCheck,
            //            Uln = vm.Uln,
            //            StdCode = vm.StandardCode,
            //            Page = vm.Page,
            //            SearchString = vm.SearchString
            //        });
            //}

            //if (vm.Status == CertificateStatus.Draft &
            //    vm.PrivatelyFundedStatus == CertificateStatus.Rejected & vm.FromApproval)
            //{
            //    var certificate = await ApiClient.GetCertificate(vm.Id);
            //    var approvalResults = new ApprovalResult[1];
            //    approvalResults[0] = new ApprovalResult
            //    {
            //        IsApproved = CertificateStatus.Submitted,
            //        CertificateReference = certificate.CertificateReference,
            //        PrivatelyFundedStatus = CertificateStatus.Approved
            //    };

            //    await ApiClient.ApproveCertificates(new CertificatePostApprovalViewModel
            //    {
            //        UserName = ContextAccessor.HttpContext.User
            //            .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value,
            //        ApprovalResults = approvalResults
            //    });
            //    return RedirectToAction("Approved", "CertificateApprovals");
            //}

            //return RedirectToAction("Index", "Comment",
            //    new
            //    {
            //        certificateId = vm.Id,
            //        redirectToCheck = vm.RedirectToCheck,
            //        Uln = vm.Uln,
            //        StdCode = vm.StandardCode,
            //        Page = vm.Page,
            //        SearchString = vm.SearchString
            //    });
        }
    }
}