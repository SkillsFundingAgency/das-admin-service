﻿namespace SFA.DAS.AdminService.Application.Interfaces.Validation
{
    public interface IValidationService
    {
        bool CheckEmailIsValid(string emailToCheck);
        bool IsNotEmpty(string stringToCheck);
        bool UkprnIsValid(string ukprnToCheck);
        bool UlnIsValid(string ulnToCheck);
        bool IsMinimumLengthOrMore(string stringToCheck, int minimumLength);
        bool IsMaximumLengthOrLess(string stringToCheck, int maximumLength);
        bool DateIsValid(string dateToCheck);
        bool DateIsTodayOrInFuture(string dateToCheck);
        bool DateIsTodayOrInPast(string dateToCheck);
        bool EndPointAssessorOrganisationIdIsValid(string endPointAssessorOrganisationIdToCheck);

        bool CompanyNumberIsValid(string companyNumberToCheck);
        bool CharityNumberIsValid(string charityNumberToCheck);
    }
}