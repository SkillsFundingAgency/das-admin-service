﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.AllowedProviders;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Controllers.Roatp.AllowedProviders
{
    [Authorize(Roles = Roles.RoatpGatewayTeam)]
    public class RoatpAllowedProvidersController : Controller
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;

        public RoatpAllowedProvidersController(IRoatpApplicationApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;
        }

        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [HttpGet("/Roatp/AllowedProviders")]
        public async Task<IActionResult> Index(string sortColumn, string sortOrder, DateTime? startDate, DateTime? endDate)
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

            return View("~/Views/Roatp/AllowedProviders/Index.cshtml", model);
        }

        [ModelStatePersist(ModelStatePersist.Store)]
        [HttpPost("/Roatp/AllowedProviders")]
        public async Task<IActionResult> AddUkprn(AddUkprnToAllowedProvidersListViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _applyApiClient.AddToAllowedProviders(int.Parse(model.Ukprn), model.StartDate.Value, model.EndDate.Value);
            }

            return RedirectToAction(nameof(Index), new { sortColumn = model.SortColumn, sortOrder = model.SortOrder, startDate = model.StartDate?.ToString("yyyy-MM-dd"), endDate = model.EndDate?.ToString("yyyy-MM-dd") });
        }

        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [HttpGet("/Roatp/AllowedProviders/{ukprn}/ConfirmRemove")]
        public async Task<IActionResult> ConfirmRemoveUkprn(string ukprn)
        {
            if (!int.TryParse(ukprn, out var providerUkprn))
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new RemoveUkprnFromAllowedProvidersListViewModel
            {
                AllowedProvider = await _applyApiClient.GetAllowedProviderDetails(providerUkprn)
            };

            return View("~/Views/Roatp/AllowedProviders/ConfirmRemoveUkprn.cshtml", model);
        }

        [ModelStatePersist(ModelStatePersist.Store)]
        [HttpPost("/Roatp/AllowedProviders/{ukprn}/ConfirmRemove")]
        public async Task<IActionResult> RemoveUkprn(int ukprn, RemoveUkprnFromAllowedProvidersListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ConfirmRemoveUkprn), new { ukprn });               
            }

            if (model.Confirm is true)
            {
                await _applyApiClient.RemoveFromAllowedProviders(ukprn);
                return RedirectToAction(nameof(UkprnRemoved), new { ukprn });
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }     
        }

        [HttpGet("/Roatp/AllowedProviders/{ukprn}/Removed")]
        public IActionResult UkprnRemoved(string ukprn)
        {
            if (!int.TryParse(ukprn, out var providerUkprn))
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new UkprnRemovedFromAllowedProvidersListViewModel
            {
                Ukprn = providerUkprn
            };

            return View("~/Views/Roatp/AllowedProviders/UkprnRemoved.cshtml", model);
        }
    }
}
