﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.ViewModels.Merge;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.MergeOrganisations
{
    public class WhenRequestingSetSecondaryEpaoEffectiveToDatePage : MergeControllerTestBase
    {
        [Test]
        public void Then_GetMergeRequestFromSession()
        {
            MergeController.SetSecondaryEpaoEffectiveToDate();

            VerifyGetMergeRequest();
        }

        [Test]
        public void Then_ReturnViewResult()
        {
            var viewResult = MergeController.SetSecondaryEpaoEffectiveToDate() as ViewResult;

            var model = viewResult.Model as SetSecondaryEpaoEffectiveToDateViewModel;

            model.SecondaryEpaoName.Should().Be(_mergeRequest.SecondaryEpao.Name);
            model.Day.Should().Be(_mergeRequest.SecondaryEpaoEffectiveTo.Value.Day.ToString());
            model.Month.Should().Be(_mergeRequest.SecondaryEpaoEffectiveTo.Value.Month.ToString());
            model.Year.Should().Be(_mergeRequest.SecondaryEpaoEffectiveTo.Value.Year.ToString());
        }
    }
}
