using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System;

namespace SFA.DAS.AdminService.Web.Tests.Attributes
{
    public class ExceptionFilterServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            var executor = new Mock<IActionResultExecutor<RedirectToActionResult>>();

            return executor.Object;
        }
    }
}
