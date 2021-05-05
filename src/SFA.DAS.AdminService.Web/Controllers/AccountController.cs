using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Domain;
using System.Linq;
using System.Security.Claims;

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
                var userName = HttpContext.User.Identity.Name ?? HttpContext.User.FindFirstValue(ClaimTypes.Upn);
                var roles = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == Domain.Roles.RoleClaimType).Select(c => c.Value);
                _logger.LogInformation($"PostSignIn - User '{userName}' does not have a valid role. They have the following roles: '{string.Join(",", roles)}'");

                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }

                return RedirectToAction("InvalidRole", "Home");
            }

            if (HttpContext.User.HasRoatpRoleOnly())
            {
                return RedirectToAction("Index", "RoatpHome");
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
            if (HttpContext.User != null)
            {
                var userName = HttpContext.User.Identity.Name ?? HttpContext.User.FindFirstValue(ClaimTypes.Upn);
                var roles = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == Domain.Roles.RoleClaimType).Select(c => c.Value);

                _logger.LogError($"AccessDenied - User '{userName}' does not have a valid role. They have the following roles: '{string.Join(",", roles)}'");
            }

            return View();
        }
    }
}