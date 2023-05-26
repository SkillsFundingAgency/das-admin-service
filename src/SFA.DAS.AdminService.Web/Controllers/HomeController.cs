using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Models;

namespace SFA.DAS.AdminService.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebConfiguration _applicationConfiguration;
        private const string ServiceName = "SFA.DAS.AdminService";
        private const string Version = "1.0";

        public HomeController(IConfiguration configuration)
        {
            _applicationConfiguration = ConfigurationService.GetConfig<WebConfiguration>(
                    configuration["EnvironmentName"],
                    configuration["ConfigurationStorageConnectionString"],
                    Version,
                    ServiceName)
                .Result;
        }

        public IActionResult Index()
        {
            return View(new HomeViewModel{ UseDfESignIn = _applicationConfiguration.UseDfeSignIn });
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        public IActionResult InvalidRole()
        {
            return View();
        }
    }
}
