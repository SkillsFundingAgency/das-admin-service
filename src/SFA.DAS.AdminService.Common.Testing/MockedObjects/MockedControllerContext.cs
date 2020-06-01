using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.AdminService.Common.Testing.MockedObjects
{
    public class MockedControllerContext
    {
        public static ControllerContext Setup()
        {
            var user = MockedUser.Setup();

            var controllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            return controllerContext;
        }
    }
}
