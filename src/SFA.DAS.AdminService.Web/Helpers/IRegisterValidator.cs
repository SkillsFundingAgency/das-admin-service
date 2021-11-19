using SFA.DAS.AdminService.Common.Validation;

namespace SFA.DAS.AdminService.Web.Helpers
{
    public interface IRegisterValidator
    {
        ValidationResponse CheckDateIsEmptyOrValid(string day, string month, string year, 
            string dayFieldName, string monthFieldName, string yearFieldName, string dateFieldName, string dateFieldDescription);

        ValidationResponse CheckDateIsNotEmptyAndIsValid(string day, string month, string year,
                                                         string dayFieldName, string monthFieldName, string yearFieldName,
                                                         string dateFieldName, string dateFieldDescription);

    }
}