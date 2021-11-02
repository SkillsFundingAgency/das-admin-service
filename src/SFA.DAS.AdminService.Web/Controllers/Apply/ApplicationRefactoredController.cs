using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Web.Commands.ApproveStandardApplication;
using SFA.DAS.AdminService.Web.Commands.ReturnApplicationSequence;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Queries.GetSequenceAndSections;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Linq;
using System.Threading.Tasks;
using ApplicationModel = SFA.DAS.AdminService.Web.Models.Apply.Application;

namespace SFA.DAS.AdminService.Web.Controllers.Apply
{
    public class ApplicationRefactoredController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMediator _mediator;

        public ApplicationRefactoredController(IHttpContextAccessor contextAccessor, IMediator mediator)
        {
            _contextAccessor = contextAccessor;
            _mediator = mediator;
        }

        [HttpPost("/Applications/{applicationId}/{backAction}/{backController}/Sequence/{sequenceNo}/Return/{backOrganisationId?}")]
        public async Task<IActionResult> ReturnRefactored(Guid applicationId, int sequenceNo, string returnType, BackViewModel backViewModel)
        {
            try
            {
                var response = await _mediator.Send(new ApproveStandardApplicationCommand(applicationId, returnType, sequenceNo));

                if (response.ErrorMessages.Count > 0)
                {
                    ModelState.AddErrorMessages(response.ErrorMessages);

                    var qnaData = await _mediator.Send(new GetSequenceAndSectionsQuery(applicationId, response.Application.ActiveSequence.SequenceId));

                    var viewModel = new ApplicationSequenceAssessmentViewModel(Mapper.Map<ApplicationModel, ApplicationResponse>(response.Application), qnaData.Sequence, qnaData.Sections, 
                        backViewModel.BackAction, backViewModel.BackController, backViewModel.BackOrganisationId);
                    
                    return View(nameof(ApplicationController.Assessment), viewModel);
                }

                if (!response.WarningMessages.Any())
                {
                    var username = _contextAccessor.HttpContext.User.UserDisplayName();

                    await _mediator.Send(new ReturnApplicationSequenceCommand(applicationId, sequenceNo, returnType, username));
                }

                var returnedViewModel = new ApplicationReturnedViewModel(sequenceNo, response.StandardDescription, returnType, response.EndPointAssessorName,
                    response.Versions, response.WarningMessages, backViewModel.BackAction, backViewModel.BackController, backViewModel.BackOrganisationId);

                return View("Returned", returnedViewModel);
            }
            catch (InvalidOperationException e)
            {
                return RedirectToApplicationsFromSequence(sequenceNo);
            }
        }

        private IActionResult RedirectToApplicationsFromSequence(int sequenceNo)
        {
            return RedirectToAction(
                    sequenceNo == ApplyConst.STANDARD_SEQUENCE_NO
                        ? nameof(StandardApplicationController.StandardApplications)
                        : nameof(OrganisationApplicationController.OrganisationApplications),
                    sequenceNo == ApplyConst.STANDARD_SEQUENCE_NO
                        ? nameof(StandardApplicationController).RemoveController()
                        : nameof(OrganisationApplicationController).RemoveController());
        }
    }
}
