using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class ContactsApiClient : ApiClientBase, IContactsApiClient
    { 
        public ContactsApiClient() : base()
        {

        }

        public ContactsApiClient(string baseUri, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(baseUri, tokenService, logger)
        {
            
        }

        public ContactsApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }

        public async virtual Task<List<Privilege>> GetPrivileges()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/privileges"))
            {
                return await RequestAndDeserialiseAsync<List<Privilege>>(request, $"Could not privileges");
            }
        }

        public async virtual Task<List<ContactsPrivilege>> GetContactPrivileges(Guid userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/user/{userId}/privileges"))
            {
                return await RequestAndDeserialiseAsync<List<ContactsPrivilege>>(request, $"Could not find the contact");
            }
        }

        public async virtual Task<ContactResponse> GetByUsername(string username)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/username/{WebUtility.UrlEncode(username)}"))
            {
                return await RequestAndDeserialiseAsync<ContactResponse>(request, $"Could not find the contact");
            }
        }

        public async virtual Task<ContactResponse> GetById(Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/user/{id}"))
            {
                return await RequestAndDeserialiseAsync<ContactResponse>(request, $"Could not find the contact");
            }
        }


        public async virtual Task<ContactResponse> Update(UpdateContactRequest updateContactRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/contacts/"))
            {
                return await PostPutRequestWithResponse<UpdateContactRequest, ContactResponse>(request, updateContactRequest);
            }
        }

        public async virtual Task<ContactResponse> UpdateStatus(UpdateContactStatusRequest updateContactStatusRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/contacts/status"))
            {
                return await PostPutRequestWithResponse<UpdateContactStatusRequest, ContactResponse>(request, updateContactStatusRequest);
            }
        }

        public async virtual Task<ContactBoolResponse> DoesContactHavePrivileges(string userId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/user/{userId}/haveprivileges"))
            {
                return await RequestAndDeserialiseAsync<ContactBoolResponse>(request, $"Could not find contact with {userId}");
            }
        }

        public async virtual Task<ContactResponse> GetContactBySignInId(string signInId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/signInId/{signInId}"))
            {
                var result = await RequestAndDeserialiseAsync<ContactResponse>(request, $"Could not find contact with {signInId}");
                return result;
            }
        }

        public async virtual Task<List<ContactResponse>> GetAllContactsForOrganisation(string epaoId, bool? withUser = null)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts/getAll"))
            {
                var response = await PostPutRequestWithResponse<GetAllContactsRequest, List<ContactResponse>>(request,
                        new GetAllContactsRequest(epaoId, withUser));

                return response;
            }
        }

        public async virtual Task<List<ContactIncludePrivilegesResponse>> GetAllContactsForOrganisationIncludePrivileges(string epaoId, bool? withUser = null)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts/getAll/includePrivileges"))
            {
                var response = await PostPutRequestWithResponse<GetAllContactsIncludePrivilegesRequest, List<ContactIncludePrivilegesResponse>>(request,
                        new GetAllContactsIncludePrivilegesRequest(epaoId, withUser));

                return response;
            }
        }

        public async virtual Task<List<ContactResponse>> GetAllContactsWhoCanBePrimaryForOrganisation(string epaoId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/contacts/getAllWhoCanBePrimary/{epaoId}"))
            {
                return await RequestAndDeserialiseAsync<List<ContactResponse>>(request, $"Could not find any contacts who can be primary for organisation {epaoId}");
            }
        }

        public async virtual Task<ContactResponse> UpdateOrgAndStatus(UpdateContactWithOrgAndStausRequest updateContactWithOrgAndStausRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/contacts/updateContactWithOrgAndStatus"))
            {
                return await PostPutRequestWithResponse<UpdateContactWithOrgAndStausRequest, ContactResponse>(request, updateContactWithOrgAndStausRequest);
            }
        }

        public async virtual Task<ContactBoolResponse> InviteUser(CreateContactRequest createContactRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts"))
            {
                var response =
                    await PostPutRequestWithResponse<CreateContactRequest, ContactBoolResponse>(request,
                        createContactRequest);

                return response;
            }
        }

        public async virtual Task Callback(SignInCallback callback)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/contacts/callback"))
            {
                await PostPutRequest(request, callback);
            }
        }

        public async virtual Task MigrateUsers()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/MigrateUsers"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GetToken());
                request.Headers.Add("Accept", "application/json");
                request.Content = new StringContent("", System.Text.Encoding.UTF8, "application/json");

                await HttpClient.SendAsync(request);
            }
        }

        public async virtual Task MigrateSingleContactToApply(System.Guid signinId)
        {
            var signinIdWrapper = new SigninIdWrapper(signinId);
            Logger.LogInformation($"MigrateSingleContactToApply json being POSTed: {JsonConvert.SerializeObject(signinIdWrapper)}");
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/MigrateSingleContactToApply"))
            {
                await PostPutRequest(request, signinIdWrapper);
            }
        }

        public async virtual Task<ContactResponse> CreateANewContactWithGivenId(Contact contact)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/createNewContactWithGivenId"))
            {
                var response =
                     await PostPutRequestWithResponse<Contact, ContactResponse>(request, contact);

                return response;
            }
        }

        public async virtual Task AssociateDefaultRolesAndPrivileges(Contact contact)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/associateDefaultRolesAndPrivileges"))
            {
                await PostPutRequest(request, contact);

            }
        }

        public async virtual Task<SetContactPrivilegesResponse> SetContactPrivileges(SetContactPrivilegesRequest privilegesRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/setContactPrivileges"))
            {
                return await PostPutRequestWithResponse<SetContactPrivilegesRequest, SetContactPrivilegesResponse>(request, privilegesRequest);
            }
        }

        public async virtual Task<RemoveContactFromOrganisationResponse> RemoveContactFromOrganisation(Guid requestingUserId, Guid contactId)
        {
            var removeContactRequest = new RemoveContactFromOrganisationRequest(requestingUserId, contactId);

            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/removeContactFromOrganisation"))
            {
                return await PostPutRequestWithResponse<RemoveContactFromOrganisationRequest, RemoveContactFromOrganisationResponse>(request, removeContactRequest);
            }
        }

        public async virtual Task<InviteContactToOrganisationResponse> InviteContactToOrganisation(InviteContactToOrganisationRequest invitationRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/inviteContactToOrganisation"))
            {
                return await PostPutRequestWithResponse<InviteContactToOrganisationRequest, InviteContactToOrganisationResponse>(request, invitationRequest);
            }
        }

        public async virtual Task RequestForPrivilege(Guid contactId, Guid privilegeId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/requestForPrivilege"))
            {
                await PostPutRequest(request, new RequestForPrivilegeRequest { ContactId = contactId, PrivilegeId = privilegeId });
            }
        }

        public async virtual Task ApproveContact(Guid contactId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/approve"))
            {
                await PostPutRequest(request, new ApproveContactRequest { ContactId = contactId });
            }
        }

        public async virtual Task RejectContact(Guid contactId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/contacts/reject"))
            {
                await PostPutRequest(request, new RejectContactRequest { ContactId = contactId });
            }
        }
    }
}
