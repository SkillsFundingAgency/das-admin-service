using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AdminService.Application.Interfaces;
using SFA.DAS.AdminService.Application.Interfaces.Validation;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AdminService.Web.Resources;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Api.Types.Models.Register;

namespace SFA.DAS.AdminService.Web.Services
{
    public class AnswerInjectionService : IAnswerInjectionService
    {
        private readonly IApiClient _apiClient;

        private readonly IValidationService _validationService;
        private readonly IAssessorValidationService _assessorValidationService;

        private readonly ILogger<AnswerService> _logger;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public AnswerInjectionService(IApiClient apiClient, IValidationService validationService, IAssessorValidationService assessorValidationService,
            ISpecialCharacterCleanserService cleanser, ILogger<AnswerService> logger)
        {
            _apiClient = apiClient;
            _validationService = validationService;
            _assessorValidationService = assessorValidationService;
            _cleanser = cleanser;
            _logger = logger;
        }


        public async Task<CreateOrganisationAndContactFromApplyResponse>
            InjectApplyOrganisationAndContactDetailsIntoRegister(CreateOrganisationContactCommand command)
        {
            var response = new CreateOrganisationAndContactFromApplyResponse { IsEpaoApproved = false, ApplySourceIsEpao = false, WarningMessages = new List<string>() };

            if ("RoEPAO".Equals(command.OrganisationReferenceType, StringComparison.InvariantCultureIgnoreCase))
            {
                await UpdateFinancialDetails(command);
                _logger.LogInformation("Source reference type is EPAO. No need to inject organisation details into register");
                response.ApplySourceIsEpao = true;
                return response;
            }
            else if (command.IsEpaoApproved is true)
            {
                await UpdateFinancialDetails(command);
                _logger.LogInformation("Source is RoEPAO approved. No need to inject organisation details into register");
                response.IsEpaoApproved = true;
                return response;
            }

            var warningMessages = new List<string>();
            var organisationName = DecideOrganisationName(command.UseTradingName, command.TradingName, command.OrganisationName);
            var ukprnAsLong = GetUkprnFromRequestDetails(command.OrganisationUkprn, command.CompanyUkprn);
            var organisationTypeId = await GetOrganisationTypeIdFromDescriptor(command.OrganisationType);

            // Organisation checks ////////////////////////////////
            RaiseWarningIfNoOrganisationName(organisationName, warningMessages);
            RaiseWarningIfOrganisationNameTooShort(organisationName, warningMessages);
            RaiseWarningOrganisationTypeNotIdentified(organisationTypeId, warningMessages);
            RaiseWarningIfUkprnIsInvalid(ukprnAsLong, warningMessages);
            RaiseWarningIfCompanyNumberIsInvalid(command.CompanyNumber, warningMessages);
            RaiseWarningIfCharityNumberIsInvalid(command.CharityNumber, warningMessages);

            // Contact checks //////////////////////////////// 
            RaiseWarningIfEmailIsMissingOrInvalid(command.ContactEmail, warningMessages);
            //Removed since some ambiguity will exist in users having contact names in some previously started applications
            //and givename familyname in new applications
            //RaiseWarningIfContactNameIsMissingOrTooShort(command.ContactName, warningMessages);

            var organisation = MapCommandToOrganisationRequest(command, organisationName, ukprnAsLong, organisationTypeId);

            // Validate new org request before processing it
            if (warningMessages.Count == 0)
            {
                var validationResponse = await _assessorValidationService.ValidateNewOrganisationRequest(organisation);

                if (!validationResponse.IsValid)
                {
                    warningMessages.AddRange(validationResponse.Errors.Select(err => err.ErrorMessage));
                }
            }

            // Now if everything has checked out, create it
            if (warningMessages.Count == 0)
            {
                _logger.LogInformation($"Creating a new epa organisation {organisation?.Name}");
                var newOrganisationId = await _apiClient.CreateEpaOrganisation(organisation);
                response.OrganisationId = newOrganisationId;

                _logger.LogInformation($"Assigning the primary contact");
                var primaryContact = MapCommandToContactRequest(command.ContactEmail, newOrganisationId, command.ContactPhoneNumber, command.ContactGivenName, command.ContactFamilyName);
                await AssignPrimaryContactToOrganisation(primaryContact, newOrganisationId);

                _logger.LogInformation($"Assign the applying user default permissions");
                await AssignApplyingContactToOrganisation(command.UserEmail, newOrganisationId);

                _logger.LogInformation($"Inviting other applying users");
                await InviteOtherApplyingUsersToOrganisation(command.OtherApplyingUserEmails, newOrganisationId);
            }
            else
            {
                _logger.LogWarning($"Source has invalid data. Cannot inject organisation details into register at this time. Warnings:  {string.Join(",", warningMessages)}");
            }

            response.WarningMessages = warningMessages;

            return response;
        }

