using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Models.Merge;
using System;

namespace SFA.DAS.AdminService.Web.Tests.Models
{
    public class MergeRequestTests
    {
        private Fixture _autoFixture;
        private MergeRequest _mergeRequest;

        [SetUp]
        public void Arrange()
        {
            _autoFixture = new Fixture();
            _mergeRequest = new MergeRequest();
        }

        [Test]
        public void When_SettingSecondaryEpaoEffectiveToDate_ThenDateIsUpdated()
        {
            var dateTime = _autoFixture.Build<DateTime>().Create();

            _mergeRequest.SetSecondaryEpaoEffectiveToDate(dateTime.Day, dateTime.Month, dateTime.Year);

            _mergeRequest.SecondaryEpaoEffectiveTo.Value.Should().Be(dateTime.Date);
        }
    }
}
