using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Extensions
{
    public class DateTimeExtensionTests
    {
        [TestCase(1, 1, 2021, "01.01.2021")]
        [TestCase(12, 12, 2021, "12.12.2021")]
        public void When_ConvertingDateTimeToShortNumericFormat_And_DateTimeIsValid_Then_CorrectStringReturned(int day, int month, int year, string expectedDateString)
        {
            DateTime? dateTime;

            dateTime = new DateTime(year, month, day);

            var result = dateTime.ToShortNumericFormatString();

            Assert.AreEqual(expectedDateString, result);
        }

        [Test]
        public void When_ConvertingDateTimeToShortNumericFormat_And_DateTimeIsNull_Then_EmptyString()
        {
            DateTime? dateTime = null;

            var result = dateTime.ToShortNumericFormatString();

            Assert.AreEqual(string.Empty, result);
        }
    }
}
