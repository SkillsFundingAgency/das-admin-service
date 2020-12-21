namespace SFA.DAS.AdminService.Web.Controllers.Roatp
{
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using SFA.DAS.AdminService.Web.Domain;

  [Authorize(Roles = Roles.RoatpGatewayTeam)]
  public class RoatpHomeController : Controller
  {
    [Route("search-apprenticeship-training-providers")]
    public IActionResult Index()
    {
      return View("~/Views/Roatp/Index.cshtml");
    }
  }
}
