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

        [HttpGet("/Roatp/AllowedProviders/{ukprn}/Remove")]
        public async Task<IActionResult> ConfirmRemoveUkprn(string ukprn)
        {
            if (!int.TryParse(ukprn, out var providerUkprn))
            {
                return RedirectToAction(nameof(List));
            }

            var model = new RemoveUkprnFromAllowedProvidersListViewModel
            {
                AllowedProvider = await _applyApiClient.GetAllowedProviderDetails(providerUkprn)
            };

            return View("~/Views/Roatp/AllowedProviders/ConfirmRemoveUkprn.cshtml", model);
        }

        [HttpPost("/Roatp/AllowedProviders/{ukprn}/Remove")]
        public async Task<IActionResult> RemoveUkprn(int ukprn, RemoveUkprnFromAllowedProvidersListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllowedProvider = await _applyApiClient.GetAllowedProviderDetails(ukprn);
                return View("~/Views/Roatp/AllowedProviders/ConfirmRemoveUkprn.cshtml", model);
            }

            if (model.Confirm is true)
            {
                await _applyApiClient.RemoveFromAllowedProviders(ukprn);
                return RedirectToAction(nameof(UkprnRemoved), new { ukprn });
            }
            else
            {
                return RedirectToAction(nameof(List));
            }     
        }

        [HttpGet("/Roatp/AllowedProviders/{ukprn}/Removed")]
        public IActionResult UkprnRemoved(string ukprn)
        {
            if (!int.TryParse(ukprn, out var providerUkprn))
            {
                return RedirectToAction(nameof(List));
            }

            var model = new UkprnRemovedFromAllowedProvidersListViewModel
            {
                Ukprn = providerUkprn
            };

            return View("~/Views/Roatp/AllowedProviders/UkprnRemoved.cshtml", model);
        }
    }
}
