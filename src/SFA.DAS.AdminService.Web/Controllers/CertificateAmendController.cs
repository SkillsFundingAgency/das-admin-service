using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Api.Types.Enums;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize]
    public class CertificateAmendController : CertificateBaseController
    {
        public CertificateAmendController(ILogger<CertificateAmendController> logger,
            IHttpContextAccessor contextAccessor,
            IApiClient apiClient) : base(logger, contextAccessor, apiClient)
        {
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public async Task<IActionResult> AmendReason(int stdCode, long uln)
        {
            var learner = await ApiClient.GetLearner(stdCode, uln, false);
            var model = new CertificateAmendReasonViewModel
            {
                Learner = learner,
                IncidentNumber = !ModelState.IsValid
                    ? ModelState[nameof(CertificateAmendReasonViewModel.IncidentNumber)]?.AttemptedValue
                    : string.Empty,
                Reasons = !ModelState.IsValid
                    ? new List<string>(ModelState[nameof(CertificateAmendReasonViewModel.Reasons)]?.AttemptedValue?.Split(",") ?? new string[] { })
                    : new List<string>(),
                OtherReason = !ModelState.IsValid
                    ? ModelState[nameof(CertificateAmendReasonViewModel.OtherReason)]?.AttemptedValue
                    : string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        public async Task<IActionResult> AmendReason(CertificateAmendReasonViewModel vm)
        {
            var username = ContextAccessor.HttpContext.User.UserId();

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(AmendReason), new { StdCode = vm.Learner.StandardCode, vm.Learner.Uln });
            }

            await ApiClient.UpdateCertificateWithAmendReason(new UpdateCertificateWithAmendReasonCommand
            {
                CertificateReference = vm.Learner.CertificateReference,
                IncidentNumber = vm.IncidentNumber,
                Reasons = ParseAmendReasons(vm.Reasons),
                OtherReason = vm.Reasons.Contains("Other") ? vm.OtherReason : string.Empty,
                Username = username
            });

            return RedirectToAction(nameof(Check), new { vm.Learner.CertificateId });
        }

        private AmendReasons? ParseAmendReasons(List<string> reasons)
        {
            var reprintReasons = string.Join(",", reasons.Where(p => !p.Equals("Other")).ToList());

            return !string.IsNullOrWhiteSpace(reprintReasons)
                ? (AmendReasons?)Enum.Parse(typeof(AmendReasons), reprintReasons)
                : null;
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public async Task<IActionResult> ReprintReason(int stdCode, long uln)
        {
            var learner = await ApiClient.GetLearner(stdCode, uln, false);
            var model = new CertificateReprintReasonViewModel
            {
                Learner = learner,
                IncidentNumber = !ModelState.IsValid
                    ? ModelState[nameof(CertificateReprintReasonViewModel.IncidentNumber)]?.AttemptedValue
                    : string.Empty,
                Reasons = !ModelState.IsValid
                    ? new List<string>(ModelState[nameof(CertificateReprintReasonViewModel.Reasons)]?.AttemptedValue?.Split(",") ?? new string[] { })
                    : new List<string>(),
                OtherReason = !ModelState.IsValid
                    ? ModelState[nameof(CertificateReprintReasonViewModel.OtherReason)]?.AttemptedValue
                    : string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        public async Task<IActionResult> ReprintReason(CertificateReprintReasonViewModel vm)
        {
            var username = ContextAccessor.HttpContext.User.UserId();

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ReprintReason), new { StdCode = vm.Learner.StandardCode, vm.Learner.Uln });
            }

            await ApiClient.UpdateCertificateWithReprintReason(new UpdateCertificateWithReprintReasonCommand { 
                CertificateReference = vm.Learner.CertificateReference, 
                IncidentNumber = vm.IncidentNumber, 
                Reasons = ParseReprintReasons(vm.Reasons),
                OtherReason = vm.Reasons.Contains("Other") ? vm.OtherReason : string.Empty,
                Username = username } );

            return RedirectToAction(nameof(Check), new { vm.Learner.CertificateId });
        }

        private ReprintReasons? ParseReprintReasons(List<string> reasons)
        {
            var reprintReasons = string.Join(",", reasons.Where(p => !p.Equals("Other")).ToList());
            
            return !string.IsNullOrWhiteSpace(reprintReasons)
                ? (ReprintReasons?)Enum.Parse(typeof(ReprintReasons), reprintReasons)
                : null;
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