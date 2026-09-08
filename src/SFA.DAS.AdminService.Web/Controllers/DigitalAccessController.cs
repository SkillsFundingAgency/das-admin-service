using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AdminService.Web.Orchestrators;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Web.ViewModels.DigitalAccess;
using SFA.DAS.AdminService.Common.Models;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Extensions;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
    public class DigitalAccessController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDigitalAccessOrchestrator _orchestrator;

        public const string UserNotFoundRouteGet = nameof(UserNotFoundRouteGet);
        public const string UserNotMatchedRouteGet = nameof(UserNotMatchedRouteGet);
        public const string DigitalAccessReferenceSearchRouteGet = nameof(DigitalAccessReferenceSearchRouteGet);
        public const string DigitalAccessReferenceSearchRoutePost = nameof(DigitalAccessReferenceSearchRoutePost);
        public const string RestoreAccessRouteGet = nameof(RestoreAccessRouteGet);
        public const string CertificateChangeRequestRouteGet = nameof(CertificateChangeRequestRouteGet);
        public const string NonSpecificContactRequestRouteGet = nameof(NonSpecificContactRequestRouteGet);
        public const string CertificatePrintRequestRouteGet = nameof(CertificatePrintRequestRouteGet);
        public const string RestoreAccessRoutePost = nameof(RestoreAccessRoutePost);

        public DigitalAccessController(IHttpContextAccessor contextAccessor, IDigitalAccessOrchestrator orchestrator)
        {
            _contextAccessor = contextAccessor;
            _orchestrator = orchestrator;
        }

        [HttpGet("digital-access/reference", Name = DigitalAccessReferenceSearchRouteGet)]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public IActionResult DigitalAccessReferenceSearch()
        {
            return View(new DigitalAccessReferenceSearchViewModel());
        }

        [HttpPost("digital-access/reference", Name = DigitalAccessReferenceSearchRoutePost)]
        [ModelStatePersist(ModelStatePersist.Store)]
        public async Task<IActionResult> DigitalAccessReferenceSearch(DigitalAccessReferenceSearchViewModel vm, int page = 1)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
            }

            var username = _contextAccessor.HttpContext.User.UserDisplayName();

            var resultVm = await _orchestrator.GetDigitalAccessReferenceViewModel(vm.ReferenceNumber, username);

            if (resultVm == null)
            {
                ModelState.AddModelError("ReferenceNumber", "No records found with this reference");
                return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
            }

            switch (resultVm.ActionType)
            {
                case ActionType.Reprint:
                    return RedirectToRoute(CertificatePrintRequestRouteGet, new { referenceNumber = resultVm.ReferenceNumber });
                case ActionType.Help:
                    return RedirectToRoute(CertificateChangeRequestRouteGet, new { referenceNumber = resultVm.ReferenceNumber });
                case ActionType.Contact:
                    return RedirectToRoute(NonSpecificContactRequestRouteGet, new { referenceNumber = resultVm.ReferenceNumber });
                case ActionType.NotFound:
                    return RedirectToRoute(UserNotFoundRouteGet, new { referenceNumber = resultVm.ReferenceNumber });
                case ActionType.NotMatched:
                    return RedirectToRoute(UserNotMatchedRouteGet, new { referenceNumber = resultVm.ReferenceNumber });
            }

            return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
        }

        [HttpGet("digital-access/reference/{referenceNumber}/not-found", Name = UserNotFoundRouteGet)]
        public async Task<IActionResult> UserNotFound(string referenceNumber)
        {
            var vm = await _orchestrator.GetUserNotFoundViewModel(referenceNumber);
            if (vm == null)
            {
                return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
            }

            return View(vm);
        }

        [HttpGet("digital-access/reference/{referenceNumber}/not-matched", Name = UserNotMatchedRouteGet)]
        public async Task<IActionResult> UserNotMatched(string referenceNumber)
        {
            var vm = await _orchestrator.GetUserNotMatchedViewModel(referenceNumber);
            if (vm == null)
            {
                return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
            }

            return View(vm);
        }
        
        [HttpGet("digital-access/reference/{referenceNumber}/restore", Name = RestoreAccessRouteGet)]
        public async Task<IActionResult> RestoreAccess(string referenceNumber)
        {
            if (string.IsNullOrWhiteSpace(referenceNumber))
            {
                return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
            }

            var vm = await _orchestrator.GetRestoreAccessViewModel(referenceNumber);
            if (vm == null)
            {
                return RedirectToRoute(UserNotMatchedRouteGet, new { referenceNumber });
            }

            return View(vm);
        }

        [HttpGet("digital-access/reference/{referenceNumber}/certificate-change-request", Name = CertificateChangeRequestRouteGet)]
        public async Task<IActionResult> CertificateChangeRequest(string referenceNumber)
        {
            var vm = await _orchestrator.GetCertificateChangeRequestViewModel(referenceNumber);
            if (vm == null)
            {
                return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
            }

            vm.ReturnUrl = Url.RouteUrl(CertificateChangeRequestRouteGet, new { referenceNumber = referenceNumber });

            return View(vm);
        }

        [HttpGet("digital-access/reference/{referenceNumber}/non-specific-contact-request", Name = NonSpecificContactRequestRouteGet)]
        public async Task<IActionResult> NonSpecificContactRequest(string referenceNumber)
        {
            var vm = await _orchestrator.GetNonSpecificContactRequestViewModel(referenceNumber);
            if (vm == null)
            {
                return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
            }

            return View(vm);
        }

        [HttpGet("digital-access/reference/{referenceNumber}/certificate-print-request", Name = CertificatePrintRequestRouteGet)]
        public async Task<IActionResult> CertificatePrintRequest(string referenceNumber)
        {
            var vm = await _orchestrator.GetCertificatePrintRequestViewModel(referenceNumber);
            if (vm == null)
            {
                return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
            }

            vm.ReturnUrl = Url.RouteUrl(CertificatePrintRequestRouteGet, new { referenceNumber = referenceNumber });

            return View(vm);
        }

        [HttpPost("digital-access/reference/{referenceNumber}/restore", Name = RestoreAccessRoutePost)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreAccessPost(string referenceNumber)
        {
            var username = _contextAccessor.HttpContext.User.UserDisplayName();
            if (string.IsNullOrWhiteSpace(referenceNumber))
            {
                return RedirectToRoute(DigitalAccessReferenceSearchRouteGet);
            }

            var restoreVm = await _orchestrator.GetRestoreAccessViewModel(referenceNumber);
            if (restoreVm == null)
            {
                return RedirectToRoute(UserNotMatchedRouteGet, new { referenceNumber });
            }

            await _orchestrator.UnlockUser(restoreVm.UserId, username, restoreVm.UserActionId);

            if (TempData != null)
            {
                TempData.AddFlashMessage("User access restored", "Tell the user to log in to Apprenticeship certificates.", TempDataDictionaryExtensions.FlashMessageLevel.Success);
            }

            return RedirectToRoute(UserNotMatchedRouteGet, new { referenceNumber });
        }
    }
}
