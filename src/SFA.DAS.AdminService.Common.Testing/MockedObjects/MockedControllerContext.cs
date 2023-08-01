using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IO;

namespace SFA.DAS.AdminService.Common.Testing.MockedObjects
{
    public class MockedControllerContext
    {
        public static ControllerContext Setup()
        {
            return Setup(string.Empty);
        }

        public static ControllerContext Setup(string buttonToAdd)
        {
            var user = MockedUser.Setup();

            var controllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            if (!string.IsNullOrEmpty(buttonToAdd))
            {
                var clarificationFileName = "file.pdf";
                var file = new FormFile(new MemoryStream(), 0, 0, clarificationFileName, clarificationFileName);
                var formFileCollection = new FormFileCollection { file };
                var dictionary = new Dictionary<string, StringValues>();
                dictionary.Add(buttonToAdd, clarificationFileName);
                controllerContext.HttpContext.Request.Form = new FormCollection(dictionary, formFileCollection);
            }
            else
            {
                controllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());
            }

            return controllerContext;
        }
    }
}
