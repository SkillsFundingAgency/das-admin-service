using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Api.Types.Enums;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
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
        public async Task<IActionResult> AmendReason(CertificateAmendReasonViewModel viewModel)
        {
            var username = ContextAccessor.HttpContext.User.UserId();

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(AmendReason), new { StdCode = viewModel.Learner.StandardCode, viewModel.Learner.Uln });
            }

            await ApiClient.UpdateCertificateWithAmendReason(new UpdateCertificateWithAmendReasonCommand
            {
                CertificateReference = viewModel.Learner.CertificateReference,
                IncidentNumber = viewModel.IncidentNumber,
                Reasons = ParseAmendReasons(viewModel.Reasons),
                OtherReason = viewModel.Reasons.Contains("Other") ? viewModel.OtherReason : string.Empty,
                Username = username
            });

            return RedirectToAction(nameof(Check), new { viewModel.Learner.CertificateId });
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
        public async Task<IActionResult> ReprintReason(CertificateReprintReasonViewModel viewModel)
        {
            var username = ContextAccessor.HttpContext.User.UserId();

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ReprintReason), new { StdCode = viewModel.Learner.StandardCode, viewModel.Learner.Uln });
            }

            await ApiClient.UpdateCertificateWithReprintReason(new UpdateCertificateWithReprintReasonCommand { 
                CertificateReference = viewModel.Learner.CertificateReference, 
                IncidentNumber = viewModel.IncidentNumber, 
                Reasons = ParseReprintReasons(viewModel.Reasons),
                OtherReason = viewModel.Reasons.Contains("Other") ? viewModel.OtherReason : string.Empty,
                Username = username } );

            return RedirectToAction(nameof(Check), new { viewModel.Learner.CertificateId });
        }

        private ReprintReasons? ParseReprintReasons(List<string> reasons)
        {
            var reprintReasons = string.Join(",", reasons.Where(p => !p.Equals("Other")).ToList());
            
            return !string.IsNullOrWhiteSpace(reprintReasons)
                ? (ReprintReasons?)Enum.Parse(typeof(ReprintReasons), reprintReasons)
                : null;
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public async Task<IActionResult> Check(Guid certificateId, string searchString, int page)
        {
            var actionResult = await LoadViewModel<CertificateCheckViewModel>(certificateId, "~/Views/CertificateAmend/Check.cshtml");
            var viewResult = (actionResult as ViewResult);
            var viewModel = viewResult.Model as CertificateCheckViewModel;

            viewModel.Page = page;
            viewModel.SearchString = searchString;

            var standards = await ApiClient.GetStandardVersions(viewModel.StandardCode);
            viewModel.StandardHasMultipleVersions = standards.Count() > 1;

            var options = await ApiClient.GetStandardOptions(viewModel.GetStandardId());
            viewModel.ShowOptionsChangeLink = options != null && options.HasMoreThanOneOption();

            return actionResult;
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        public IActionResult Check(CertificateCheckViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Check", new { CertificateId = viewModel.Id, viewModel.SearchString, viewModel.Page });
            }

            if (viewModel.CanRequestReprint)
            {
                return RedirectToAction("ConfirmReprint",
                    new
                    {
                        certificateId = viewModel.Id,
                        redirectToCheck = viewModel.RedirectToCheck,
                        Uln = viewModel.Uln,
                        StdCode = viewModel.StandardCode,
                        SearchString = viewModel.SearchString,
                        Page = viewModel.Page
                    });
            }

            return RedirectToAction("ConfirmAmend",
                new
                {
                    certificateId = viewModel.Id,
                    redirectToCheck = viewModel.RedirectToCheck,
                    Uln = viewModel.Uln,
                    StdCode = viewModel.StandardCode,
                    SearchString = viewModel.SearchString,
                    Page = viewModel.Page
                });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmReprint(Guid certificateId,
            long uln,
            int stdCode,
            string searchString,
            int page = 1)
        {
            var viewModel = await ReprintCertificate(certificateId, searchString, stdCode, uln, page);
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmAmend(Guid certificateId,
            bool redirectToCheck,
            long uln,
            int stdCode,
            string searchString,
            int page = 1)
        {
            var certificate = await ApiClient.GetCertificate(certificateId);
            var certData = JsonSerializer.Deserialize<CertificateData>(certificate.CertificateData);

            return View(new CertificateAmendConfirmViewModel 
            { 
                CertificateId = certificateId,
                SearchString = searchString,
                Page = page,
                Uln = uln,
                StdCode = stdCode,
                BackToCheckPage = redirectToCheck,
                CertificateReference = certificate.CertificateReference,
                FullName = certData.FullName
            });
        }

        private async Task<CertificateReprintConfirmViewModel> ReprintCertificate(Guid certificateId, string searchString, int stdCode, long uln, int page)
        {
            var username = ContextAccessor.HttpContext.User.UserId();
           
            var certificate = await ApiClient.UpdateCertificateRequestReprint(new UpdateCertificateRequestReprintCommand
            {
                CertificateId = certificateId,
                Username = username
            });

            var certData = JsonSerializer.Deserialize<CertificateData>(certificate.CertificateData);

            var nextScheduledRun = await ApiClient.GetNextScheduledRun((int)ScheduleType.PrintRun);

            var viewModel = new CertificateReprintConfirmViewModel
            {
                CertificateId = certificate.Id,
                NextBatchDate = nextScheduledRun?.RunTime.ToString("dd/MM/yyyy"),
                CertificateReference = certificate.CertificateReference,
                SearchString = searchString,
                StdCode = stdCode,
                Uln = uln,
                Status = certificate.Status,
                FullName = certData.FullName,
                Page = page
            };

            return viewModel;
        }
    }
}