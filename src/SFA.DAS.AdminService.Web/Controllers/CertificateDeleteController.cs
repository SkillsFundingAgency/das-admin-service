using System;
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
        public async Task<IActionResult> AuditDetails(Guid certificateId, string reasonForChange, string incidentNumber)
        {
            var viewModel =
                await LoadViewModel<CertificateAuditDetailsViewModel>(certificateId,
                    "~/Views/CertificateDelete/AuditDetails.cshtml");
            var viewResult = (viewModel as ViewResult);

            var certificateAuditDetailsViewModel = viewResult.Model as CertificateAuditDetailsViewModel;

            certificateAuditDetailsViewModel.ReasonForChange = reasonForChange;
            certificateAuditDetailsViewModel.IncidentNumber = incidentNumber;

            return View(certificateAuditDetailsViewModel);
        }

        [HttpPost(Name = "AuditDetails")]
        public async Task<IActionResult> AuditDetails(CertificateAuditDetailsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("ConfirmDelete", "CertificateDelete", new
                {
                    certificateId = vm.Id,
                    reasonForDeletion = vm.ReasonForChange,
                    incidentNumber = vm.IncidentNumber
                });
            }
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(Guid certificateId, string reasonForChange, string incidentNumber)
        {
            var viewModel =
                await LoadViewModel<CertificateConfirmDeleteViewModel>(certificateId,
                    "~/Views/CertificateDelete/ConfirmDelete.cshtml");
            var viewResult = (viewModel as ViewResult);
            var username = ContextAccessor.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
            var certificateConfirmDeleteViewModel = viewResult.Model as CertificateConfirmDeleteViewModel;

            certificateConfirmDeleteViewModel.ReasonForChange = reasonForChange;
            certificateConfirmDeleteViewModel.IncidentNumber = incidentNumber;
            certificateConfirmDeleteViewModel.UserName = username;

            return View(certificateConfirmDeleteViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(CertificateConfirmDeleteViewModel deleteViewModel)
        {
            try
            {
                var request = new DeleteCertificateRequest
                {
                    ReasonForChange = deleteViewModel.ReasonForChange,
                    IncidentNumber = deleteViewModel.IncidentNumber,
                    StandardCode = deleteViewModel.StandardCode,
                    Uln = deleteViewModel.Uln,
                    Username = deleteViewModel.UserName
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