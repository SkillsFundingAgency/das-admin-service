namespace SFA.DAS.AdminService.Web.Controllers.Roatp
{
    using System.Threading.Tasks;
    using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SFA.DAS.AdminService.Web.Domain;
    using SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowList;
    using System;

    [Authorize(Roles = Roles.RoatpGatewayTeam)]
    public class RoatpAllowListController : Controller
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;

        public RoatpAllowListController(IRoatpApplicationApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;
        }

        [HttpGet("/Roatp/AllowList")]
        public async Task<IActionResult> List(string sortColumn, string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            var model = new AddUkprnToAllowListViewModel
            {
                SortColumn = sortColumn,
                SortOrder = sortOrder,
                Ukprn = null,
                StartDate = startDate,
                EndDate = endDate,
                AllowedUkprns = await _applyApiClient.GetAllowedUkprns(sortColumn, sortOrder)
            };

            return View("~/Views/Roatp/AllowList/List.cshtml", model);
        }

        [HttpPost("/Roatp/AllowList")]
        public async Task<IActionResult> AddUkprn(AddUkprnToAllowListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllowedUkprns = await _applyApiClient.GetAllowedUkprns(model.SortColumn, model.SortOrder);
                return View("~/Views/Roatp/AllowList/List.cshtml", model);
            }

            await _applyApiClient.AddToAllowUkprns(model.Ukprn, model.StartDate.Value, model.EndDate.Value);

            return RedirectToAction(nameof(List), model);
        }
    }
}
