using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.CertificateDelete;

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
        public async Task<IActionResult> ConfirmAndSubmit(Guid certificateId, string searchString, int page)
        {
            var viewModel =
                await LoadViewModel<CertificateSubmitDeleteViewModel>(certificateId,
                    "~/Views/CertificateDelete/ConfirmAndSubmit.cshtml");
            var viewResult = (viewModel as ViewResult);
            var certificateDeleteViewModel = viewResult.Model as CertificateSubmitDeleteViewModel;

            certificateDeleteViewModel.Page = page;
            certificateDeleteViewModel.SearchString = searchString;
            //certificateDeleteViewModel.FromApproval = fromApproval;

            //var options = await ApiClient.GetOptions(certificateDeleteViewModel.StandardCode);
            //TempData["HideOption"] = !options.Any();

            return View(certificateDeleteViewModel);
        }


        [HttpPost(Name = "ConfirmAndSubmit")]
        public IActionResult ConfirmAndSubmit(CertificateSubmitDeleteViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.IsDeleteConfirmed != null && vm.IsDeleteConfirmed == true)
                {
                    return RedirectToAction("AuditDetails", "CertificateDelete", new
                    {
                        //searchString = vm.SearchString,
                        //page = vm.Page,
                        certificateId = vm.Id
                    });
                }
                if (vm.IsDeleteConfirmed != null && vm.IsDeleteConfirmed == false)
                {
                    return RedirectToAction("Select", "Search", new
                    {
                        stdCode = vm.StandardCode,
                        uln = vm.Uln,
                        searchString = vm.SearchString,
                        page = vm.Page
                    });
                }
            }
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AuditDetails(Guid certificateId)
        {
            var viewModel =
                await LoadViewModel<CertificateAuditDetailsViewModel>(certificateId,
                    "~/Views/CertificateDelete/AuditDetails.cshtml");
            var viewResult = (viewModel as ViewResult);

            var certificateAuditDetailsViewModel = viewResult.Model as CertificateAuditDetailsViewModel;

            //certificateAuditDetailsViewModel.Page = page;
            //certificateAuditDetailsViewModel.SearchString = searchString;
            //certificateAuditDetailsViewModel.FromApproval = fromApproval;

            //var options = await ApiClient.GetOptions(certificateAuditDetailsViewModel.StandardCode);
            //TempData["HideOption"] = !options.Any();

            return View(certificateAuditDetailsViewModel);
        }


        [HttpPost(Name = "AuditDetails")]
        public IActionResult AuditDetails(CertificateAuditDetailsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("ConfirmDelete", "CertificateDelete", new
                {
                    certificateId = vm.Id,
                    reasonForDeletion = vm.ReasonForDeletion,
                    incidentNumber = vm.IncidentNumber
                    //uln = vm.Uln,
                    //searchString = vm.SearchString,
                    //page = vm.Page
                });
            }
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(Guid certificateId, string reasonForDeletion, string incidentNumber)
        {
            var viewModel =
                await LoadViewModel<CertificateConfirmDeleteViewModel>(certificateId,
                    "~/Views/CertificateDelete/ConfirmDelete.cshtml");
            var viewResult = (viewModel as ViewResult);
            var username = ContextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var certificateConfirmDeleteViewModel = viewResult.Model as CertificateConfirmDeleteViewModel;

            certificateConfirmDeleteViewModel.ReasonForDeletion = reasonForDeletion;
            certificateConfirmDeleteViewModel.IncidentNumber = incidentNumber;
            certificateConfirmDeleteViewModel.UserName = username;

            //var options = await ApiClient.GetOptions(certificateConfirmDeleteViewModel.StandardCode);
            //TempData["HideOption"] = !options.Any();

            return View(certificateConfirmDeleteViewModel);
        }
    }
}