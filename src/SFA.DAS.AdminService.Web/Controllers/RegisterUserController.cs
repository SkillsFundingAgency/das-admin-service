using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Orchestrators;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam + "," + Roles.RegisterViewOnlyTeam)]
    public class RegisterUserController : Controller
    {
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IRegisterUserOrchestrator _registerUserOrchestrator;

        public RegisterUserController(IContactsApiClient contactsApiClient, IHttpContextAccessor httpContextAccessor, IOrganisationsApiClient organisationsApiClient, IRegisterUserOrchestrator registerUserOrchestrator)
        {
            _contactsApiClient = contactsApiClient;
            _organisationsApiClient = organisationsApiClient;
            _registerUserOrchestrator = registerUserOrchestrator;
        }

        [HttpGet("register/view-user/{contactId}", Name = "RegisterUserController_Details")]
        public async Task<IActionResult> Details(Guid contactId)
        {
            var vm = await _registerUserOrchestrator.GetUserViewModel(contactId);

            return View("~/Views/Register/ViewUser.cshtml", vm);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/{contactId}/user-permissions", Name = "RegisterUser_EditPermissions")]
        public async Task<IActionResult> EditPermissions(Guid contactId)
        {
            var vm = await _registerUserOrchestrator.GetUserViewModel(contactId);

            return View("~/Views/Register/EditUserPermissions.cshtml", vm);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpPost("register/{contactId}/user-permissions", Name = "RegisterUser_EditPermissions")]
        public async Task<IActionResult> EditPermissions(EditPrivilegesViewModel vm)
        {
            if (vm.Button == "Approve" || vm.Button == "Save")
            {
                if (vm.Button == "Approve")
                {
                    await _contactsApiClient.ApproveContact(vm.ContactId);
                }

                var response = await _contactsApiClient.SetContactPrivileges(
                    new SetContactPrivilegesRequest()
                    {
                        AmendingContactId = Guid.Empty, // amended by a staff member who is not a contact
                        ContactId = vm.ContactId,
                        PrivilegeIds = vm.PrivilegeViewModels.Where(pvm => pvm.Selected).Select(pvm => pvm.Privilege.Id).ToArray(),
                        IsNewContact = vm.Button == "Approve"
                    });

                if (!response.Success)
                {
                    ModelState.AddModelError("permissions", response.ErrorMessage);

                    var editVm = await _registerUserOrchestrator.GetUserViewModel(vm.ContactId);

                    return View("~/Views/Register/EditUserPermissions.cshtml", editVm);
                }

                return RedirectToAction("Details", new { contactId = vm.ContactId });
            }

            await _contactsApiClient.RejectContact(vm.ContactId);
            return RedirectToAction("Register", "ViewOrganisation", new { vm.EndPointAssessorOrganisationId });
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/{contactId}/remove", Name = "RegisterUser_Remove")]
        public async Task<IActionResult> Remove(Guid contactId)
        {
            var contactToBeRemoved = await _contactsApiClient.GetById(contactId);
            return View("~/Views/Register/RemoveConfirm.cshtml", contactToBeRemoved);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpPost("register/{contactId}/remove", Name = "RegisterUser_Remove")]
        public async Task<IActionResult> RemoveConfirmed(Guid contactId)
        {
            var contactToBeRemoved = await _contactsApiClient.GetById(contactId);
            var removedFrom = contactToBeRemoved.OrganisationId;

            var response = await _contactsApiClient.RemoveContactFromOrganisation(Guid.Empty, contactId);

            if (!response.Success)
            {
                ModelState.AddModelError("permissions", response.ErrorMessage);

                return View("~/Views/Register/RemoveConfirm.cshtml", contactToBeRemoved);
            }
            
            return RedirectToAction("Removed", "RegisterUser", new { contactId, organisationId = removedFrom });
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/{contactId}/removedFrom/{organisationId}", Name = "RegisterUser_Removed")]
        public async Task<IActionResult> Removed(Guid contactId, Guid organisationId)
        {
            var contactRemoved = await _contactsApiClient.GetById(contactId);
            var organisation = await _organisationsApiClient.Get(organisationId);

            return View("~/Views/Register/Removed.cshtml", 
                new UserRemovedViewModel
                {
                    ContactEmail = contactRemoved.Email,
                    OrganisationName = organisation.EndPointAssessorName,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId
                });
        }
    }
}