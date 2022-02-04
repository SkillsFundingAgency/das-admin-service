using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AdminService.Web.Infrastructure.Merge;
using SFA.DAS.AdminService.Web.Models.Merge;
using System;

namespace SFA.DAS.AdminService.Web.Attributes
{
    public class MergeRequestFilter : Attribute, IActionFilter
    {
        private const string _mergeOrganisationTypeKey = "mergeOrganisationType";

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var sessionService = context.HttpContext.RequestServices.GetService<IMergeOrganisationSessionService>();

            var mergeRequest = sessionService.GetMergeRequest();

            if (mergeRequest.Completed)
            {
                context.Result = new RedirectToActionResult("MergeComplete", "MergeOrganisations", new { });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var sessionService = context.HttpContext.RequestServices.GetService<IMergeOrganisationSessionService>();

            var mergeRequest = sessionService.GetMergeRequest();

            var actionName = context.ActionDescriptor.RouteValues["action"];

            var type = context.RouteData.Values.ContainsKey(_mergeOrganisationTypeKey) ? context.RouteData.Values[_mergeOrganisationTypeKey].ToString() : "";

            var navigatingBack = context.HttpContext.Request.Query.ContainsKey("back") ? bool.Parse(context.HttpContext.Request.Query["back"]) : false;
            
            var lastCommand = mergeRequest.PreviousCommand;

            if (navigatingBack &&
                (actionName == MergePageNames.EpaoSearchResults && type == MergeOrganisationType.Primary && lastCommand.CommandName == SessionCommands.SearchPrimaryEpao
                || actionName == MergePageNames.EpaoSearchResults && type == MergeOrganisationType.Secondary && lastCommand.CommandName == SessionCommands.SearchSecondaryEpao
                || actionName == MergePageNames.ConfirmEpao && type == MergeOrganisationType.Primary && lastCommand.CommandName == SessionCommands.ConfirmPrimaryEpao
                || actionName == MergePageNames.ConfirmEpao && type == MergeOrganisationType.Secondary && lastCommand.CommandName == SessionCommands.ConfirmSecondaryEpao
                || actionName == MergePageNames.SetSecondaryEpaoEffectiveToDate && lastCommand.CommandName == SessionCommands.SetSecondaryEpaoEffectiveTo))
            {
                mergeRequest.DeleteLastCommand();

                sessionService.UpdateMergeRequest(mergeRequest);
            }
        }

    }
}
