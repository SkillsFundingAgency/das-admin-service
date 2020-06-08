using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.CertificateDelete;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize]
    public class CertificateDeleteController : CertificateBaseController
    {
        private readonly ILogger<CertificateDeleteController> _logger;
        private ICertificateApiClient _certificateApiClient;
        public CertificateDeleteController(ILogger<CertificateDeleteController> logger,
            IHttpContextAccessor contextAccessor,
            ApiClient apiClient, ICertificateApiClient certificateApiClient) : base(logger, contextAccessor, apiClient)
        {
            _logger = logger;
            _certificateApiClient = certificateApiClient;
        }

        [HttpGet("confirm-delete-certificate", Name = "ConfirmAndSubmit")]
        public async Task<IActionResult> ConfirmAndSubmit(CertificateDeleteViewModel vm)
        {
            var viewModel =
                await LoadViewModel<CertificateSubmitDeleteViewModel>(vm.CertificateId,
                    "~/Views/CertificateDelete/ConfirmAndSubmit.cshtml");
            var viewResult = (viewModel as ViewResult);
            var certificateDeleteViewModel = viewResult.Model as CertificateSubmitDeleteViewModel;

            certificateDeleteViewModel.Page = vm.Page;
            certificateDeleteViewModel.SearchString = vm.SearchString;
            certificateDeleteViewModel.IsDeleteConfirmed = vm.IsDeleteConfirmed;
            certificateDeleteViewModel.IncidentNumber = vm.IncidentNumber;
            certificateDeleteViewModel.ReasonForChange = vm.ReasonForChange;

            return View(certificateDeleteViewModel);
        }


        [HttpPost("confirm-delete-certificate", Name = "ConfirmAndSubmit")]
        public IActionResult ConfirmAndSubmit(CertificateSubmitDeleteViewModel vm)
        {
            var errorMessages = new Dictionary<string, string>();

            if (!vm.IsDeleteConfirmed.HasValue)
            {
                errorMessages["IsDeleteConfirmed"] = "Select yes if you want to delete this certificate";
            }

            if (errorMessages.Any())
            {
                foreach (var error in errorMessages)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
            }

            if (ModelState.IsValid)
            {
                if (vm.IsDeleteConfirmed != null && vm.IsDeleteConfirmed == true)
                {
                    return RedirectToAction("AuditDetails", "CertificateDelete", new
                    {
                        certificateId = vm.Id,
                        isDeleteConfirmed = vm.IsDeleteConfirmed,
                        incidentNumber = vm.IncidentNumber,
                        reasonForChange = vm.ReasonForChange
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

        [HttpGet("audit-details", Name = "AuditDetails")]
        public async Task<IActionResult> AuditDetails(CertificateDeleteViewModel vm)
        {
            var viewModel =
                await LoadViewModel<CertificateAuditDetailsViewModel>(vm.CertificateId,
                    "~/Views/CertificateDelete/AuditDetails.cshtml");
            var viewResult = (viewModel as ViewResult);

            var certificateAuditDetailsViewModel = viewResult.Model as CertificateAuditDetailsViewModel;

            certificateAuditDetailsViewModel.ReasonForChange = vm.ReasonForChange;
            certificateAuditDetailsViewModel.IncidentNumber = vm.IncidentNumber;
            certificateAuditDetailsViewModel.IsDeleteConfirmed = vm.IsDeleteConfirmed;

            return View(certificateAuditDetailsViewModel);
        }

        [HttpPost("audit-details", Name = "AuditDetails")]
        public async Task<IActionResult> AuditDetails(CertificateAuditDetailsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("ConfirmDelete", "CertificateDelete", new
                {
                    certificateId = vm.Id,
                    reasonForChange = vm.ReasonForChange,
                    incidentNumber = vm.IncidentNumber,
                    isDeleteConfirmed = vm.IsDeleteConfirmed
                });
            }
            return View(vm);
        }

        [HttpGet("check-your-answers", Name = "ConfirmDelete")]
        public async Task<IActionResult> ConfirmDelete(CertificateDeleteViewModel vm)
        {
            var viewModel =
                await LoadViewModel<CertificateConfirmDeleteViewModel>(vm.CertificateId,
                    "~/Views/CertificateDelete/ConfirmDelete.cshtml");
            var viewResult = (viewModel as ViewResult);
            var username = ContextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var certificateConfirmDeleteViewModel = viewResult.Model as CertificateConfirmDeleteViewModel;

            certificateConfirmDeleteViewModel.ReasonForChange = vm.ReasonForChange;
            certificateConfirmDeleteViewModel.IncidentNumber = vm.IncidentNumber;
            certificateConfirmDeleteViewModel.Username = username;
            certificateConfirmDeleteViewModel.IsDeleteConfirmed = vm.IsDeleteConfirmed;
            return View(certificateConfirmDeleteViewModel);
        }

        [HttpPost("successfully-deleted-this-certificate", Name = "SuccessfulDelete")]
        public async Task<IActionResult> SuccessfulDelete(CertificateConfirmDeleteViewModel deleteViewModel)
        {
            try
            {
                var request = new DeleteCertificateRequest
                {
                    ReasonForChange = deleteViewModel.ReasonForChange,
                    IncidentNumber = deleteViewModel.IncidentNumber,
                    StandardCode = deleteViewModel.StandardCode,
                    Uln = deleteViewModel.Uln,
                    Username = deleteViewModel.Username
                };
                await _certificateApiClient.Delete(request);
                return View("SuccessfullDeletion", deleteViewModel);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Could not delete certificate with certificateId:{deleteViewModel.Id}." +
                                 $"Exception message:{exception.Message}");
                throw new HttpRequestException();
            }

        }
    }
}