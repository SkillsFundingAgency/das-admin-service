using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Roatp
{
    [SetUpFixture]
    public class GlobalTestSetup
    {
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            MappingStartup.AddMappings();
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            AutoMapper.Mapper.Reset();
        }
    }
}