        private async Task AssignPrimaryContactToOrganisation(CreateEpaOrganisationContactRequest primaryContact, string organisationId)
        {
            if (primaryContact != null && !string.IsNullOrEmpty(organisationId))
            {
                var primaryContactId = Guid.Empty;

                var assessorContact = await _apiClient.GetEpaContactByEmail(primaryContact.Email);

                if (assessorContact is null)
                {
                    _logger.LogInformation($"Creating a new primary contact ({primaryContact.Email}) for {organisationId}");
                    var validationResponse = await _assessorValidationService.ValidateNewContactRequest(primaryContact);

                    if (!validationResponse.IsValid)
                    {
                        _logger.LogWarning($"Cannot create new primary contact in assessor for {organisationId}. Validation errors: {validationResponse.Errors.Select(err => err.ErrorMessage)}");
                    }
                    else
                    {
                        //Create a new contact in assessor table, 
                        //Assumption is that this user will need to have an account created in aslogon too  
                        //And then when they login the signinid etc wll get populated as it does for existing users
                        var id = await _apiClient.CreateEpaContact(primaryContact);
                        if (Guid.TryParse(id, out primaryContactId))
                        {
                            _logger.LogInformation($"Contact created successfully - {primaryContactId}");
                        }
                    }
                }
                else
                {
                    _logger.LogInformation($"Primary contact ({primaryContact.Email}) already exists");
                    primaryContactId = assessorContact.Id;
                }

                if (primaryContactId != Guid.Empty)
                {
                    _logger.LogInformation($"Associating primary contact ({primaryContact.Email}) to organisation {organisationId}");
                    var request = new AssociateEpaOrganisationWithEpaContactRequest
                    {
                        ContactId = primaryContactId,
                        OrganisationId = organisationId,
                        ContactStatus = ContactStatus.Live,
                        MakePrimaryContact = true,
                        AddDefaultRoles = true,
                        AddDefaultPrivileges = false
                    };

                    await _apiClient.AssociateOrganisationWithEpaContact(request);
                }
            }
        }

        private async Task AssignApplyingContactToOrganisation(string applyingUserEmail, string organisationId)
        {
            if (!string.IsNullOrEmpty(applyingUserEmail) && !string.IsNullOrEmpty(organisationId))
            {
                var applyingContact = await _apiClient.GetEpaContactByEmail(applyingUserEmail);

                if (applyingContact != null)
                {
                    _logger.LogInformation($"Associating applying contact ({applyingContact.Email}) to organisation {organisationId} with default privileges ");
                    var request = new AssociateEpaOrganisationWithEpaContactRequest
                    {
                        ContactId = applyingContact.Id,
                        OrganisationId = organisationId,
                        ContactStatus = ContactStatus.Live,
                        MakePrimaryContact = false,
                        AddDefaultRoles = true,
                        AddDefaultPrivileges = true
                    };

                    await _apiClient.AssociateOrganisationWithEpaContact(request);
                }
            }
        }

