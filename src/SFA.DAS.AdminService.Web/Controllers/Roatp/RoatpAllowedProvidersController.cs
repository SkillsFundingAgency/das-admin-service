namespace SFA.DAS.AdminService.Web.Controllers.Roatp
{
    using System.Threading.Tasks;
    using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SFA.DAS.AdminService.Web.Domain;
    using SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowedProviders;
    using System;

    [Authorize(Roles = Roles.RoatpGatewayTeam)]
    public class RoatpAllowedProvidersController : Controller
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;

        public RoatpAllowedProvidersController(IRoatpApplicationApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;
        }

        [HttpGet("/Roatp/AllowedProviders")]
        public async Task<IActionResult> List(string sortColumn, string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            var model = new AddUkprnToAllowedProvidersListViewModel
            {
                SortColumn = sortColumn,
                SortOrder = sortOrder,
                Ukprn = null,
                StartDate = startDate,
                EndDate = endDate,
                AllowedProviders = await _applyApiClient.GetAllowedProvidersList(sortColumn, sortOrder)
            };

            return View("~/Views/Roatp/AllowedProviders/List.cshtml", model);
        }

        [HttpPost("/Roatp/AllowedProviders")]
        public async Task<IActionResult> AddUkprn(AddUkprnToAllowedProvidersListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllowedProviders = await _applyApiClient.GetAllowedProvidersList(model.SortColumn, model.SortOrder);
                return View("~/Views/Roatp/AllowedProviders/List.cshtml", model);
            }

            await _applyApiClient.AddToAllowedProviders(int.Parse(model.Ukprn), model.StartDate.Value, model.EndDate.Value);

            return RedirectToAction(nameof(List), model);
        }
    }
}
