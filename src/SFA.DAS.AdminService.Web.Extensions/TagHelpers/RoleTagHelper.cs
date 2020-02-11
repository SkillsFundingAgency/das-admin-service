using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Extensions.TagHelpers
{
    [HtmlTargetElement("div", Attributes = RoleTagHelperAttributeName)]
    [HtmlTargetElement("a", Attributes = RoleTagHelperAttributeName)]
    [HtmlTargetElement("th", Attributes = RoleTagHelperAttributeName)]
    [HtmlTargetElement("td", Attributes = RoleTagHelperAttributeName)]
    public class RoleTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAuthorizationService _authorizationService;

        public RoleTagHelper(IHttpContextAccessor contextAccessor, IAuthorizationService authorizationService)
        {
            _contextAccessor = contextAccessor;
            _authorizationService = authorizationService;
        }

        private const string RoleTagHelperAttributeName = "sfa-show-for-roles";

        [HtmlAttributeName(RoleTagHelperAttributeName)]
        public string Roles { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(!ShowForRoles(Roles))
            {
                output.SuppressOutput();
            }
        }

        public bool ShowForRoles(string roles)
        {
            string[] roleArray = roles.Split(',');
            return roleArray.Any(role => _contextAccessor.HttpContext.User.IsInRole(role));
        }
    }
}