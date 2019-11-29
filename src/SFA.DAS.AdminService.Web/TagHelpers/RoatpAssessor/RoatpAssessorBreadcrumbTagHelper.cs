using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SFA.DAS.AdminService.Web.TagHelpers.RoatpAssessor
{

    [HtmlTargetElement("roatp-assessor-breadcrumb")]
    public class RoatpAssessorBreadcrumbTagHelper : TagHelper
    {
        private readonly IHtmlGenerator _htmlGenerator;

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("route")]
        public string Route { get; set; }

        public RoatpAssessorBreadcrumbTagHelper(IHtmlGenerator htmlGenerator)
        {
            _htmlGenerator = htmlGenerator;
        }

        public override void Process(TagHelperContext context,
            TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
            output.Attributes.SetAttribute(new TagHelperAttribute("class", "govuk-breadcrumbs"));

            var list = new TagBuilder("ol");
            list.AddCssClass("govuk-breadcrumbs__list");

            SetBreadcrumbItems(list);

            output.Content.SetHtmlContent(list);
        }

        private void SetBreadcrumbItems(TagBuilder list)
        {
            AddRoatpDashboard(list, true);

            switch (Route)
            {
                case RouteNames.RoatpAssessor_Gateway_Dashboard_Get:
                    AddGatewayRoatpApplications(list, false);
                    break;
                case RouteNames.RoatpAssessor_Gateway_Overview_Get:
                    AddGatewayRoatpApplications(list, true);
                    AddGatewayOverview(list, false);
                    break;
                case RouteNames.RoatpAssessor_Gateway_LegalChecks_Get:
                    AddGatewayRoatpApplications(list, true);
                    AddGatewayOverview(list, true);
                    AddGatewayLegalChecks(list);
                    break;
                case RouteNames.RoatpAssessor_Gateway_Parent_Company_Get:
                case RouteNames.RoatpAssessor_Gateway_Website_Get:
                    AddGatewayRoatpApplications(list, true);
                    AddGatewayOverview(list, true);
                    AddGatewayOrganisationInformation(list);
                    break;
            }
        }

        private void AddRoatpDashboard(TagBuilder list, bool renderAsLink) => AddBreadcrumb(list, RouteNames.RoatpDashboard_Index_Get, "RoATP dashboard", renderAsLink);
        private void AddGatewayRoatpApplications(TagBuilder list, bool renderAsLink) => AddBreadcrumb(list, RouteNames.RoatpAssessor_Gateway_Dashboard_Get, "RoATP applications", renderAsLink);
        private void AddGatewayOverview(TagBuilder list, bool renderAsLink) => AddBreadcrumb(list, RouteNames.RoatpAssessor_Gateway_Overview_Get, "Application assessment overview", renderAsLink);
        private void AddGatewayLegalChecks(TagBuilder list) => AddBreadcrumb(list, string.Empty, "Legal and address checks", false);
        private void AddGatewayOrganisationInformation(TagBuilder list) => AddBreadcrumb(list, string.Empty, "Organisation Information", false);

        private void AddBreadcrumb(TagBuilder list, string routeName, string text, bool renderAsLink)
        {
            list.InnerHtml.AppendHtml(renderAsLink 
                ? GetLinkItem(routeName, text) 
                : GetTextItem(text));
        }

        private IHtmlContent GetTextItem(string text)
        {
            var listItem = GetListItem();

            listItem.InnerHtml.AppendHtml(new HtmlString(text));

            return listItem;
        }

        private IHtmlContent GetLinkItem(string routeName, string text)
        {
            var listItem = GetListItem();

            var link = _htmlGenerator.GenerateRouteLink(
                ViewContext,
                linkText: text,
                routeName: routeName,
                protocol: null,
                hostName: null,
                fragment: null,
                routeValues: null,
                htmlAttributes: new { @class = "govuk-breadcrumbs__link" });

            listItem.InnerHtml.AppendHtml(link);

            return listItem;
        }

        private TagBuilder GetListItem()
        {
            var listItem = new TagBuilder("li");
            listItem.AddCssClass("govuk-breadcrumbs__list-item");

            return listItem;
        }
    }
}