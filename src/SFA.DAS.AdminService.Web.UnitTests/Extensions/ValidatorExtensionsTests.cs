using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Extensions;

namespace SFA.DAS.AdminService.Web.Tests.Extensions
{
    public class ValidatorExtensionsTests
    {
        public class ConstructDate
        {
            [Test]
            public void When_ValidParameters_Then_ReturnDate()
            {
                var result = ValidatorExtensions.ConstructDate("01", "02", "2021");

                result.Should().NotBeNull();
                result.Should().HaveValue();
                result.Should().HaveDay(1);
                result.Should().HaveMonth(2);
                result.Should().HaveYear(2021);
            }

            [Test]
            public void When_InvalidParameters_Then_ReturnNull()
            {
                var result = ValidatorExtensions.ConstructDate("31", "02", "2021");

                result.Should().BeNull();
            }
        }

        public class IsValidDate
        {
            [Test]
            public void When_ValidParameters_Then_ReturnTrue()
            {
                var result = ValidatorExtensions.IsValidDate(2021, 2, 1);

                result.Should().BeTrue();
            }

            [Test]
            public void When_InvalidParameters_Then_ReturnFalse()
            {
                var result = ValidatorExtensions.IsValidDate(2021, 2, 31);

                result.Should().BeFalse();
            }
        }
    }
}
