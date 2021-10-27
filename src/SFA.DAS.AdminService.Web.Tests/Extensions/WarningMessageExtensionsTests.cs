using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Extensions;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Tests.Extensions
{
    public class WarningMessageExtensionsTests
    {
        private List<string> _warnings;

        [SetUp]
        public void Arrange()
        {
            _warnings = new List<string>
            {
                "warning 1",
                "Warning 2"
            };
        }
        [Test]
        public void When_WarningMessagesNotEmpty_Then_AddWarnings()
        {
            var newWarnings = new List<string>
            {
                "warning 3",
                "Warning 4"
            };

            _warnings.AddWarningMessages(newWarnings);

            _warnings.Count.Should().Be(4);
        }
    }
}
