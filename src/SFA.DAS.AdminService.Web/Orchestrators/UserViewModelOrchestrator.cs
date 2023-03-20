using AutoMapper;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Threading.Tasks;
using static StackExchange.Redis.Role;

namespace SFA.DAS.AdminService.Web.Orchestrators
{
    public class UserViewModelOrchestrator : IUserViewModelOrchestrator
    {
        private readonly IOrganisationsApiClient _organisationsApiClient;
        private readonly IContactsApiClient _contactsApiClient;
        private readonly IMapper _mapper;

        public UserViewModelOrchestrator(IContactsApiClient contactsApiClient, IOrganisationsApiClient organisationsApiClient, IMapper mapper)
        {
            _contactsApiClient = contactsApiClient;
            _organisationsApiClient = organisationsApiClient;
            _mapper = mapper;
        }

        public async Task<RegisterViewAndEditUserViewModel> GetUserViewModel(Guid contactId)
        {
            var contact = await _contactsApiClient.GetById(contactId);
            var organisation = await _organisationsApiClient.Get(contact.OrganisationId.Value);

            var vm = _mapper.Map<RegisterViewAndEditUserViewModel>(contact);
            vm.EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId;
            vm.AssignedPrivileges = await _contactsApiClient.GetContactPrivileges(contact.Id);
            vm.AllPrivilegeTypes = await _contactsApiClient.GetPrivileges();

            return vm;
        }
    }
}
