using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AdminService.Web.Domain;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AdminService.Web.Extensions.TagHelpers;
using SFA.DAS.AdminService.Web.ViewModels.Shared;

namespace SFA.DAS.AdminService.Web.Controllers
{
    [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam + "," + Roles.RegisterViewOnlyTeam)]
    [CheckSession(nameof(RegisterController), nameof(ResetSession), nameof(IControllerSession.Register_SessionValid))]
    public class RegisterController: Controller
    {
        private readonly IControllerSession _controllerSession;
        private readonly IApiClient _apiClient;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IStandardServiceClient _standardServiceClient;
        private readonly IHostingEnvironment _env;

        private const int DefaultPageIndex = 1;
        private const int DefaultStandardsPerPage = 2;
        private const int DefaultPageSetSize = 6;

        public RegisterController(IControllerSession controllerSession, IApiClient apiClient, IContactsApiClient contactsApiClient, IStandardServiceClient standardServiceClient,  IHostingEnvironment env)
        {
            _controllerSession = controllerSession;
            _apiClient = apiClient;
            _contactsApiClient = contactsApiClient;
            _standardServiceClient = standardServiceClient;
            _env = env;
        }

        [CheckSession(nameof(IControllerSession.Register_SessionValid), CheckSession.Ignore)]
        public IActionResult ResetSession()
        {
            SetDefaultSession();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Index()
        {            
            return View(); 
        }

        [HttpGet("register/results")]
        public async Task<IActionResult> Results(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("index",vm);
            }

            var searchstring = vm.SearchString?.Trim().ToLower();
            searchstring = string.IsNullOrEmpty(searchstring) ? "" : searchstring;
            var rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
            searchstring = rx.Replace(searchstring, "");
            var searchResults = await _apiClient.SearchOrganisations(searchstring);

            var results = searchResults ?? new List<AssessmentOrganisationSummary>();
            var registerViewModel = new RegisterViewModel
            {
                Results = results,
                SearchString = vm.SearchString
            };

            return View(registerViewModel);
        }


        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/edit-organisation/{organisationId}")]
        public async Task<IActionResult> EditOrganisation(string organisationId)
        {
            var organisation = await _apiClient.GetEpaOrganisation(organisationId);
            var viewModel = await MapOrganisationModel(organisation, false);
            return View(viewModel);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpPost("register/edit-organisation/{organisationId}")]
        public async Task<IActionResult> EditOrganisation(RegisterViewAndEditOrganisationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.OrganisationTypes = await _apiClient.GetOrganisationTypes();
                await GatherOrganisationContacts(viewModel);
                await GatherOrganisationStandards(viewModel, false);
                return View(viewModel);
            }

            var updateOrganisationRequest = new UpdateEpaOrganisationRequest
            {
                Name = viewModel.Name,
                OrganisationId = viewModel.OrganisationId,
                Ukprn = viewModel.Ukprn,
                OrganisationTypeId = viewModel.OrganisationTypeId,
                LegalName = viewModel.LegalName,
                TradingName = viewModel.TradingName,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber,
                WebsiteLink = viewModel.WebsiteLink,
                Address1 = viewModel.Address1,
                Address2 = viewModel.Address2,
                Address3 = viewModel.Address3,
                Address4 = viewModel.Address4,
                Postcode = viewModel.Postcode,
                Status = viewModel.Status,
                ActionChoice = viewModel.ActionChoice,
                CompanyNumber = viewModel.CompanyNumber,
                CharityNumber = viewModel.CharityNumber,
                FinancialDueDate = viewModel.FinancialDueDate,
                FinancialExempt = viewModel.FinancialExempt
            };
         
            await _apiClient.UpdateEpaOrganisation(updateOrganisationRequest);
         
            return RedirectToAction("ViewOrganisation", "register", new { organisationId = viewModel.OrganisationId});
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/add-organisation")]
        public async Task<IActionResult> AddOrganisation()
        {
            var vm = new RegisterOrganisationViewModel
            {
                OrganisationTypes = await _apiClient.GetOrganisationTypes()
            };

            return View(vm);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/add-standard/organisation/{organisationId}/standard/{standardId}")]
        public async Task<IActionResult> AddOrganisationStandard(string organisationId, int standardId)
       {
           var viewModelToHydrate =
               new RegisterAddOrganisationStandardViewModel {OrganisationId = organisationId, StandardId = standardId};
           var vm = await ConstructOrganisationAndStandardDetails(viewModelToHydrate);

           return View(vm);
       }


        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpPost("register/add-standard/organisation/{organisationId}/standard/{standardId}")]
        public async Task<IActionResult> AddOrganisationStandard(RegisterAddOrganisationStandardViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var viewModelInvalid = await ConstructOrganisationAndStandardDetails(viewModel);
                return View(viewModelInvalid);
            }                   

            var addOrganisationStandardRequest = new CreateEpaOrganisationStandardRequest
            {
                OrganisationId = viewModel.OrganisationId,
               StandardCode = viewModel.StandardId,
               EffectiveFrom = viewModel.EffectiveFrom,
               EffectiveTo = viewModel.EffectiveTo,
               ContactId = viewModel.ContactId.ToString(),
               DeliveryAreas = viewModel.DeliveryAreas,
               Comments = viewModel.Comments,
               DeliveryAreasComments = viewModel.DeliveryAreasComments
            };

            var organisationStandardId = await _apiClient.CreateEpaOrganisationStandard(addOrganisationStandardRequest);
            return Redirect($"/register/view-standard/{organisationStandardId}");
        }

        [HttpGet("register/view-standard/{organisationStandardId}", Name="Register_ViewStandard")]
        public async Task<IActionResult> ViewStandard(int organisationStandardId)
        {
            var organisationStandard = await _apiClient.GetOrganisationStandard(organisationStandardId);


            var viewModel =
                MapOrganisationStandardToViewModel(organisationStandard);

            return View(viewModel);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/edit-standard/{organisationStandardId}")]
        public async Task<IActionResult> EditOrganisationStandard(int organisationStandardId)
        {
            var organisationStandard = await _apiClient.GetOrganisationStandard(organisationStandardId);
            var viewModel =
                MapOrganisationStandardToViewModel(organisationStandard);
            var vm = await AddContactsAndDeliveryAreasAndDateDetails(viewModel);
            return View(vm);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpPost("register/edit-standard/{organisationStandardId}")]
        public async Task<IActionResult> EditOrganisationStandard(RegisterViewAndEditOrganisationStandardViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var viewModelInvalid = await AddContactsAndDeliveryAreasAndDateDetails(viewModel);
                return View(viewModelInvalid);
            }

            var updateOrganisationStandardRequest = new UpdateEpaOrganisationStandardRequest
            {
                OrganisationId = viewModel.OrganisationId,
                StandardCode = viewModel.StandardId,
                EffectiveFrom = viewModel.EffectiveFrom,
                EffectiveTo = viewModel.EffectiveTo,
                ContactId = viewModel.ContactId.ToString(),
                DeliveryAreas = viewModel.DeliveryAreas,
                Comments = viewModel.Comments,
                DeliveryAreasComments = viewModel.DeliveryAreasComments
            };

            var organisationStandardId = await _apiClient.UpdateEpaOrganisationStandard(updateOrganisationStandardRequest);
            return Redirect($"/register/view-standard/{organisationStandardId}");
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/set-status-user/{organisationId}/{contactId}/{status}", Name= "Register_SetStatusAndNotify")]
        public async Task<IActionResult> SetStatusAndNotify(string organisationId, Guid contactId, string status)
        {
            if (status == ContactStatus.Approve)
            {
                await _contactsApiClient.ApproveContact(contactId);
            }
            else
            {
                await _contactsApiClient.RejectContact(contactId);
            }

            return RedirectToAction(nameof(ViewOrganisation), nameof(RegisterController).RemoveController(), new { organisationId });
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/add-contact/{organisationId}")]
        public async Task<IActionResult> AddContact(string organisationId)
        {
            var vm = new RegisterAddContactViewModel
            {
                EndPointAssessorOrganisationId = organisationId
            };

            return View(vm);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpPost("register/add-contact/{organisationId}")]
        public async Task<IActionResult> AddContact(RegisterAddContactViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {      
                return View(viewModel);
            }

            var addContactRequest = new CreateEpaOrganisationContactRequest
            {
                EndPointAssessorOrganisationId = viewModel.EndPointAssessorOrganisationId,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber
            };

            var contactId = await _apiClient.CreateEpaContact(addContactRequest);
            return Redirect($"/register/view-contact/{contactId}");
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/edit-contact/{contactId}")]
        public async Task<IActionResult> EditContact(string contactId)
        {
            var contact = await _apiClient.GetEpaContact(contactId);
            var organisation = await _apiClient.GetEpaOrganisation(contact.OrganisationId);
            var viewModel = MapContactModel(contact, organisation);
            return View(viewModel);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpPost("register/edit-contact/{contactId}")]
        public async Task<IActionResult> EditContact(RegisterViewAndEditContactViewModel viewAndEditModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewAndEditModel);
            }

            var request = new UpdateEpaOrganisationContactRequest
            {
                ContactId = viewAndEditModel.ContactId,
                FirstName = viewAndEditModel.FirstName,
                LastName = viewAndEditModel.LastName,
                Email = viewAndEditModel.Email,
                PhoneNumber = viewAndEditModel.PhoneNumber,
                ActionChoice = viewAndEditModel.ActionChoice
            };
            await _apiClient.UpdateEpaContact(request);
            return RedirectToAction("ViewContact", "register", new { contactId = viewAndEditModel.ContactId});
        }

        [HttpGet("register/view-contact/{contactId}")]
        public async Task<IActionResult> ViewContact(string contactId)
        {
            var contact = await _apiClient.GetEpaContact(contactId);
            var organisation = await _apiClient.GetEpaOrganisation(contact.OrganisationId);
            var viewModel = MapContactModel(contact, organisation);
            return View(viewModel);
        }


        [HttpGet("register/impage")]
        public async Task<IActionResult> Impage()
        {
            if (!_env.IsDevelopment())
                return NotFound();
           
            var vm = new AssessmentOrgsImportResponse { Status = "Press to run" };         
            return View(vm);
        }
        [HttpGet("register/impage-{choice}")]
        public async Task<IActionResult> Impage(string choice)
        {
            if (!_env.IsDevelopment())
                return NotFound();

            var vm = new AssessmentOrgsImportResponse { Status = "Running" };
            if (choice == "DoIt")
            {
                var importResults = await _apiClient.ImportOrganisations();
                vm.Status = importResults;
            }
            return View(vm);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpPost("register/add-organisation")]
        public async Task<IActionResult> AddOrganisation(RegisterOrganisationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.OrganisationTypes = await _apiClient.GetOrganisationTypes();             
                return View(viewModel);
            }

            var addOrganisationRequest = new CreateEpaOrganisationRequest
            {
                Name = viewModel.Name,
                Ukprn = viewModel.Ukprn,
                OrganisationTypeId = viewModel.OrganisationTypeId,
                LegalName = viewModel.LegalName,
                TradingName = viewModel.TradingName,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber,
                WebsiteLink = viewModel.WebsiteLink,
                Address1 = viewModel.Address1,
                Address2 = viewModel.Address2,
                Address3 = viewModel.Address3,
                Address4 = viewModel.Address4,
                Postcode = viewModel.Postcode,
                CompanyNumber = viewModel.CompanyNumber,
                CharityNumber = viewModel.CharityNumber
            };

            var organisationId = await _apiClient.CreateEpaOrganisation(addOrganisationRequest);
            return RedirectToAction("ViewOrganisation", "register",new { organisationId });
        }

        [HttpGet("register/view-organisation/{organisationId}", Name = "Register_ViewOrganisation")]
        public async Task<IActionResult> ViewOrganisation(string organisationId)
        {
            var organisation = await _apiClient.GetEpaOrganisation(organisationId);
            var viewModel = await MapOrganisationModel(organisation, true);     
            return View(viewModel);
        }

        [HttpGet("register/view-organisation-approved-standards/{organisationId}/page-index/{pageIndex}", Name = "Register_ChangePageViewOrganisationApprovedStandards")]
        public IActionResult ChangePageViewOrganisationApprovedStandards(string organisationId, int pageIndex = DefaultPageIndex)
        {
            _controllerSession.Register_ApprovedStandards.PageIndex = pageIndex;
            return RedirectToAction(nameof(ViewOrganisation), new { organisationId });
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.Register_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangePageViewOrganisationApprovedStandardsPartial(string organisationId, int pageIndex)
        {
            _controllerSession.Register_ApprovedStandards.PageIndex = pageIndex;
            var viewModel = new OrganisationStandardsViewModel
            {
                OrganisationId = organisationId,
                PaginationViewModel = await GatherOrganisationStandards(organisationId, true)
            };
            return PartialView("_OrganisationStandardsPartial", viewModel);
        }

        [HttpGet]
        public IActionResult ChangeStandardsPerPageViewOrganisationApprovedStandards(string organisationId, int itemsPerPage = DefaultStandardsPerPage)
        {
            _controllerSession.Register_ApprovedStandards.ItemsPerPage = itemsPerPage;
            return RedirectToAction(nameof(ChangePageViewOrganisationApprovedStandards), new { organisationId, pageIndex = 1 });
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.Register_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> ChangeStandardsPerPageViewOrganisationApprovedStandardsPartial(string organisationId, int itemsPerPage)
        {
            _controllerSession.Register_ApprovedStandards.ItemsPerPage = itemsPerPage;
            _controllerSession.Register_ApprovedStandards.PageIndex = 1;
            var viewModel = new OrganisationStandardsViewModel
            {
                OrganisationId = organisationId,
                PaginationViewModel = await GatherOrganisationStandards(organisationId, true)
            };
            return PartialView("_OrganisationStandardsPartial", viewModel);
        }

        [HttpGet]
        public IActionResult SortViewOrganisationApprovedStandards(string organisationId, string sortColumn, string sortDirection)
        {
            UpdateSortDirection(sortColumn, sortDirection, _controllerSession.Register_ApprovedStandards);
            return RedirectToAction(nameof(ChangePageViewOrganisationApprovedStandards), new { organisationId, pageIndex = 1 });
        }

        [HttpGet]
        [CheckSession(nameof(IControllerSession.Register_SessionValid), CheckSession.Error)]
        public async Task<IActionResult> SortViewOrganisationApprovedStandardsPartial(string organisationId, string sortColumn, string sortDirection)
        {
            UpdateSortDirection(sortColumn, sortDirection, _controllerSession.Register_ApprovedStandards);
            _controllerSession.Register_ApprovedStandards.PageIndex = 1;
            var viewModel = new OrganisationStandardsViewModel
            {
                OrganisationId = organisationId,
                PaginationViewModel = await GatherOrganisationStandards(organisationId, true)
            };
            return PartialView("_OrganisationStandardsPartial", viewModel);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/search-standards/{organisationId}")]
        public async Task<IActionResult> SearchStandards(string organisationId)
        {
            var organisation = await _apiClient.GetEpaOrganisation(organisationId);
            var vm = new SearchStandardsViewModel{OrganisationId = organisationId, OrganisationName = organisation.Name};
            
            return View(vm);
        }

        [Authorize(Roles = Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
        [HttpGet("register/search-standards-results")]
        public async Task<IActionResult> SearchStandardsResults(SearchStandardsViewModel vm)
        {
            var organisation = await _apiClient.GetEpaOrganisation(vm.OrganisationId);
            vm.OrganisationName = organisation.Name;
            if (!ModelState.IsValid)
            {
                return View("SearchStandards", vm);
            }

            var searchstring = vm.StandardSearchString?.Trim().ToLower();
            searchstring = string.IsNullOrEmpty(searchstring) ? "" : searchstring;
            var rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
            searchstring = rx.Replace(searchstring, "");
            searchstring = searchstring.Replace("/", "");
            var searchResults = await _apiClient.SearchStandards(searchstring);

            var standardViewModel = new SearchStandardsViewModel
            {
                Results = searchResults,
                StandardSearchString = vm.StandardSearchString,
                OrganisationId = vm.OrganisationId,
                OrganisationName = vm.OrganisationName
            };

            return View(standardViewModel);
        }

        private async Task<RegisterAddOrganisationStandardViewModel> ConstructOrganisationAndStandardDetails(RegisterAddOrganisationStandardViewModel vm)
        {
            var organisation = await _apiClient.GetEpaOrganisation(vm.OrganisationId);
            var standard = await _standardServiceClient.GetStandard(vm.StandardId);
            var availableDeliveryAreas = await _apiClient.GetDeliveryAreas();

            vm.Contacts = await _contactsApiClient.GetAllContactsForOrganisation(vm.OrganisationId);

            vm.OrganisationName = organisation.Name;
            vm.Ukprn = organisation.Ukprn;
            vm.StandardTitle = standard.Title;
            vm.StandardEffectiveFrom = standard.StandardData.EffectiveFrom;
            vm.StandardEffectiveTo = standard.StandardData.EffectiveTo;
            vm.StandardLastDateForNewStarts = standard.StandardData.LastDateForNewStarts;
            vm.AvailableDeliveryAreas = availableDeliveryAreas;
            vm.DeliveryAreas = vm.DeliveryAreas ?? new List<int>();
            vm.OrganisationStatus = organisation.Status;
            return vm;
        }


        private async Task<RegisterViewAndEditOrganisationStandardViewModel> AddContactsAndDeliveryAreasAndDateDetails(RegisterViewAndEditOrganisationStandardViewModel vm)
        {
            var availableDeliveryAreas = await _apiClient.GetDeliveryAreas();

            vm.Contacts = await _apiClient.GetEpaOrganisationContacts(vm.OrganisationId);
            vm.AvailableDeliveryAreas = availableDeliveryAreas;
            vm.DeliveryAreas = vm.DeliveryAreas ?? new List<int>();
            if (vm.EffectiveFrom.HasValue)
            {
                var effectiveFrom = vm.EffectiveFrom.Value;
                vm.EffectiveFromDay = effectiveFrom.Day.ToString();
                vm.EffectiveFromMonth = effectiveFrom.Month.ToString();
                vm.EffectiveFromYear = effectiveFrom.Year.ToString();
            }

            if (vm.EffectiveTo.HasValue)
            {
                var effectiveTo = vm.EffectiveTo.Value;
                vm.EffectiveToDay = effectiveTo.Day.ToString();
                vm.EffectiveToMonth = effectiveTo.Month.ToString();
                vm.EffectiveToYear = effectiveTo.Year.ToString();
            }

            return vm;
        }

        private async Task GatherOrganisationStandards(RegisterViewAndEditOrganisationViewModel viewAndEditModel, bool paged = true)
        {
            var organisationStandards = await _apiClient.GetEpaOrganisationStandards(viewAndEditModel.OrganisationId);
            if (organisationStandards != null)
            {
                viewAndEditModel.RegisterViewOrganisationStandardsViewModel = new RegisterViewOrganisationStandardsViewModel(nameof(RegisterController).RemoveController());
                viewAndEditModel.RegisterViewOrganisationStandardsViewModel.OrganisationStandards.OrganisationId = viewAndEditModel.OrganisationId;
                viewAndEditModel.RegisterViewOrganisationStandardsViewModel.OrganisationStandards.PaginationViewModel = await GatherOrganisationStandards(viewAndEditModel.OrganisationId, paged);
            }
        }

        private async Task<PaginationViewModel<OrganisationStandardSummary>> GatherOrganisationStandards(string organisationId, bool paged)
        {
            var organisationStandards = await _apiClient.GetEpaOrganisationStandards(organisationId);
            if (organisationStandards != null)
            {
                var pageOfOrganisationStandards = paged
                    ? SortOrganisationStandardSummary(organisationStandards)
                    .Skip((_controllerSession.Register_ApprovedStandards.PageIndex - 1) * _controllerSession.Register_ApprovedStandards.ItemsPerPage)
                    .Take(_controllerSession.Register_ApprovedStandards.ItemsPerPage)
                    .ToList()
                    : organisationStandards;

                return new PaginationViewModel<OrganisationStandardSummary>
                    {
                        PaginatedList = new PaginatedList<OrganisationStandardSummary>(
                            pageOfOrganisationStandards,
                            organisationStandards.Count,
                            _controllerSession.Register_ApprovedStandards.PageIndex,
                            _controllerSession.Register_ApprovedStandards.ItemsPerPage,
                            DefaultPageSetSize),
                        PageIndex = _controllerSession.Register_ApprovedStandards.PageIndex,
                        ItemsPerPage = _controllerSession.Register_ApprovedStandards.ItemsPerPage,
                        Fragment = "approved",
                        SortColumn = _controllerSession.Register_ApprovedStandards.SortColumn,
                        SortDirection = _controllerSession.Register_ApprovedStandards.SortDirection,
                        ChangePageAction = nameof(ChangePageViewOrganisationApprovedStandards),
                        ChangeItemsPerPageAction = nameof(ChangeStandardsPerPageViewOrganisationApprovedStandards),
                        SortColumnAction = nameof(SortViewOrganisationApprovedStandards)
                    };
            }

            return null;
        }

        private void UpdateSortDirection(string sortColumn, string sortDirection, IPagingState pagingState)
        {
            if (pagingState.SortColumn == sortColumn)
            {
                pagingState.SortDirection = sortDirection;
            }
            else
            {
                pagingState.SortColumn = sortColumn;
                pagingState.SortDirection = SortOrder.Asc;
            }
        }

        private List<OrganisationStandardSummary> SortOrganisationStandardSummary(List<OrganisationStandardSummary> organisationStandards)
        {
            string sortPropertyName = string.Empty;
            switch (_controllerSession.Register_ApprovedStandards.SortColumn)
            {
                case OrganisationStandardSortColumn.StandardName:
                    sortPropertyName = $"{nameof(OrganisationStandardSummary.StandardCollation)}.{nameof(OrganisationStandardSummary.StandardCollation.Title)}";
                    break;
                case OrganisationStandardSortColumn.StandardCode:
                    sortPropertyName = $"{nameof(OrganisationStandardSummary.StandardCollation)}.{nameof(OrganisationStandardSummary.StandardCollation.ReferenceNumber)}";
                    break;
                case OrganisationStandardSortColumn.DateApproved:
                    sortPropertyName = $"{nameof(OrganisationStandardSummary.DateStandardApprovedOnRegister)}";
                    break;
            }

            if(_controllerSession.Register_ApprovedStandards.SortDirection == SortOrder.Asc)
            {
                return organisationStandards
                    .AsQueryable()
                    .OrderBy(sortPropertyName)
                    .ToList();
            }
            else
            {
                return organisationStandards
                    .AsQueryable()
                    .OrderByDescending(sortPropertyName)
                    .ToList();
            }
        }

        private RegisterViewAndEditContactViewModel MapContactModel(AssessmentOrganisationContact contact, EpaOrganisation organisation)
        {
            var viewModel = new RegisterViewAndEditContactViewModel
            {
                Email = contact.Email,
                ContactId = contact.Id.ToString(),
                PhoneNumber = contact.PhoneNumber,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                OrganisationName = organisation.Name,
                OrganisationId = organisation.OrganisationId,
                IsPrimaryContact = contact.IsPrimaryContact
            };

            return viewModel;
        }

        private async Task GatherOrganisationContacts(RegisterViewAndEditOrganisationViewModel viewAndEditModel)
        {
            viewAndEditModel.Contacts = await _contactsApiClient.GetAllContactsForOrganisation(viewAndEditModel.OrganisationId, false);
            viewAndEditModel.Users = await _contactsApiClient.GetAllContactsForOrganisationIncludePrivileges(viewAndEditModel.OrganisationId, true);

            var contactsWhoCanBePrimary = await _contactsApiClient.GetAllContactsWhoCanBePrimaryForOrganisation(viewAndEditModel.OrganisationId);
            if (viewAndEditModel.PrimaryContact != null && contactsWhoCanBePrimary.Any(x => x.Username == viewAndEditModel.PrimaryContact))
            {
                var primaryContact = contactsWhoCanBePrimary.First(x => x.Username == viewAndEditModel.PrimaryContact);
                viewAndEditModel.PrimaryContactName = primaryContact.DisplayName;
                if (primaryContact.Username != null)
                {
                    viewAndEditModel.PrimaryContactName = $"{viewAndEditModel.PrimaryContactName} ({primaryContact.Username})";
                }
            }
        }

        private void SetDefaultSession()
        {
            _controllerSession.Register_SessionValid = true;

            _controllerSession.Register_ApprovedStandards.PageIndex = DefaultPageIndex;
            _controllerSession.Register_ApprovedStandards.ItemsPerPage = DefaultStandardsPerPage;
            _controllerSession.Register_ApprovedStandards.SortColumn = OrganisationStandardSortColumn.StandardName;
            _controllerSession.Register_ApprovedStandards.SortDirection = SortOrder.Desc;
        }

        private async Task<RegisterViewAndEditOrganisationViewModel> MapOrganisationModel(EpaOrganisation organisation, bool pageStandards)
        {
            var notSetDescription = "Not set";
            var viewModel = new RegisterViewAndEditOrganisationViewModel
            {
                Id = organisation.Id,
                OrganisationId = organisation.OrganisationId,
                Name = organisation.Name,
                Ukprn = organisation.Ukprn,
                OrganisationTypeId = organisation.OrganisationTypeId,
                OrganisationType = notSetDescription,
                LegalName = organisation.OrganisationData?.LegalName,
                TradingName = organisation.OrganisationData?.TradingName,
                Email = organisation.OrganisationData?.Email,
                PhoneNumber = organisation.OrganisationData?.PhoneNumber,
                WebsiteLink = organisation.OrganisationData?.WebsiteLink,
                Address1 = organisation.OrganisationData?.Address1,
                Address2 = organisation.OrganisationData?.Address2,
                Address3 = organisation.OrganisationData?.Address3,
                Address4 = organisation.OrganisationData?.Address4,
                Postcode = organisation.OrganisationData?.Postcode,
                PrimaryContact = organisation.PrimaryContact,
                PrimaryContactName = organisation.PrimaryContactName ?? notSetDescription,
                CharityNumber = organisation.OrganisationData?.CharityNumber,
                CompanyNumber =  organisation.OrganisationData?.CompanyNumber,
                Status = organisation.Status,
                FinancialDueDate = organisation.OrganisationData?.FHADetails?.FinancialDueDate,
                FinancialExempt = organisation.OrganisationData?.FHADetails?.FinancialExempt
            };

            viewModel.OrganisationTypes = _apiClient.GetOrganisationTypes().Result;

            if (viewModel.OrganisationTypeId != null)
            {
                var organisationTypes = viewModel.OrganisationTypes;
                viewModel.OrganisationType = organisationTypes.FirstOrDefault(x => x.Id == viewModel.OrganisationTypeId)?.Type;
            }
               
            await GatherOrganisationContacts(viewModel);
            await GatherOrganisationStandards(viewModel, pageStandards);

            return viewModel;
        }

        private static RegisterViewAndEditOrganisationStandardViewModel MapOrganisationStandardToViewModel(OrganisationStandard organisationStandard)
        {
            return new RegisterViewAndEditOrganisationStandardViewModel
            {
                OrganisationStandardId = organisationStandard.Id,
                StandardId = organisationStandard.StandardId,
                StandardTitle = organisationStandard.StandardTitle,
                OrganisationId = organisationStandard.OrganisationId,
                Ukprn = organisationStandard.Ukprn,
                EffectiveFrom = organisationStandard.EffectiveFrom,
                EffectiveTo = organisationStandard.EffectiveTo,
                DateStandardApprovedOnRegister = organisationStandard.DateStandardApprovedOnRegister,
                StandardEffectiveFrom = organisationStandard.StandardEffectiveFrom,
                StandardEffectiveTo = organisationStandard.StandardEffectiveTo,
                StandardLastDateForNewStarts = organisationStandard.StandardLastDateForNewStarts,
                Comments = organisationStandard.Comments,
                Status = organisationStandard.Status,
                ContactId = organisationStandard.ContactId,
                Contact = organisationStandard.Contact,
                DeliveryAreas = organisationStandard.DeliveryAreas,
                OrganisationName = organisationStandard.OrganisationName,
                OrganisationStatus = organisationStandard.OrganisationStatus,
                DeliveryAreasDetails = organisationStandard.DeliveryAreasDetails,
                DeliveryAreasComments = organisationStandard.OrganisationStandardData?.DeliveryAreasComments
            };
        }
    }
}