        private async Task InviteOtherApplyingUsersToOrganisation(List<string> otherApplyingUsersEmails, string organisationId)
        {
            if (otherApplyingUsersEmails != null && !string.IsNullOrEmpty(organisationId))
            {
                // For any other user who was trying to apply for the same organisation; they now need to request access
                foreach (var email in otherApplyingUsersEmails)
                {
                    var otherApplyingContact = await _apiClient.GetEpaContactByEmail(email);
                    if (otherApplyingContact != null)
                    {
                        _logger.LogInformation($"Inviting contact ({otherApplyingContact.Email}) to {organisationId}");
                        var request = new AssociateEpaOrganisationWithEpaContactRequest
                        {
                            ContactId = otherApplyingContact.Id,
                            OrganisationId = organisationId,
                            ContactStatus = ContactStatus.InvitePending,
                            MakePrimaryContact = false,
                            AddDefaultRoles = false,
                            AddDefaultPrivileges = false
                        };

                        await _apiClient.AssociateOrganisationWithEpaContact(request);
                    }
                }
            }
        }

        public async Task<CreateOrganisationStandardFromApplyResponse> InjectApplyOrganisationStandardDetailsIntoRegister(CreateOrganisationStandardCommand command)
        {
            var response = new CreateOrganisationStandardFromApplyResponse { WarningMessages = new List<string>() };

            var warningMessages = new List<string>();

            // Organisation checks ////////////////////////////////
            RaiseWarningIfNoOrganisationId(command.OrganisationId, warningMessages);
            RaiseWarningIfOrganisationIdIsInvalid(command.OrganisationId, warningMessages);

            // Standard checks ///////////////////////////////////
            RaiseWarningIfStandardCodeIsInvalid(command.StandardCode, warningMessages);

            var standard = await MapCommandToOrganisationStandardRequest(command);

            // Validate new org standard request before processing it
            if (warningMessages.Count == 0)
            {
                var validationResponse = await _assessorValidationService.ValidateNewOrganisationStandardRequest(standard);

                if (!validationResponse.IsValid)
                {
                    warningMessages.AddRange(validationResponse.Errors.Select(err => err.ErrorMessage));
                }
            }

            if (warningMessages.Count == 0)
            {
                _logger.LogInformation("Injecting new standard into register");
                response.EpaoStandardId = await _apiClient.CreateEpaOrganisationStandard(standard);
            }
            else
            {
                _logger.LogWarning("Source has invalid data. Cannot inject standard details into register at this time");
            }

            response.WarningMessages = warningMessages;

            return response;
        }

        private async Task UpdateFinancialDetails(CreateOrganisationContactCommand command)
        {
            var epaOrgs = await _apiClient.SearchOrganisations(command.OrganisationName);
            var result = epaOrgs.FirstOrDefault();

            if (result != null)
            {
                _logger.LogInformation($"Updating FHADetails for {result.Id}");

                var req = new UpdateFinancialsRequest
                {
                    EpaOrgId = result.Id,
                    FinancialDueDate = command.FinancialDueDate,
                    FinancialExempt = command.IsFinancialExempt
                };

                await _apiClient.UpdateFinancials(req);
            }
        }

        private static string DecideOrganisationName(bool useTradingName, string tradingName, string organisationName)
        {
            return useTradingName && !string.IsNullOrEmpty(tradingName)
                ? tradingName
                : organisationName;
        }

        private static long? GetUkprnFromRequestDetails(string organisationUkprn, string companyUkprn)
        {
            long? ukprnAsLong = null;
            var ukprn = !string.IsNullOrEmpty(organisationUkprn) ? organisationUkprn : companyUkprn;

            if (long.TryParse(ukprn, out long _))
            {
                ukprnAsLong = long.Parse(ukprn);
            }
            return ukprnAsLong;
        }

