using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Snapshot;
using System.Linq;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using SFA.DAS.AdminService.Web.Infrastructure.FeatureToggles;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.Apply
{
    [Authorize(Roles = Roles.RoatpGatewayTeam + "," + Roles.CertificationTeam)]
    [FeatureToggle(FeatureToggles.EnableRoatpSnapshot, "Dashboard", "Index")]
    public class RoatpSnapshotController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;

        public RoatpSnapshotController(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IConfiguration configuration)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _configuration = configuration;
        }

        [HttpGet("/Roatp/Snapshot")]
        public IActionResult Index()
        {
            if (_configuration["EnvironmentName"].EndsWith("PROD", StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var viewmodel = new SnapshotViewModel();
            return View("~/Views/Roatp/Apply/Snapshot/Index.cshtml", viewmodel);
        }

        [HttpPost("/Roatp/Snapshot")]
        public async Task<IActionResult> PerformSnapshot(SnapshotViewModel vm)
        {
            if (_configuration["EnvironmentName"].EndsWith("PROD", StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var application = await _applyApiClient.GetApplication(vm.ApplicationId ?? Guid.NewGuid());

            if (vm.ApplicationId is null)
            {
                ModelState.AddModelError("ApplicationId", "Please enter Application Id");
            }
            else if (application is null)
            {
                ModelState.AddModelError("ApplicationId", "Application not found");
            }

            if (ModelState.IsValid)
            {
                var snapshotResponse = await _qnaApiClient.SnapshotApplication(application.ApplicationId);

                var allSnapshotSequences = await _qnaApiClient.GetAllApplicationSequences(snapshotResponse.ApplicationId);
                var allSnapshotSections = await _qnaApiClient.GetAllApplicationSections(snapshotResponse.ApplicationId);

                var applySequences = application.ApplyData.Sequences;

                // must update all ids!
                foreach (var sequence in applySequences)
                {
                    var snapshotSequence = allSnapshotSequences.FirstOrDefault(seq => seq.SequenceNo == sequence.SequenceNo);
                    if (snapshotSequence != null)
                    {
                        sequence.SequenceId = snapshotSequence.Id;

                        foreach (var section in sequence.Sections)
                        {
                            var snapshotSection = allSnapshotSections.FirstOrDefault(sec => sec.SequenceNo == sequence.SequenceNo && sec.SectionNo == section.SectionNo);

                            if (snapshotSection != null)
                            {
                                section.SectionId = snapshotSection.Id;
                            }
                        }
                    }
                }

                var applySnapshotResponse = await _applyApiClient.SnapshotApplication(application.ApplicationId, snapshotResponse.ApplicationId, applySequences);

                if (applySnapshotResponse != Guid.Empty)
                {
                    vm.SnapshotApplicationId = applySnapshotResponse;
                    vm.SnapshotSuccessful = true;
                }
                else
                {
                    vm.SnapshotSuccessful = false;
                }
            }

            return View("~/Views/Roatp/Apply/Snapshot/Index.cshtml", vm);
        }
    }
}
