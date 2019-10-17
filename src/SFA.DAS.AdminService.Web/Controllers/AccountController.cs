using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Domain;

namespace SFA.DAS.AdminService.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            _logger.LogInformation("Start of Sign In");
            var redirectUrl = Url.Action(nameof(PostSignIn), "Account");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                WsFederationDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult PostSignIn()
        {
            if(!HttpContext.User.HasValidRole())
            {
                _logger.LogInformation($"PostSignIn - User '{HttpContext.User.Identity.Name}' does not have a valid role");
                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }

                return RedirectToAction("InvalidRole", "Home");
            }

            if (HttpContext.User.HasRoatpRolesOnly())
            {
                if(User.IsInRole(Roles.RoatpGatewayTeam) && User.IsInRole(Roles.RoatpAssessorTeam))
                {
                    return RedirectToAction("Index", "RoatpDashboard");
                }

                if (User.IsInRole(Roles.RoatpGatewayTeam))
                {
                    return RedirectToAction("Index", "RoatpHome");
                }

                if (User.IsInRole(Roles.RoatpAssessorTeam))
                {
                    return RedirectToAction("Index", "RoatpAssessorDashboard");
                }
            }

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = Url.Action(nameof(SignedOut), "Account", values: null, protocol: Request.Scheme);

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                CookieAuthenticationDefaults.AuthenticationScheme,
                WsFederationDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult SignedOut()
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}