        private async Task<int?> GetOrganisationTypeIdFromDescriptor(string organisationType)
        {
            var organisationTypes = await _apiClient.GetOrganisationTypes();
            return organisationTypes.FirstOrDefault(x => string.Equals(x.Type.Replace(" ", ""),
                organisationType.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))?.Id;
        }

        private void RaiseWarningIfNoOrganisationName(string organisationName, ICollection<string> warningMessages)
        {
            if (!_validationService.IsNotEmpty(organisationName))
                warningMessages.Add(OrganisationAndContactMessages.NoOrganisationName);
        }

        private void RaiseWarningIfOrganisationNameTooShort(string organisationName, ICollection<string> warningMessages)
        {
            if (!_validationService.IsMinimumLengthOrMore(organisationName, 2))
                warningMessages.Add(OrganisationAndContactMessages.OrganisationNameTooShort);
        }

        private static void RaiseWarningOrganisationTypeNotIdentified(int? organisationTypeId, ICollection<string> warningMessages)
        {
            if (organisationTypeId == null)
                warningMessages.Add(OrganisationAndContactMessages.OrganisationTypeNotIdentified);
        }

        private void RaiseWarningIfUkprnIsInvalid(long? ukprnAsLong, ICollection<string> warningMessages)
        {
            if (ukprnAsLong.HasValue && !_validationService.UkprnIsValid(ukprnAsLong.Value.ToString()))
                warningMessages.Add(OrganisationAndContactMessages.UkprnIsInvalidFormat);
        }

        private void RaiseWarningIfCompanyNumberIsInvalid(string companyNumber, ICollection<string> warningMessages)
        {
            if (!string.IsNullOrEmpty(companyNumber) && !_validationService.CompanyNumberIsValid(companyNumber))
                warningMessages.Add(OrganisationAndContactMessages.CompanyNumberNotValid);
        }

        private void RaiseWarningIfCharityNumberIsInvalid(string charityNumber, ICollection<string> warningMessages)
        {
            if (!string.IsNullOrEmpty(charityNumber) && !_validationService.CharityNumberIsValid(charityNumber))
                warningMessages.Add(OrganisationAndContactMessages.CharityNumberNotValid);
        }

        private void RaiseWarningIfEmailIsMissingOrInvalid(string email, ICollection<string> warningMessagesContact)
        {
            if (!_validationService.IsNotEmpty(email))
                warningMessagesContact.Add(OrganisationAndContactMessages.EmailIsMissing);

            if (!_validationService.CheckEmailIsValid(email))
                warningMessagesContact.Add(OrganisationAndContactMessages.EmailIsInvalid);
        }

        private void RaiseWarningIfContactNameIsMissingOrTooShort(string contactName, List<string> warningMessagesContact)
        {
            if (!_validationService.IsNotEmpty(contactName))
                warningMessagesContact.Add(OrganisationAndContactMessages.ContactNameIsMissing);

            if (!_validationService.IsMinimumLengthOrMore(contactName, 2))
                warningMessagesContact.Add(OrganisationAndContactMessages.ContactNameIsTooShort);
        }

        private void RaiseWarningIfNoOrganisationId(string organisationId, List<string> warningMessages)
        {
            if (!_validationService.IsNotEmpty(organisationId))
                warningMessages.Add(OrganisationAndContactMessages.NoOrganisationId);
        }

        private void RaiseWarningIfOrganisationIdIsInvalid(string organisationId, List<string> warningMessages)
        {
            if (!_validationService.OrganisationIdIsValid(organisationId))
                warningMessages.Add(OrganisationAndContactMessages.OrganisationIdNotValid);
        }

        private void RaiseWarningIfStandardCodeIsInvalid(int standardCode, List<string> warningMessagesStandard)
        {
            if (standardCode < 1)
            {
                warningMessagesStandard.Add(OrganisationAndContactMessages.StandardInvalid);
            }
        }

