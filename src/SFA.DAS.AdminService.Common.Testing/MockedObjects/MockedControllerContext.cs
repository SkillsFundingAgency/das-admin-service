using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

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

            controllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());

            return controllerContext;
        }
    }
}
