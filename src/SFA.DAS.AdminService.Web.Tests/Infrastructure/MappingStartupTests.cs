using AutoMapper;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;

namespace SFA.DAS.AdminService.Web.Tests
{
    [TestFixture]
    public class MappingStartupTests
    {
        [Test]
        public void IsAutomapperConfigurationValid()
        {
            Mapper.Reset();

            MappingStartup.AddMappings();

            Mapper.AssertConfigurationIsValid();
        }
    }
}