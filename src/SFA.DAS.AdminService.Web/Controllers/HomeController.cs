using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AdminService.Web.Models;

namespace SFA.DAS.AdminService.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebConfiguration _webConfiguration;

        public HomeController(
            IWebConfiguration webConfiguration)
        {
            _webConfiguration = webConfiguration;
        }

        public IActionResult Index()
        {
            // if the user is already signed in, then redirect the user to the Dashboard index page.
            if (_webConfiguration.UseDfESignIn && User.Identity != null && User.Identity.IsAuthenticated) return RedirectToAction("Index", "Dashboard");

            return View(new HomeViewModel { UseDfESignIn = _webConfiguration.UseDfESignIn });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        public IActionResult InvalidRole()
        {
            return View(new Error403ViewModel { UseDfESignIn = _webConfiguration.UseDfESignIn, HelpPageLink = _webConfiguration.DfESignInServiceHelpUrl });
        }
    }
}