        private CreateEpaOrganisationRequest MapCommandToOrganisationRequest(CreateOrganisationContactCommand command, string organisationName, long? ukprnAsLong, int? organisationTypeId)
        {
            organisationName = _cleanser.CleanseStringForSpecialCharacters(organisationName);
            var legalName = _cleanser.CleanseStringForSpecialCharacters(command.OrganisationName);
            var tradingName = _cleanser.CleanseStringForSpecialCharacters(command.TradingName);
            var website = _cleanser.CleanseStringForSpecialCharacters(command.StandardWebsite);
            var address1 = _cleanser.CleanseStringForSpecialCharacters(command.ContactAddress1);
            var address2 = _cleanser.CleanseStringForSpecialCharacters(command.ContactAddress2);
            var address3 = _cleanser.CleanseStringForSpecialCharacters(command.ContactAddress3);
            var address4 = _cleanser.CleanseStringForSpecialCharacters(command.ContactAddress4);
            var postcode = _cleanser.CleanseStringForSpecialCharacters(command.ContactPostcode);
            var companyNumber = _cleanser.CleanseStringForSpecialCharacters(command.CompanyNumber);
            var charityNumber = _cleanser.CleanseStringForSpecialCharacters(command.CharityNumber);

            if (!string.IsNullOrWhiteSpace(companyNumber))
            {
                companyNumber = companyNumber.ToUpper();
            }

            return new CreateEpaOrganisationRequest
            {
                Name = organisationName,
                OrganisationTypeId = organisationTypeId,
                Ukprn = ukprnAsLong,
                Address1 = address1,
                Address2 = address2,
                Address3 = address3,
                Address4 = address4,
                LegalName = legalName,
                TradingName = tradingName,
                Postcode = postcode,
                WebsiteLink = website,
                CompanyNumber = companyNumber,
                CharityNumber = charityNumber,
                FinancialDueDate = command.FinancialDueDate,
                FinancialExempt = command.IsFinancialExempt
            };
        }

        private CreateEpaOrganisationContactRequest MapCommandToContactRequest(string contactEmail, string organisationId, string contactPhoneNumber, string givenNames, string familyName)
        {
            contactEmail = _cleanser.CleanseStringForSpecialCharacters(contactEmail);
            contactPhoneNumber = _cleanser.CleanseStringForSpecialCharacters(contactPhoneNumber);
            givenNames = _cleanser.CleanseStringForSpecialCharacters(givenNames);
            familyName = _cleanser.CleanseStringForSpecialCharacters(familyName);
            // NOTE: This used to have 'contactName' as the explicit DisplayName. I've removed it as everywhere else it concatenates givenNames & familyName

            return new CreateEpaOrganisationContactRequest
            {
                Email = contactEmail,
                EndPointAssessorOrganisationId = organisationId,
                PhoneNumber = contactPhoneNumber,
                FirstName = givenNames,
                LastName = familyName
            };
        }

        private async Task<CreateEpaOrganisationStandardRequest> MapCommandToOrganisationStandardRequest(CreateOrganisationStandardCommand command)
        {
            return new CreateEpaOrganisationStandardRequest
            {
                OrganisationId = command.OrganisationId,
                StandardCode = command.StandardCode,
                EffectiveFrom = command.EffectiveFrom,
                ContactId = command.CreatedBy,
                DeliveryAreas = await MapCommandToDeliveryAreas(command),
                DeliveryAreasComments = string.Empty
            };
        }

        private async Task<List<int>> MapCommandToDeliveryAreas(CreateOrganisationStandardCommand command)
        {
            if (command.DeliveryAreas != null)
            {
                var areas = await _apiClient.GetDeliveryAreas();
                return areas.Where(a => command.DeliveryAreas.Contains(a.Area)).Select(a => a.Id).ToList();
            }

            return new List<int>();
        }
    }
}
