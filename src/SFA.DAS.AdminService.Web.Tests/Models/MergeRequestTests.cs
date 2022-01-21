using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Models.Merge;
using System;
using System.Linq;

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
            _mergeRequest.StartNewRequest();
        }

        [Test]
        public void When_StartingNewRequest_Then_MergeRequestResetAndStartCommandAdded()
        {
            var command = _mergeRequest.Actions.Single();

            command.CommandName.Should().Be(SessionCommands.StartSession);
        }

        [Test]
        public void When_UpdatingPrimaryEpao_Then_Update_And_AddCommand()
        {
            var epaoId = _autoFixture.Create<string>();
            var name = _autoFixture.Create<string>();
            var ukprn = _autoFixture.Create<long>();

            _mergeRequest.UpdateEpao("primary", epaoId, name, ukprn, "");

            _mergeRequest.PrimaryEpao.Id.Should().Be(epaoId);
            _mergeRequest.PrimaryEpao.Name.Should().Be(name);
            _mergeRequest.PrimaryEpao.Ukprn.Should().Be(ukprn);

            _mergeRequest.PreviousCommand.CommandName.Should().Be(SessionCommands.ConfirmPrimaryEpao);
        }

        [Test]
        public void When_UpdatingSecondaryEpao_Then_Update_And_AddCommand()
        {
            var epaoId = _autoFixture.Create<string>();
            var name = _autoFixture.Create<string>();
            var ukprn = _autoFixture.Create<long>();

            _mergeRequest.UpdateEpao("secondary", epaoId, name, ukprn, "");

            _mergeRequest.SecondaryEpao.Id.Should().Be(epaoId);
            _mergeRequest.SecondaryEpao.Name.Should().Be(name);
            _mergeRequest.SecondaryEpao.Ukprn.Should().Be(ukprn);

            _mergeRequest.PreviousCommand.CommandName.Should().Be(SessionCommands.ConfirmSecondaryEpao);
        }

        [Test]
        public void When_SettingSecondaryEpaoEffectiveToDate_ThenDateIsUpdated()
        {
            var dateTime = _autoFixture.Build<DateTime>().Create();

            _mergeRequest.SetSecondaryEpaoEffectiveToDate(dateTime.Day, dateTime.Month, dateTime.Year);

            _mergeRequest.SecondaryEpaoEffectiveTo.Value.Should().Be(dateTime.Date);
        }

        [TestCase("primary", SessionCommands.SearchPrimaryEpao)]
        [TestCase("secondary", SessionCommands.SearchSecondaryEpao)]
        public void When_AddingSearchCommand(string type, string expectedCommandName)
        {
            var searchString = _autoFixture.Create<string>();

            _mergeRequest.AddSearchEpaoCommand(type, searchString);

            var command = _mergeRequest.Actions.OrderByDescending(a => a.Order).FirstOrDefault();

            command.SearchString.Should().Be(searchString);
            command.CommandName.Should().Be(expectedCommandName);
        }
    }
}
