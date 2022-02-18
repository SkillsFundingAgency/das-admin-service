using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AdminService.Application.Interfaces;
using SFA.DAS.AdminService.Application.Interfaces.Validation;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AdminService.Web.Resources;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AdminService.Common.Validation;

namespace SFA.DAS.AdminService.Web.Services
{
    public class AnswerInjectionService : IAnswerInjectionService
    {
        private readonly IApiClient _apiClient;
        private readonly IApplicationApiClient _applyApiClient;

        private readonly IValidationService _validationService;
        private readonly IAssessorValidationService _assessorValidationService;

        private readonly ILogger<AnswerService> _logger;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public AnswerInjectionService(IApiClient apiClient, IApplicationApiClient applyApiClient, IValidationService validationService,
            IAssessorValidationService assessorValidationService, ISpecialCharacterCleanserService cleanser, ILogger<AnswerService> logger)
        {
            _apiClient = apiClient;
            _applyApiClient = applyApiClient;
            _validationService = validationService;
            _assessorValidationService = assessorValidationService;
            _cleanser = cleanser;
            _logger = logger;
        }


        public async Task<CreateOrganisationAndContactFromApplyResponse>
            InjectApplyOrganisationAndContactDetailsIntoRegister(CreateOrganisationContactCommand command)
        {
            var response = new CreateOrganisationAndContactFromApplyResponse { IsEpaoApproved = false, WarningMessages = new List<string>() };

            if (command.IsRoEpaoApproved is true)
            {
                await UpdateFinancialDetails(command);
                _logger.LogInformation("Source is RoEPAO approved. No need to inject organisation details into register");
                response.IsEpaoApproved = true;
                return response;
            }

            var warningMessages = new List<string>();
            var organisationName = DecideOrganisationName(command.UseTradingName, command.TradingName, command.OrganisationName);
            var ukprn = GetUkprnFromRequestDetails(command.OrganisationUkprn, command.CompanyUkprn);
            var organisationTypeId = await GetOrganisationTypeIdFromDescriptor(command.OrganisationType);

            // Organisation checks
            RaiseWarningIfNoEpaoId(command.EndPointAssessorOrganisationId, warningMessages);
            RaiseWarningIfEpaoIdIsInvalid(command.EndPointAssessorOrganisationId, warningMessages);
            RaiseWarningIfNoOrganisationName(organisationName, warningMessages);
            RaiseWarningIfOrganisationNameTooShort(organisationName, warningMessages);
            RaiseWarningOrganisationTypeNotIdentified(organisationTypeId, warningMessages);
            RaiseWarningIfUkprnIsInvalid(ukprn, warningMessages);
            RaiseWarningIfCompanyNumberIsInvalid(command.CompanyNumber, warningMessages);
            RaiseWarningIfCharityNumberIsInvalid(command.CharityNumber, warningMessages);

            // Contact checks
            RaiseWarningIfEmailIsMissingOrInvalid(command.ContactEmail, warningMessages);
            RaiseWarningIfContactGivenNameIsMissingOrTooShort(command.ContactGivenNames, warningMessages);
            RaiseWarningIfContactFamilyNameIsMissingOrTooShort(command.ContactFamilyName, warningMessages);
            
            var request = MapCommandToOrganisationRequest(command, organisationName, ukprn, organisationTypeId);

            // If we passed basic pre-checks; then validate fully
            if (warningMessages.Count == 0)
            {    
                var validationResponse = await _assessorValidationService.ValidateUpdateOrganisationRequest(request);

                if (!validationResponse.IsValid)
                {
                    var validationResponseErrors = validationResponse.Errors.Select(err => err.ErrorMessage);
                    warningMessages.AddRange(validationResponseErrors);
                    _logger.LogInformation($"Inject organisation failed on Validation Service. OrganisationId: {command.OrganisationId} - Warnings:  {string.Join(",", validationResponseErrors)}");
                }
            }
            else
            {
                _logger.LogInformation($"Inject organisation failed at pre-check. OrganisationId: {command.OrganisationId} - Warnings:  {string.Join(",", warningMessages)}");
            }

            // If everything has checked out; approve the application
            if (warningMessages.Count == 0)
            {
                _logger.LogInformation($"Approving organisation {request?.Name} onto the register");
                request.Status = OrganisationStatus.New;

                var organisationId = await _apiClient.UpdateEpaOrganisation(request);
                response.OrganisationId = organisationId;

                _logger.LogInformation($"Assigning the primary contact");
                var primaryContact = MapCommandToContactRequest(command.ContactEmail, organisationId, command.ContactPhoneNumber, command.ContactGivenNames, command.ContactFamilyName);
                await AssignPrimaryContactToOrganisation(primaryContact, organisationId);

                _logger.LogInformation($"Assign the applying user default permissions");
                await AssignApplyingContactToOrganisation(command.ApplyingContactEmail, organisationId);

                _logger.LogInformation($"Inviting other applying users");
                await InviteOtherApplyingUsersToOrganisation(command.OtherApplyingUserEmails, organisationId);
            }
            else
            {
                _logger.LogWarning($"Cannot inject organisation details into register at this time. OrganisationId: {command.OrganisationId} - Warnings:  {string.Join(",", warningMessages)}");
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
            RaiseWarningIfNoEpaoId(command.EndPointAssessorOrganisationId, warningMessages);
            RaiseWarningIfEpaoIdIsInvalid(command.EndPointAssessorOrganisationId, warningMessages);

            // Standard checks ///////////////////////////////////
            RaiseWarningIfStandardCodeIsInvalid(command.StandardCode, warningMessages);

            var standard = await MapCommandToOrganisationStandardRequest(command);

            //if a withdrawal exists - then versions must exist - update rather than insert in the assessor service
            var withdrawal = await _applyApiClient.GetWithdrawnApplications(command.OrganisationId, command.StandardCode);
            if (withdrawal.Count != 0)
                standard.ApplyFollowingWithdrawal = true;


            // If we passed basic pre-checks; then validate fully
            if (warningMessages.Count == 0)
            {
                var validationResponse = await _assessorValidationService.ValidateNewOrganisationStandardRequest(standard);

                if (standard.ApplyFollowingWithdrawal && validationResponse.Errors.Count == 1 && 
                    validationResponse.Errors[0].Field == "OrganisationId" && 
                    validationResponse.Errors[0].StatusCode == ValidationStatusCode.AlreadyExists.ToString())
                {
                    _logger.LogInformation($"Inject standard on Validation Service - versions must exist so bypass warnings. OrganisationId: {command.OrganisationId})");
                }
                else
                { 
                    if (!validationResponse.IsValid)
                    {
                        var validationResponseErrors = validationResponse.Errors.Select(err => err.ErrorMessage);
                        warningMessages.AddRange(validationResponseErrors);
                        _logger.LogInformation($"Inject standard failed on Validation Service. OrganisationId: {command.OrganisationId} - Warnings:  {string.Join(",", validationResponseErrors)}");
                    }
                }
            }
            else
            {
                _logger.LogInformation($"Inject standard failed at pre-check. OrganisationId: {command.OrganisationId} - Warnings:  {string.Join(",", warningMessages)}");
            }

            // If everything has checked out; approve the standard
            if (warningMessages.Count == 0)
            {
                _logger.LogInformation("Injecting new standard into register");
                response.EpaoStandardId = await _apiClient.CreateEpaOrganisationStandard(standard);
            }
            else
            {
                _logger.LogWarning($"Cannot inject standard details into register at this time. OrganisationId: {command.OrganisationId} - Warnings:  {string.Join(", ", warningMessages)}");
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

                await _applyApiClient.UpdateFinancials(req);
            }
        }

        private static string DecideOrganisationName(bool useTradingName, string tradingName, string organisationName)
        {
            return useTradingName && !string.IsNullOrEmpty(tradingName)
                ? tradingName
                : organisationName;
        }

        private static int? GetUkprnFromRequestDetails(int? organisationUkprn, string companyUkprn)
        {
            int? ukprnAsInt = null;

            if (organisationUkprn.HasValue)
            {
                ukprnAsInt = organisationUkprn;
            }
            else if(int.TryParse(companyUkprn, out var ukprn))
            {
                ukprnAsInt = ukprn;
            }
            
            return ukprnAsInt;
        }

        private async Task<int?> GetOrganisationTypeIdFromDescriptor(string organisationType)
        {
            var organisationTypes = await _apiClient.GetOrganisationTypes();
            return organisationTypes.FirstOrDefault(x => string.Equals(x.Type?.Replace(" ", ""),
                organisationType?.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))?.Id;
        }

        private void RaiseWarningIfNoOrganisationName(string organisationName, ICollection<string> warningMessages)
        {
            if (!_validationService.IsNotEmpty(organisationName))
                warningMessages.Add(OrganisationAndContactMessages.NoOrganisationName);
        }

        private void RaiseWarningIfOrganisationNameTooShort(string organisationName, ICollection<string> warningMessages)
        {
            if (!_validationService.IsMinimumLengthOrMore(organisationName, 2))
                warningMessages.Add($"{OrganisationAndContactMessages.OrganisationNameTooShort} : '{organisationName}'");
        }

        private static void RaiseWarningOrganisationTypeNotIdentified(int? organisationTypeId, ICollection<string> warningMessages)
        {
            if (organisationTypeId == null)
                warningMessages.Add(OrganisationAndContactMessages.OrganisationTypeNotIdentified);
        }

        private void RaiseWarningIfUkprnIsInvalid(int? ukprn, ICollection<string> warningMessages)
        {
            if (ukprn.HasValue && !_validationService.UkprnIsValid(ukprn.Value.ToString()))
                warningMessages.Add($"{OrganisationAndContactMessages.UkprnIsInvalidFormat} : '{ukprn}'");
        }

        private void RaiseWarningIfCompanyNumberIsInvalid(string companyNumber, ICollection<string> warningMessages)
        {
            if (!string.IsNullOrEmpty(companyNumber) && !_validationService.CompanyNumberIsValid(companyNumber))
                warningMessages.Add($"{OrganisationAndContactMessages.CompanyNumberNotValid} : '{companyNumber}'");
        }

        private void RaiseWarningIfCharityNumberIsInvalid(string charityNumber, ICollection<string> warningMessages)
        {
            if (!string.IsNullOrEmpty(charityNumber) && !_validationService.CharityNumberIsValid(charityNumber))
                warningMessages.Add($"{OrganisationAndContactMessages.CharityNumberNotValid} : '{charityNumber}'");
        }

        private void RaiseWarningIfEmailIsMissingOrInvalid(string email, ICollection<string> warningMessagesContact)
        {
            if (!_validationService.IsNotEmpty(email))
                warningMessagesContact.Add(OrganisationAndContactMessages.EmailIsMissing);

            if (!_validationService.CheckEmailIsValid(email))
                warningMessagesContact.Add($"{OrganisationAndContactMessages.EmailIsInvalid} : '{email}'");
        }

        private void RaiseWarningIfContactGivenNameIsMissingOrTooShort(string contactGivenNames, List<string> warningMessagesContact)
        {
            if (!_validationService.IsNotEmpty(contactGivenNames))
                warningMessagesContact.Add(OrganisationAndContactMessages.ContactGivenNamesIsMissing);

            if (!_validationService.IsMinimumLengthOrMore(contactGivenNames, 2))
                warningMessagesContact.Add($"{OrganisationAndContactMessages.ContactGivenNameIsTooShort} : '{contactGivenNames}'");
        }

        private void RaiseWarningIfContactFamilyNameIsMissingOrTooShort(string contactFamilyName, List<string> warningMessagesContact)
        {
            if (!_validationService.IsNotEmpty(contactFamilyName))
                warningMessagesContact.Add(OrganisationAndContactMessages.ContactFamilyNameIsMissing);

            if (!_validationService.IsMinimumLengthOrMore(contactFamilyName, 2))
                warningMessagesContact.Add($"{OrganisationAndContactMessages.ContactFamilyNameIsTooShort} : '{contactFamilyName}'");
        }

        private void RaiseWarningIfNoEpaoId(string endPointAssessorOrganisationId, List<string> warningMessages)
        {
            if (!_validationService.IsNotEmpty(endPointAssessorOrganisationId))
                warningMessages.Add(OrganisationAndContactMessages.NoOrganisationId);
        }

        private void RaiseWarningIfEpaoIdIsInvalid(string endPointAssessorOrganisationId, List<string> warningMessages)
        {
            if (!_validationService.EndPointAssessorOrganisationIdIsValid(endPointAssessorOrganisationId))
                warningMessages.Add($"{OrganisationAndContactMessages.OrganisationIdNotValid} : '{endPointAssessorOrganisationId}'");
        }

        private void RaiseWarningIfStandardCodeIsInvalid(int standardCode, List<string> warningMessagesStandard)
        {
            if (standardCode < 1)
            {
                warningMessagesStandard.Add($"{OrganisationAndContactMessages.StandardInvalid} : '{standardCode}'");
            }
        }

        private UpdateEpaOrganisationRequest MapCommandToOrganisationRequest(CreateOrganisationContactCommand command, string organisationName, long? ukprn, int? organisationTypeId)
        {
            organisationName = _cleanser.CleanseStringForSpecialCharacters(organisationName);
            var organisationId = _cleanser.CleanseStringForSpecialCharacters(command.EndPointAssessorOrganisationId);
            var legalName = _cleanser.CleanseStringForSpecialCharacters(command.OrganisationName);
            var tradingName = _cleanser.CleanseStringForSpecialCharacters(command.TradingName);
            var email = _cleanser.CleanseStringForSpecialCharacters(command.ContactEmail);
            var phonenumber = _cleanser.CleanseStringForSpecialCharacters(command.ContactPhoneNumber);
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

            return new UpdateEpaOrganisationRequest
            {
                Name = organisationName,
                OrganisationId = organisationId,
                OrganisationTypeId = organisationTypeId,
                Ukprn = ukprn,
                Status = null, 
                LegalName = legalName,
                TradingName = tradingName,
                Email = email,
                PhoneNumber = phonenumber,
                WebsiteLink = website,
                Address1 = address1,
                Address2 = address2,
                Address3 = address3,
                Address4 = address4,   
                Postcode = postcode,
                CompanyNumber = companyNumber,
                CharityNumber = charityNumber,
                FinancialDueDate = command.FinancialDueDate,
                FinancialExempt = command.IsFinancialExempt,
                ActionChoice = "ApproveApplication" // This will set: RoEPAOApproved = true
            };
        }

        private CreateEpaOrganisationContactRequest MapCommandToContactRequest(string contactEmail, string organisationId, string contactPhoneNumber, string givenNames, string familyName)
        {
            contactEmail = _cleanser.CleanseStringForSpecialCharacters(contactEmail);
            contactPhoneNumber = _cleanser.CleanseStringForSpecialCharacters(contactPhoneNumber);
            givenNames = _cleanser.CleanseStringForSpecialCharacters(givenNames);
            familyName = _cleanser.CleanseStringForSpecialCharacters(familyName);

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
            var organisationId = _cleanser.CleanseStringForSpecialCharacters(command.EndPointAssessorOrganisationId);

            return new CreateEpaOrganisationStandardRequest
            {
                OrganisationId = organisationId,
                StandardCode = command.StandardCode,
                StandardReference = command.StandardReference,
                StandardVersions = command.StandardVersions,
                DateStandardApprovedOnRegister = command.DateStandardApprovedOnRegister,
                EffectiveFrom = command.EffectiveFrom,
                ContactId = command.ApplyingContactId.ToString(),
                DeliveryAreas = await MapCommandToDeliveryAreas(command),
                DeliveryAreasComments = string.Empty,
                StandardApplicationType = command.StandardApplicationType
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
