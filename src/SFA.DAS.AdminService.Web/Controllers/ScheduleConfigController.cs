﻿using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Domain.Roles.OperationsTeam + "," + Domain.Roles.CertificationTeam)]
    public class ScheduleConfigController : Controller
    {
        private readonly IScheduleApiClient _scheduleApiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public ScheduleConfigController(IScheduleApiClient scheduleApiClient, IHttpContextAccessor contextAccessor)
        {
            _scheduleApiClient = scheduleApiClient;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<ScheduleConfigViewModel> viewModels = new List<ScheduleConfigViewModel>();

            foreach (var schedule in await _scheduleApiClient.GetAllScheduledRun((int)ScheduleJobType.PrintRun))
            {
                ScheduleConfigViewModel viewModel = MapToScheduleConfigViewModel(schedule);

                viewModels.Add(viewModel);
            }

            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid scheduleRunId)
        {
            var schedule = await _scheduleApiClient.GetScheduleRun(scheduleRunId);

            ScheduleConfigViewModel viewModel = new ScheduleConfigViewModel
            {
                Id = schedule.Id,
                RunTime = schedule.RunTime.UtcToTimeZoneTime(),
                Interval = schedule.Interval.HasValue ? TimeSpan.FromMinutes(schedule.Interval.Value).Humanize().Titleize() : "-",
                IsRecurring = schedule.IsRecurring,
                ScheduleType = (ScheduleJobType)schedule.ScheduleType,
            };

            if (Enum.TryParse<ScheduleInterval>(schedule.Interval.ToString(), out var scheduleInterval) && Enum.IsDefined(typeof(ScheduleInterval), scheduleInterval))
            {
                viewModel.ScheduleInterval = scheduleInterval;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ScheduleConfigViewModel viewModel)
        {
            if (viewModel != null)
            {
                await _scheduleApiClient.DeleteScheduleRun(viewModel.Id);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            DateTime now = DateTime.UtcNow.UtcToTimeZoneTime();

            var viewModel = new ScheduleConfigViewModel
            {
                ScheduleType = ScheduleJobType.PrintRun,
                RunTime = now,
                Year = now.Year,
                Month = now.Month,
                Day = now.Day,
                Hour = now.Hour,
                Minute = now.Minute,
                IsRecurring = false,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ScheduleConfigViewModel viewModel)
        {
            if (viewModel != null)
            {
                // Binding to a nullable enum will cause a ModelState error
                ViewData.ModelState.Remove(nameof(viewModel.ScheduleInterval));

                if (!ViewData.ModelState.IsValid)
                {
                    return View(viewModel);
                }

                ScheduleRun schedule = new ScheduleRun
                {
                    ScheduleType = (ScheduleType)viewModel.ScheduleType,
                    RunTime = viewModel.Date.UtcFromTimeZoneTime(),
                    IsRecurring = viewModel.ScheduleInterval.HasValue,
                    Interval = (int?)viewModel.ScheduleInterval
                };

                await _scheduleApiClient.CreateScheduleRun(schedule);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> RunNow(int scheduleType)
        {
            var viewModel = new ScheduleConfigViewModel
            {
                ScheduleType = (ScheduleJobType)scheduleType,
                RunTime = DateTime.UtcNow.UtcToTimeZoneTime(),
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Restart(Guid scheduleRunId)
        {
            var schedule = await _scheduleApiClient.GetScheduleRun(scheduleRunId);

            var viewModel = MapToScheduleConfigViewModel(schedule);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RunNow(ScheduleConfigViewModel viewModel)
        {
            if (viewModel != null)
            {
                await _scheduleApiClient.RunNowScheduledRun((int)viewModel.ScheduleType);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Restart(ScheduleConfigViewModel viewModel)
        {
            if (viewModel != null)
            {
                await _scheduleApiClient.RestartSchedule(viewModel.Id);
            }

            return RedirectToAction("Index");
        }

        private static ScheduleConfigViewModel MapToScheduleConfigViewModel(ScheduleRun schedule)
        {
            ScheduleConfigViewModel viewModel = new ScheduleConfigViewModel
            {
                Id = schedule.Id,
                RunTime = schedule.RunTime.UtcToTimeZoneTime(),
                Interval = schedule.Interval.HasValue ? TimeSpan.FromMinutes(schedule.Interval.Value).Humanize().Titleize() : "-",
                IsRecurring = schedule.IsRecurring,
                ScheduleType = (ScheduleJobType)schedule.ScheduleType,
                LastRunStatus = schedule.LastRunStatus
            };

            if (Enum.TryParse<ScheduleInterval>(schedule.Interval.ToString(), out var scheduleInterval) && Enum.IsDefined(typeof(ScheduleInterval), scheduleInterval))
            {
                viewModel.ScheduleInterval = scheduleInterval;
            }

            return viewModel;
        }
    }
}