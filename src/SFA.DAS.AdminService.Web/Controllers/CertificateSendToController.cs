using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.CertificateAmend;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers
{
    public class CertificateSendToController : CertificateBaseController
    {
        public CertificateSendToController(ILogger<CertificateAmendController> logger, IHttpContextAccessor contextAccessor,
            IApiClient apiClient) : base(logger, contextAccessor, apiClient)
        {
        }

        [HttpGet]
        public async Task<IActionResult> SendTo(Guid certificateId)
        {
            var actionResult = await LoadViewModel<CertificateSendToViewModel>(certificateId, "~/Views/CertificateAmend/SendTo.cshtml");
            return actionResult;
        }
        
        [HttpPost(Name = "SendTo")]
        public async Task<IActionResult> SendTo(CertificateSendToViewModel vm)
        {
            var certificate = await ApiClient.GetCertificate(vm.Id);
            var certData = JsonSerializer.Deserialize<CertificateData>(certificate.CertificateData);

            if (certData.SendTo != vm.SendTo)
            {
                return await SaveViewModel(vm,
                    returnToIfModelNotValid: "~/Views/CertificateAmend/SendTo.cshtml",
                    nextAction: RedirectToAction("Address", "CertificateAddress", new { certificateId = vm.Id, editForm = false }), action: CertificateActions.SendTo);
            }

            return RedirectToAction("Check", "CertificateAmend", new { certificateId = vm.Id });
        }
    }
}