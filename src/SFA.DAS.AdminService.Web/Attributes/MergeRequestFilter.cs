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
            var type = context.RouteData.Values.ContainsKey("type") ? context.RouteData.Values["type"].ToString() : "";

            var navigatingBack = context.HttpContext.Request.Query.ContainsKey("back") ? bool.Parse(context.HttpContext.Request.Query["back"]) : false;
            
            var lastCommand = mergeRequest.PreviousCommand;

            if (navigatingBack == true &&
                (actionName == "EpaoSearchResults" && type == "primary" && lastCommand.CommandName == SessionCommands.SearchPrimaryEpao
                || actionName == "EpaoSearchResults" && type == "secondary" && lastCommand.CommandName == SessionCommands.SearchSecondaryEpao
                || actionName == "ConfirmEpao" && type == "primary" && lastCommand.CommandName == SessionCommands.ConfirmPrimaryEpao
                || actionName == "ConfirmEpao" && type == "secondary" && lastCommand.CommandName == SessionCommands.ConfirmSecondaryEpao
                || actionName == "SetSecondaryEpaoEffectiveToDate" && lastCommand.CommandName == SessionCommands.SetSecondaryEpaoEffectiveTo))
            {
                mergeRequest.DeleteLastCommand();

                sessionService.UpdateMergeRequest(mergeRequest);
            }
        }

    }
}
